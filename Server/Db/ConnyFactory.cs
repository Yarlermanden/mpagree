using Npgsql;

namespace Server.Db;

public class ConnyFactory(Options opts)
{
	public Options Options { get; } = opts;

	public NpgsqlConnection Conny()
	{
		return new(Options.ConnectionString);
	}

	public async Task<NpgsqlConnection> ConnyOpen(CancellationToken cancellationToken)
	{
		var conn = Conny();
		await conn.OpenAsync(cancellationToken);
		return conn;
	}
}