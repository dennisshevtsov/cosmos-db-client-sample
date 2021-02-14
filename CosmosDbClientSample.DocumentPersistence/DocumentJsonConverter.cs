// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Text.Json;
  using System.Text.Json.Serialization;

  /// <summary>Converts an object or value to or from JSON.</summary>
  /// <typeparam name="TDocument">The type of object or value handled by the converter.</typeparam>
  public sealed class DocumentJsonConverter<TDocument> : JsonConverter<TDocument> where TDocument : DocumentBase
  {
    /// <summary>Reads and converts the JSON to type T.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override TDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      var document = Activator.CreateInstance<TDocument>();

      int braces = 0;
      string propertyName = null;

      do
      {
        if (reader.TokenType == JsonTokenType.StartObject && propertyName == null)
        {
          ++braces;
        }
        else if (reader.TokenType == JsonTokenType.EndObject)
        {
          --braces;
        }
        else if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
        }
        else if (propertyName != null)
        {
          var property = GetProperty(propertyName, options);
          var propertyValue = JsonSerializer.Deserialize(ref reader, property.PropertyType, options);

          property.SetValue(document, propertyValue);

          propertyName = null;
        }
      }
      while (braces != 0 && reader.Read());

      return document;
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="document">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, TDocument document, JsonSerializerOptions options)
    {
      writer.WriteStartObject();

      var properties = typeof(TDocument).GetProperties();

      foreach (var property in properties)
      {
        writer.WritePropertyName(GetJsonPropertyName(property, options));

        JsonSerializer.Serialize(writer, property.GetValue(document), property.PropertyType, options);
      }

      writer.WriteEndObject();
    }

    private IDictionary<string, PropertyInfo> _properties;
    private IDictionary<string, PropertyInfo> GetProperties(JsonSerializerOptions options)
    {
      if (_properties == null)
      {
        _properties = new Dictionary<string, PropertyInfo>();

        foreach (var property in typeof(TDocument).GetProperties())
        {
          _properties.Add(GetJsonPropertyName(property, options), property);
        }
      }

      return _properties;
    }

    private string GetJsonPropertyName(PropertyInfo property, JsonSerializerOptions options)
    {
      string jsonPropertyName;

      if (!DocumentPropertyNames.TypeJsonNameDictionary.TryGetValue(property.Name, out jsonPropertyName))
      {
        jsonPropertyName = options.PropertyNamingPolicy.ConvertName(property.Name);
      }

      return jsonPropertyName;
    }

    private PropertyInfo GetProperty(string jsonPropertyName, JsonSerializerOptions options)
      => GetProperties(options)[jsonPropertyName];
  }
}
