// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System;

  /// <summary>Represents a base of a document in the Azure Cosmos DB.</summary>
  public abstract class DocumentBase
  {
    /// <summary>Gets/sets a value that represents an ID of a document in the Azure Cosmos DB service.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets/sets a value that represents a partition ID of a document in the Azure Cosmos DB service.</summary>
    public string Type { get; set; }

    /// <summary>Gets/sets a value that represents a resource ID associated with a document in the Azure Cosmos DB service.</summary>
    public string ResourceId { get; set; }

    /// <summary>Gets/sets a value that represents a self-link associated with a document in the Azure Cosmos DB service.</summary>
    public string SelfLink { get; set; }

    /// <summary>Gets/sets a value that represents an entity tag associated with a document in the Azure Cosmos DB service.</summary>
    public string Etag { get; set; }

    /// <summary>Gets/sets a value that represents the self-link associated with attachments of a document in the Azure Cosmos DB service.</summary>
    public string AttachmentsLink { get; set; }

    /// <summary>Gets/sets a value that represents a last modified timestamp associated with a document from the Azure Cosmos DB service.</summary>
    public int Timestamp { get; set; }
  }
}
