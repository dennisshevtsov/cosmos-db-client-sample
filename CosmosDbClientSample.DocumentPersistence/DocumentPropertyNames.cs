// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

using System.Collections.Generic;

namespace CosmosDbClientSample.DocumentPersistence
{
  /// <summary>Represents a set of document base property names.</summary>
  public static class DocumentPropertyNames
  {
    /// <summary>A value that represents a name of the ID property.</summary>
    public const string IdPropertyName = "id";

    /// <summary>A value that represents a name of the partition ID property.</summary>
    public const string PartitionIdPropertyName = "_type";

    /// <summary>A value that represents a name of the resource ID property.</summary>
    public const string ResourceIdPropertyName = "_rid";

    /// <summary>A value that represents a name of the self link property.</summary>
    public const string SelfLinkPropertyName = "_self";

    /// <summary>A value that represents a name of the etag property.</summary>
    public const string EtagPropertyName = "_etag";

    /// <summary>A value that represents a name of the attachments link property.</summary>
    public const string AttachmentsLinkPropertyName = "_attachments";

    /// <summary>A value that represents a name of the timestamp property.</summary>
    public const string TimestampPropertyName = "_ts";

    /// <summary>An object that represents a dictionary of base document properties.</summary>
    public static readonly IDictionary<string, string> TypeJsonNameDictionary =
      new Dictionary<string, string>
      {
        { nameof(DocumentBase.Id), DocumentPropertyNames.IdPropertyName },
        { nameof(DocumentBase.Type), DocumentPropertyNames.PartitionIdPropertyName },
        { nameof(DocumentBase.ResourceId), DocumentPropertyNames.ResourceIdPropertyName },
        { nameof(DocumentBase.SelfLink), DocumentPropertyNames.SelfLinkPropertyName },
        { nameof(DocumentBase.Etag), DocumentPropertyNames.EtagPropertyName },
        { nameof(DocumentBase.AttachmentsLink), DocumentPropertyNames.AttachmentsLinkPropertyName },
        { nameof(DocumentBase.Timestamp), DocumentPropertyNames.TimestampPropertyName },
      };
  }
}
