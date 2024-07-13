using System.Net;
using CommandLine;
using Destructurama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Server.Db;
using Server.Repos;
using Server.Services;
using Server.Utils;



namespace Server.Commands;

public class Command
{
	public class BaseOptions
	{
		[Option("log-level", Default = LogEventLevel.Information)]
		public LogEventLevel LogLevel { get; set; }

		[Option("log-json", HelpText = "Toggle JSON logging to stdout instead of human-friendly text")]
		public bool LogJson { get; set; }

		[Option("init", HelpText = "Run DB migrations")]
		public virtual bool Init { get; set; }

		[Option("env", HelpText = "Environment", Default = Env.Local)]
		public Env Env { get; set; }

		[Option("pg-host", Default = "localhost", HelpText = "Which Postgres host to connect to")]
		public virtual string PostgresHost { get; set; } = default!;

		[Option("pg-db", Default = "dp", HelpText = "Which Postgres Database to use")]
		public string PostgresDb { get; set; } = default!;

		[Option("pg-schema", Default = "mpagree", HelpText = "Which Postgres Schema to use")]
		public string PostgresSchema { get; set; } = default!;

		[Option("pg-user", Default = "mpagree", HelpText = "Postgres username")]
		public string PostgresUser { get; set; } = default!;

		[Option("pg-pass", Default = "mpagree", HelpText = "Postgres password")]
		public string PostgresPass { get; set; } = default!;

		[Option("pg-poolsize", Default = 500, HelpText = "Max size of the connection pool")]
		public int PostgresMaxPoolSize { get; set; }

		[Option("pg-buffersize", Default = 65535, HelpText = "Buffer size for read and write buffers")]
		public int PostgresBufferSize { get; set; }

		public ILogger BuildLogger()
		{
			var configuration = new LoggerConfiguration()
				.Destructure.UsingAttributes()
				.MinimumLevel.Is(LogLevel)
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
				.MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
				.Enrich.WithSpan()
				.Enrich.FromLogContext();

			// if (LogJson)
			// {
			// 	configuration = configuration.WriteTo.Console(new RenderedCompactJsonFormatter(),
			// 	  standardErrorFromLevel: LogEventLevel.Error);
			// }
			// else
			// {
			// 	configuration = configuration
			// 	  .WriteTo.Console(
			// 		outputTemplate:
			// 		"{Timestamp:HH:mm:ss} [{Level:u3}] {Message:I}{NewLine}{Exception}",
			// 		theme: AnsiConsoleTheme.Code,
			// 		applyThemeToRedirectedOutput: true);
			// }

			Log.Logger = configuration.CreateLogger();
			return Log.Logger;
		}

		public Options BuildDbOptions()
		{
			return new()
			{
				Schema = PostgresSchema,
				ConnectionString = Options.BuildConnstr(
				PostgresHost,
				PostgresUser,
				PostgresPass,
				PostgresDb,
				PostgresMaxPoolSize,
				PostgresBufferSize
				)
			};
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(new BaseOptions());
			services.AddSingleton(BuildLogger());
			services.AddSingleton(BuildDbOptions());
			//Temporary code for being able to seed local and stage db with policies
			services.AddSingleton(this);
			services.AddSingleton<Init>();
			services.AddSingleton<ConnyFactory>();
			services.AddTypesImplementing<IRepo>();
			services.AddTypesImplementing<IService>();
			services.Configure<ForwardedHeadersOptions>(fho =>
			{
				fho.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
				fho.KnownNetworks.Clear();
				fho.KnownProxies.Clear();
				fho.ForwardedProtoHeaderName = "x-forwarded-scheme";
				fho.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
			});
		}
	}
}

public enum Env
{
	Local,
	Stage,
	Prod
}