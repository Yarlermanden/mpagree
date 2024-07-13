using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;
using System.Diagnostics;
using Npgsql;
using SimpleMigrations.Console;
using ILogger = Serilog.ILogger;
using Server.Repos;

namespace Server.Db;

public class Init(ILogger logger, QueueRepo queueRepo, Options opts)
{
	public void Initialize(bool logMigrationToConsole = true)
	{
		ForMigrator((conn, m) =>
		{
			if (!logMigrationToConsole)
			{
				m.Logger = null;
			}
			m.Load();
			m.MigrateToLatest();

			// SeedDatabase(conn);
		}, logMigrationToConsole);
	}

	public void Reset(int version, bool logMigrationToConsole = true)
	{
		var sw = Stopwatch.StartNew();

		ForMigrator((_, m) =>
		{
			m.Load();
			m.MigrateTo(version);
		}, logMigrationToConsole);

		if (logMigrationToConsole)
		{
			logger.Information("Reset done in {elapsedMs}", sw.ElapsedMilliseconds);
		}
	}

	private void ForMigrator(Action<NpgsqlConnection, SimpleMigrator> act, bool logMigrationToConsole)
	{
		if (logMigrationToConsole)
		{
			logger.Information("Building migrator");
		}
		var sw = Stopwatch.StartNew();

		var assm = GetType().Assembly;
		using (var conn = new NpgsqlConnection(opts.ConnectionString))
		{
			var provider = new PostgresqlDatabaseProvider(conn)
			{
				SchemaName = opts.Schema,
				CreateSchema = false
			};

			var migrator = new SimpleMigrator(assm, provider, logMigrationToConsole ? new ConsoleLogger() : null);

			act(conn, migrator);
		}

		if (logMigrationToConsole)
		{
			logger.Information("Migrator done in {elapsedMs}", sw.ElapsedMilliseconds);
		}
	}

	// private void SeedDatabase(NpgsqlConnection conn)
	// {
	// 	PolicyHelper.SeedDatabase(conn, queueRepo);
	// }
}