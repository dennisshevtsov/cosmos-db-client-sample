// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>Provides a simple API to persistence of documents that inherits the <see cref="CosmosDbClientSample.DocumentPersistence.DocumentBase"/> class.</summary>
  public interface IDocumentClient
  {
    /// <summary>Receives a document from its persistence.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="id">A value that represents an ID of a document.</param>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public Task<TDocument> FirstOrDefaultAsync<TDocument>(
      Guid id, string partitionId, CancellationToken cancellationToken) where TDocument : DocumentBase;

    /// <summary>Receives documents from their persistence.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="query">A value that represents a condition to query documents.</param>
    /// <param name="parameters">An object that represents a collection of parameters for a query.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public IAsyncEnumerable<TDocument> AsAsyncEnumerable<TDocument>(
      string partitionId,
      string query,
      IDictionary<string, object> parameters,
      CancellationToken cancellationToken) where TDocument : DocumentBase;

    /// <summary>Creates persistence for a document.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="document">An object that represents a document, for which it is required to create persistence.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public Task<TDocument> InsertAsync<TDocument>(
      TDocument document, CancellationToken cancellationToken) where TDocument : DocumentBase;

    /// <summary>Updates persistence of a document.</summary>
    /// <typeparam name="TDocument">A type of a document.</typeparam>
    /// <param name="document">An object that represents a document, which persistence is required to update.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public Task<TDocument> UpdateAsync<TDocument>(
      TDocument document, CancellationToken cancellationToken) where TDocument : DocumentBase;

    /// <summary>Deletes persistence of a document.</summary>
    /// <param name="id">A value that represents an ID of a document.</param>
    /// <param name="partitionId">A value that represents a partition ID of a document.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public Task DeleteAsync(Guid id, string partitionId, CancellationToken cancellationToken);
  }
}
