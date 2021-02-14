// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using Microsoft.IO;

  /// <summary>Provides a simple API to receive an instance of the <see cref="RecyclableMemoryStream"/> class.</summary>
  public sealed class RecyclableMemoryStreamManagerProvider
  {
    private readonly RecyclableMemoryStreamManager _streamManager;

    /// <summary>Initializes a new instance of the <see cref="RecyclableMemoryStreamManagerProvider"/> class.</summary>
    public RecyclableMemoryStreamManagerProvider() => _streamManager = new RecyclableMemoryStreamManager();

    /// <summary>Gets an instance of the <see cref="RecyclableMemoryStreamManager"/> class.</summary>
    /// <returns>An instance of the <see cref="RecyclableMemoryStreamManager"/> class.</returns>
    public RecyclableMemoryStreamManager Get() => _streamManager;
  }
}
