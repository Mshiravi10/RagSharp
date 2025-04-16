using Microsoft.Extensions.DependencyInjection;
using RagSharp.Abstractions;
using RagSharp.Core;

namespace RagSharp.Extensions;

public static class RagSharpServiceCollectionExtensions
{
    public static IServiceCollection AddRagSharp(this IServiceCollection services, Action<RagSharpOptions> configure)
    {
        var options = new RagSharpOptions();
        configure(options);

        if (options.EmbeddingService == null)
            throw new ArgumentNullException(nameof(options.EmbeddingService), "EmbeddingService is required.");

        if (options.MemoryStore == null)
            throw new ArgumentNullException(nameof(options.MemoryStore), "MemoryStore is required.");

        services.AddSingleton(options);
        services.AddSingleton<IEmbeddingService>(options.EmbeddingService);
        services.AddSingleton<IMemoryStore>(options.MemoryStore);
        services.AddSingleton<IEnumerable<IFileParser>>(options.FileParsers);
        services.AddScoped<IRagSharpService, RagSharpService>();

        return services;
    }
}
