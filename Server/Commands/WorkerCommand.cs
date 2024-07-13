using Microsoft.Extensions.DependencyInjection;
using ILogger = Serilog.ILogger;

namespace Server.Commands;

public interface IWorkerExecutor<in TOptions>
		where TOptions : Command.BaseOptions
{
	Task<int> RunInner(TOptions options, CancellationToken cancellationToken);
}

public abstract class WorkerCommand<TExecutor, TOptions>
	where TOptions : Command.BaseOptions
	where TExecutor : class, IWorkerExecutor<TOptions>
{
	public static Task<int> Run(TOptions options, CancellationToken cancellationToken)
	{
		var serviceProvider = BuildServiceProvider(options);
		serviceProvider.GetRequiredService<ILogger>().Information("Starting with options: {@options}", options);
		var executor = serviceProvider.GetRequiredService<TExecutor>();
		return executor.RunInner(options, cancellationToken);
	}

	protected static IServiceProvider BuildServiceProvider(TOptions options)
	{
		var services = new ServiceCollection();
		options.ConfigureServices(services);
		services.AddSingleton<TOptions>();
		services.AddSingleton<TExecutor>();
		return services.BuildServiceProvider();
	}
}