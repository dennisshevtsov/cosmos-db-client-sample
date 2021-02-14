// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>Extends the API of the <see cref="CosmosDbClientSample.DocumentPersistence.IDocumentClient"/>.</summary>
  public static class DocumentClientExtensions
  {
    /// <summary>Receives documents from their persistence.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="query">A value that represents a condition to query documents.</param>
    /// <param name="parameters">An object that represents a collection of parameters for a query.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public static async Task<IList<TDocument>> ToListAsync<TDocument>(
      this IDocumentClient documentClient,
      string partitionId,
      string query,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken)
      where TDocument : DocumentBase
    {
      if (documentClient == null)
      {
        throw new ArgumentNullException(nameof(documentClient));
      }

      var documents = new List<TDocument>();

      await foreach (var document in documentClient.AsAsyncEnumerable<TDocument>(
        partitionId, query, parameters, cancellationToken))
      {
        documents.Add(document);
      }

      return documents;
    }

    /// <summary>Receives documents from their persistence that are grouped by its ID.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="query">A value that represents a condition to query documents.</param>
    /// <param name="parameters">An object that represents a collection of parameters for a query.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public static async Task<IDictionary<Guid, TDocument>> ToDictionaryAsync<TDocument>(
      this IDocumentClient documentClient,
      string partitionId,
      string query,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken)
      where TDocument : DocumentBase
    {
      if (documentClient == null)
      {
        throw new ArgumentNullException(nameof(documentClient));
      }

      var documentDictionary = new Dictionary<Guid, TDocument>();

      await foreach (var document in documentClient.AsAsyncEnumerable<TDocument>(
        partitionId, query, parameters, cancellationToken))
      {
        documentDictionary.Add(document.Id, document);
      }

      return documentDictionary;
    }
  }
}
