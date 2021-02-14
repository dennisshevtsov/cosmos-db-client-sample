// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System.Collections.Generic;

  /// <summary>Represents detail of a document collection response.</summary>
  /// <typeparam name="TDocument"></typeparam>
  public sealed class DocumentCollection<TDocument> where TDocument : DocumentBase
  {
    /// <summary>Gets/sets a value that represents an ID of a resource.</summary>
    public string ResourceId { get; set; }

    /// <summary>Gets/sets a value that represents number of documents.</summary>
    public int Count { get; set; }

    /// <summary>Gets/sets an object that represents a collection of documents.</summary>
    public IEnumerable<TDocument> Documents { get; set; }
  }
}
