// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  /// <summary>Represents options of a request to Cosmos DB.</summary>
  public sealed class DocumentClientOptions
  {
    /// <summary>Gets/sets a value that represents an account endpoint.</summary>
    public string AccountEndpoint { get; set; }

    /// <summary>Gets/sets a value that represents an account key.</summary>
    public string AccountKey { get; set; }

    /// <summary>Gets/sets a value that represents an database ID.</summary>
    public string DatabaseId { get; set; }

    /// <summary>Gets/sets a value that represents an container ID.</summary>
    public string ContainerId { get; set; }

    /// <summary>Gets/sets a value that represents how many items there can be in a request.</summary>
    public int ItemsPerRequest { get; set; }
  }
}
