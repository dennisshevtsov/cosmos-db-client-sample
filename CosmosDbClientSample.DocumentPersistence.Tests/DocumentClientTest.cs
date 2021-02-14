// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace CosmosDbClientSample.DocumentPersistence.Tests
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using CosmosDbClientSample.DocumentPersistence;

  [Ignore]
  [TestClass]
  public sealed class DocumentClientTest
  {
    private IDisposable _disposable;
    private IDocumentClient _documentClient;

    [TestInitialize]
    public void Initialize()
    {
      var configuration =
        new ConfigurationBuilder().AddJsonFile("local.settings.json")
                                  .Build();
      var provider =
        new ServiceCollection().AddDocumentClient(
                                  options =>
                                  {
                                    options.AccountEndpoint = configuration[nameof(DocumentClientOptions.AccountEndpoint)];
                                    options.AccountKey = configuration[nameof(DocumentClientOptions.AccountKey)];
                                    options.DatabaseId = configuration[nameof(DocumentClientOptions.DatabaseId)];
                                    options.ContainerId = configuration[nameof(DocumentClientOptions.ContainerId)];

                                    if (int.TryParse(configuration[nameof(DocumentClientOptions.ItemsPerRequest)], out var itemsPerRequest))
                                    {
                                      options.ItemsPerRequest = itemsPerRequest;
                                    }
                                    else
                                    {
                                      options.ItemsPerRequest = 50;
                                    }
                                  })
                               .BuildServiceProvider();

      _disposable = provider;
      _documentClient = provider.GetRequiredService<IDocumentClient>();
    }

    [TestCleanup]
    public void Cleanup() => _disposable?.Dispose();

    [TestMethod]
    public async Task Test_Insert_First_Update_Delete()
    {
      var creating = DocumentClientTest.NewDocument();
      var created = await _documentClient.InsertAsync(creating, CancellationToken.None);

      DocumentClientTest.Test(creating, created);

      var received = await _documentClient.FirstOrDefaultAsync<TestDocument>(
        created.Id, created.Type, CancellationToken.None);

      DocumentClientTest.Test(created, received);

      DocumentClientTest.Update(created);

      var updated = await _documentClient.UpdateAsync(created, CancellationToken.None);

      DocumentClientTest.Test(created, updated);

      await _documentClient.DeleteAsync(updated.Id, updated.Type, CancellationToken.None);

      var deleted = await _documentClient.FirstOrDefaultAsync<TestDocument>(
          updated.Id, updated.Type, CancellationToken.None);

      Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task Test_AsEnumerable()
    {
      var document0 = await _documentClient.InsertAsync(
        DocumentClientTest.NewDocument(), CancellationToken.None);
      var document1 = await _documentClient.InsertAsync(
        DocumentClientTest.NewDocument(), CancellationToken.None);
      var document2 = await _documentClient.InsertAsync(
        DocumentClientTest.NewDocument(), CancellationToken.None);
      var document3 = await _documentClient.InsertAsync(
        DocumentClientTest.NewDocument(), CancellationToken.None);
      var document4 = await _documentClient.InsertAsync(
        DocumentClientTest.NewDocument(), CancellationToken.None);

      await foreach (var document in _documentClient.AsAsyncEnumerable<TestDocument>(
        nameof(TestDocument), "SELECT * FROM c", null, CancellationToken.None))
      {
        Assert.IsNotNull(document);
      }

      await _documentClient.DeleteAsync(document0.Id, document0.Type, CancellationToken.None);
      await _documentClient.DeleteAsync(document1.Id, document1.Type, CancellationToken.None);
      await _documentClient.DeleteAsync(document2.Id, document2.Type, CancellationToken.None);
      await _documentClient.DeleteAsync(document3.Id, document3.Type, CancellationToken.None);
      await _documentClient.DeleteAsync(document4.Id, document4.Type, CancellationToken.None);
    }

    [TestMethod]
    public async Task Test_Double_Update()
    {
      var creating = DocumentClientTest.NewDocument();
      var created = await _documentClient.InsertAsync(creating, CancellationToken.None);

      var document0 = created;
      var document1 = created;

      DocumentClientTest.Update(document0);
      DocumentClientTest.Update(document1);

      var updated0 = await _documentClient.UpdateAsync(document0, CancellationToken.None);
      var updated1 = await _documentClient.UpdateAsync(document1, CancellationToken.None);

      DocumentClientTest.Test(updated0, updated1);
    }

    private static TestDocument NewDocument()
      => new TestDocument
      {
        StringProperty = "test0",
        DateTimeProperty = DateTime.UtcNow,
        GuidProperty = Guid.NewGuid(),
        EmbeddedProperty = new EmbeddedTestDocument
        {
          StringProperty = "test1",
          DateTimeProperty = DateTime.UtcNow,
          GuidProperty = Guid.NewGuid(),
        },
      };

    private static void Test(TestDocument expected, TestDocument actual)
    {
      Assert.IsNotNull(actual);

      Assert.IsFalse(string.IsNullOrWhiteSpace(actual.ResourceId));
      Assert.IsFalse(string.IsNullOrWhiteSpace(actual.SelfLink));
      Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Etag));
      Assert.IsFalse(string.IsNullOrWhiteSpace(actual.AttachmentsLink));
      Assert.IsTrue(actual.Timestamp != default);

      Assert.AreEqual(expected.StringProperty, actual.StringProperty);
      Assert.AreEqual(expected.DateTimeProperty, actual.DateTimeProperty);
      Assert.AreEqual(expected.GuidProperty, actual.GuidProperty);

      Assert.IsNotNull(actual.EmbeddedProperty);
      Assert.AreEqual(expected.EmbeddedProperty.StringProperty, actual.EmbeddedProperty.StringProperty);
      Assert.AreEqual(expected.EmbeddedProperty.DateTimeProperty, actual.EmbeddedProperty.DateTimeProperty);
      Assert.AreEqual(expected.EmbeddedProperty.GuidProperty, actual.EmbeddedProperty.GuidProperty);
    }

    private static void Update(TestDocument document)
    {
      document.StringProperty = Guid.NewGuid().ToString();
      document.DateTimeProperty = DateTime.UtcNow;
      document.GuidProperty = Guid.NewGuid();

      document.EmbeddedProperty.StringProperty = Guid.NewGuid().ToString();
      document.EmbeddedProperty.GuidProperty = Guid.NewGuid();
      document.EmbeddedProperty.DateTimeProperty = DateTime.UtcNow;
    }
  }
}
