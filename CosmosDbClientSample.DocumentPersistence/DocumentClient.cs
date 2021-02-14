// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;
  using System.Runtime.CompilerServices;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Azure.Cosmos;
  using Microsoft.Extensions.Options;
  using Microsoft.IO;

  /// <summary>Provides a simple API to persistence of documents that inherits the <see cref="CosmosDbClientSample.DocumentPersistence.DocumentBase"/> class.</summary>
  public sealed class DocumentClient : IDocumentClient
  {
    private readonly DocumentClientOptions _requestOptions;
    private readonly Container _container;
    private readonly RecyclableMemoryStreamManager _streamManager;
    private readonly IDocumentSerializer _serializer;

    /// <summary>Initializes a new instance of the <see cref="DocumentClient"/> class.</summary>
    /// <param name="requestOptions">An object that represents options of a request to Cosmos DB.</param>
    /// <param name="container">An object that provides operations for reading, replacing, or deleting a specific, existing container or item in a container by ID.</param>
    /// <param name="streamManagerProvider">An object that provides a simple API to receive an instance of the <see cref="RecyclableMemoryStream"/> class.</param>
    /// <param name="serializer">An object that provides a simple API to serialize/deserialize objects.</param>
    public DocumentClient(
      IOptions<DocumentClientOptions> requestOptions,
      Container container,
      RecyclableMemoryStreamManagerProvider streamManagerProvider,
      IDocumentSerializer serializer)
    {
      _requestOptions = requestOptions?.Value ?? throw new ArgumentNullException(nameof(requestOptions));
      _container = container ?? throw new ArgumentNullException(nameof(container));
      _streamManager = streamManagerProvider?.Get() ?? throw new ArgumentNullException(nameof(streamManagerProvider));
      _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    /// <summary>Receives a document from its persistence.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="id">A value that represents an ID of a document.</param>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public async Task<TDocument> FirstOrDefaultAsync<TDocument>(
      Guid id, string partitionId, CancellationToken cancellationToken) where TDocument : DocumentBase
    {
      using (var responseMessage = await _container.ReadItemStreamAsync(id.ToString(), new PartitionKey(partitionId), null, cancellationToken))
      {
        TDocument document = null;

        if (responseMessage.StatusCode != HttpStatusCode.NotFound)
        {
          responseMessage.EnsureSuccessStatusCode();

          document = await _serializer.DeserializeAsync<TDocument>(
            responseMessage.Content, cancellationToken);
        }

        return document;
      }
    }

    /// <summary>Receives documents from their persistence.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="query">A value that represents a condition to query documents.</param>
    /// <param name="parameters">An object that represents a collection of parameters for a query.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public async IAsyncEnumerable<TDocument> AsAsyncEnumerable<TDocument>(
      string partitionId,
      string query,
      IDictionary<string, object> parameters,
      [EnumeratorCancellation] CancellationToken cancellationToken) where TDocument : DocumentBase
    {
      var queryDefinition = new QueryDefinition(query);

      if (parameters != null)
      {
        foreach (var parameter in parameters)
        {
          queryDefinition.WithParameter(parameter.Key, parameter.Value);
        }
      }

      var feedIterator = _container.GetItemQueryStreamIterator(
        queryDefinition,
        null,
        new QueryRequestOptions
        {
          PartitionKey = new PartitionKey(partitionId),
          MaxItemCount = _requestOptions.ItemsPerRequest,
        });

      while (feedIterator.HasMoreResults)
      {
        using (var responseMessage = await feedIterator.ReadNextAsync(cancellationToken))
        {
          responseMessage.EnsureSuccessStatusCode();

          var documentCollection = await _serializer.DeserializeAsync<DocumentCollection<TDocument>>(
            responseMessage.Content, cancellationToken);

          foreach (var document in documentCollection.Documents)
          {
            yield return document;
          }
        }
      }
    }

    /// <summary>Creates persistence for a document.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="document">An object that represents a document, for which it is required to create persistence.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public async Task<TDocument> InsertAsync<TDocument>(
      TDocument document, CancellationToken cancellationToken) where TDocument : DocumentBase
    {
      if (document.Id == default)
      {
        document.Id = Guid.NewGuid();
      }

      if (document.Type == null)
      {
        document.Type = typeof(TDocument).Name;
      }

      using (var stream = _streamManager.GetStream())
      {
        await _serializer.SerializeAsync(stream, document, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        using (var responseMessage = await _container.CreateItemStreamAsync(
          stream, new PartitionKey(document.Type), null, cancellationToken))
        {
          responseMessage.EnsureSuccessStatusCode();

          document = await _serializer.DeserializeAsync<TDocument>(
            responseMessage.Content, cancellationToken);

          return document;
        }
      }
    }

    /// <summary>Updates persistence of a document.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="document">An object that represents a document, which persistence is required to update.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public async Task<TDocument> UpdateAsync<TDocument>(
      TDocument document, CancellationToken cancellationToken) where TDocument : DocumentBase
    {
      using (var stream = _streamManager.GetStream())
      {
        await _serializer.SerializeAsync(stream, document, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        using (var responseMessage = await _container.ReplaceItemStreamAsync(
          stream, document.Id.ToString(), new PartitionKey(document.Type), null, cancellationToken))
        {
          responseMessage.EnsureSuccessStatusCode();

          document = await _serializer.DeserializeAsync<TDocument>(
            responseMessage.Content, cancellationToken);

          return document;
        }
      }
    }

    /// <summary>Deletes persistence of a document.</summary>
    /// <param name="id">A value that represents an ID of a document.</param>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public async Task DeleteAsync(Guid id, string partitionId, CancellationToken cancellationToken)
    {
      using (var responseMessage = await _container.DeleteItemStreamAsync(id.ToString(), new PartitionKey(partitionId), null, cancellationToken))
      {
        responseMessage.EnsureSuccessStatusCode();
      }
    }
  }
}
