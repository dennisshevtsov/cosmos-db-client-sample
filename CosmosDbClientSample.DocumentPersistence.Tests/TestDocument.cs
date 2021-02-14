// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence.Tests
{
  using System;

  using CosmosDbClientSample.DocumentPersistence;

  public sealed class TestDocument : DocumentBase
  {
    public string StringProperty { get; set; }

    public DateTime DateTimeProperty { get; set; }

    public Guid GuidProperty { get; set; }

    public EmbeddedTestDocument EmbeddedProperty { get; set; }
  }

  public sealed class EmbeddedTestDocument
  {
    public string StringProperty { get; set; }

    public DateTime DateTimeProperty { get; set; }

    public Guid GuidProperty { get; set; }
  }
}
