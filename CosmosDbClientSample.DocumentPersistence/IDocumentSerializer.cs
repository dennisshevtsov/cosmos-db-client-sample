// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>Provides a simple API to serialize/deserialize objects.</summary>
  public interface IDocumentSerializer
  {
    /// <summary>Serializes an object to a stream.</summary>
    /// <typeparam name="TDocument">A type of a serializing object.</typeparam>
    /// <param name="output">An object that represents an output stream.</param>
    /// <param name="document">An object that it is required to serialize.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public Task SerializeAsync<TDocument>(Stream output, TDocument document, CancellationToken cancellationToken) where TDocument : class;

    /// <summary>Deserializes an object from a stream.</summary>
    /// <typeparam name="TDocument">A type of a serializing object.</typeparam>
    /// <param name="input">An object that represents an input stream.</param>
    /// <param name="cancellationToken">A value that propagates notification that operations should be canceled.</param>
    /// <returns>An object that represents an async operation.</returns>
    public ValueTask<TDocument> DeserializeAsync<TDocument>(Stream input, CancellationToken cancellationToken) where TDocument : class;
  }
}
