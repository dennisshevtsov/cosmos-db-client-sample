// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System.Text.Json;
  using System.Text.Json.Serialization;

  /// <summary>Extends the API of the <see cref="System.Text.Json.Utf8JsonReader"/> class.</summary>
  public static class JsonReaderExtensions
  {
    /// <summary>Reads an object from an instance of the <see cref="System.Text.Json.Utf8JsonReader"/> class.</summary>
    /// <typeparam name="TDocument">A type of a class that represents a JSON object.</typeparam>
    /// <param name="reader">An object that provides a high-performance API for forward-only, read-only access to UTF-8 encoded JSON text.</param>
    /// <param name="options">An object that provides options to be used with <see cref="System.Text.Json.JsonSerializer"/>.</param>
    /// <returns>An instance of a class that represents a JSON object.</returns>
    public static TDocument Read<TDocument>(
      this ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      if (options.GetConverter(typeof(TDocument)) is JsonConverter<TDocument> converter)
      {
        return converter.Read(ref reader, typeof(TDocument), options);
      }

      return JsonSerializer.Deserialize<TDocument>(ref reader, options);
    }
  }
}
