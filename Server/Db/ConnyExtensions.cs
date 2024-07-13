using Dapper;
using System.Data;

namespace Server.Db;

public static class DbConnectionExtensions
{
	public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection conn, string sql, object? param, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		return conn.QueryAsync<T>(cmd);
	}

	public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection conn, string sql, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		return conn.QueryAsync<T>(sql, null, cancellationToken, commandTimeout, commandType);
	}

	public static async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection conn, string sql, object? param, Func<DbRow, T> factory, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		var res = await conn.QueryAsync(cmd);
		return res.Select(x => factory(new DbRow((IDictionary<string, object>)x)));
	}

	public static Task<T> QuerySingleAsync<T>(this IDbConnection conn, string sql, object? param, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		return conn.QuerySingleAsync<T>(cmd);
	}

	public static Task<T> QuerySingleAsync<T>(this IDbConnection conn, string sql, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		return conn.QuerySingleAsync<T>(sql, null, cancellationToken, commandTimeout, commandType);
	}

	public static async Task<T> QuerySingleAsync<T>(this IDbConnection conn, string sql, object? param, Func<DbRow, T> factory, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		var res = await conn.QuerySingleAsync(cmd);
		return factory(new DbRow((IDictionary<string, object>)res));
	}

	public static Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection conn, string sql, object? param, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		return conn.QuerySingleOrDefaultAsync<T>(cmd)!;
	}

	public static Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection conn, string sql, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		return conn.QuerySingleOrDefaultAsync<T>(sql, null, cancellationToken, commandTimeout, commandType);
	}

	public static async Task<T?> QuerySingleOrDefaultAsync<T>(this IDbConnection conn, string sql, object? param, Func<DbRow, T> factory, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		var res = await conn.QuerySingleOrDefaultAsync(cmd);
		return (res != null ? factory(new DbRow((IDictionary<string, object>)res!)) : default)!;
	}

	public static Task<int> ExecuteAsync(this IDbConnection conn, string sql, object? param, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		var cmd = new CommandDefinition(sql, param, cancellationToken: cancellationToken, commandTimeout: commandTimeout, commandType: commandType);
		return conn.ExecuteAsync(cmd);
	}

	public static Task<int> ExecuteAsync(this IDbConnection conn, string sql, CancellationToken cancellationToken, int? commandTimeout = null, CommandType? commandType = null)
	{
		return conn.ExecuteAsync(sql, null, cancellationToken, commandTimeout, commandType);
	}
}