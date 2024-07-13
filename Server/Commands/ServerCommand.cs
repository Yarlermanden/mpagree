using CommandLine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Db;
using Server.Grpc;

namespace Server.Commands;

public class ServerCommand : WorkerCommand<ServerCommand.Executor, ServerCommand.Options>
{
	[Verb("server", HelpText = "Run server")]
	public class Options : Command.BaseOptions
	{
		[Option("port", Default = 5002, HelpText = "Port number to listen on.")]
		public int Port { get; set; }

		[Option("agreed-host", Default = "localhost:5003", HelpText = "Port number to agreed host")]
		public string AgreedHostUrl { get; set; } = default!;
	}

	public class Executor() : IWorkerExecutor<Options>
	{
		public async Task<int> RunInner(Options options, CancellationToken cancellationToken)
		{
			var logger = options.BuildLogger();
			var builder = WebApplication.CreateBuilder();
			builder.WebHost.UseKestrel(kestrelOptions =>
			{
				kestrelOptions.ListenAnyIP(options.Port,
					listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
			});
			options.ConfigureServices(builder.Services);

			//builder.Host.UseSerilog();
			builder.Services.AddGrpc(options => options.MaxReceiveMessageSize = 16 * 1024 * 1024);

			builder.Services.AddGrpcClient<Engine.Engine.EngineClient>(o => o.Address = new Uri(options.AgreedHostUrl));

			var app = builder.Build();

			app.MapGrpcService<QueueGrpcService>();

			app.UseSessionLifecycle();

			if (options.Init || options.Env == Env.Local)
			{
				var init = app.Services.GetRequiredService<Init>();
				init.Initialize();
			}

			await app.RunAsync(cancellationToken);
			return 0;
		}
	}
}