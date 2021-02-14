// Copyright (c) Dennis Shevtsov. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection
{
  using System;

  using Microsoft.Azure.Cosmos;
  using Microsoft.Extensions.Options;

  using CosmosDbClientSample.DocumentPersistence;

  /// <summary>Provides a simple API to register the document client API.</summary>
  public static class DocumentClientServicesExtensions
  {
    /// <summary>Registers the document client API.</summary>
    /// <param name="services">An object that specifies the contract for a collection of service descriptors.</param>
    /// <param name="configure">An object that provides an operation to configure and register an instance of the <see cref="CosmosDbClientSample.DocumentPersistence.DocumentClientOptions"/> class.</param>
    /// <returns>An object that specifies the contract for a collection of service descriptors.</returns>
    public static IServiceCollection AddDocumentClient(
      this IServiceCollection services,
      Action<DocumentClientOptions> configure)
    {
      if (services == null)
      {
        throw new ArgumentNullException(nameof(services));
      }

      services.AddScoped(provider =>
      {
        var options = provider.GetRequiredService<IOptions<DocumentClientOptions>>()
                              .Value;
        var cosmosClient = new CosmosClient(options.AccountEndpoint, options.AccountKey);

        return cosmosClient;
      });
      services.AddScoped(provider =>
      {
        var options = provider.GetRequiredService<IOptions<DocumentClientOptions>>()
                              .Value;
        var container = provider.GetRequiredService<CosmosClient>()
                                .GetContainer(options.DatabaseId, options.ContainerId);

        return container;
      });

      services.AddSingleton<RecyclableMemoryStreamManagerProvider>();
      services.AddScoped(provider => DocumentSerializer.Get());
      services.AddScoped<IDocumentClient, DocumentClient>();
      services.Configure(configure);

      return services;
    }
  }
}
