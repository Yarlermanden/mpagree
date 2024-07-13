using Npgsql;

namespace Server.Db;

public enum SessionStatus
{
	Success,
	Failure
}

public interface ISession<out TContext>
	where TContext : IContext
{
	CancellationToken CancellationToken { get; }
	Task<NpgsqlConnection> GetConnection();
	Task<NpgsqlConnection> GetTransactionConnection();
	TContext Context { get; }
	SessionStatus Status { get; set; }
	ISession<TNextContext> Upgrade<TNextContext>(TNextContext next) where TNextContext : IContext;
}

public class ConnectionState(ConnyFactory connyFactory)
{
	public NpgsqlConnection? Connection { get; set; }
	public NpgsqlTransaction? Transaction { get; set; }


	public async Task<NpgsqlConnection> Get(CancellationToken cancellationToken)
	{
		Connection ??= await connyFactory.ConnyOpen(cancellationToken);

		return Connection;
	}

	public async Task<NpgsqlConnection> GetTransaction(CancellationToken cancellationToken)
	{
		var conn = await Get(cancellationToken);
		Transaction ??= await conn.BeginTransactionAsync(cancellationToken);

		return conn;
	}

	public async Task Commit(CancellationToken cancellationToken)
	{
		if (Transaction is { } trx)
		{
			await trx.CommitAsync(cancellationToken);
			await trx.DisposeAsync();
			Transaction = null;
		}

		await DisposeConnection();
	}

	public async Task Rollback(CancellationToken cancellationToken)
	{
		if (Transaction is { } trx)
		{
			await trx.RollbackAsync(cancellationToken);
			await trx.DisposeAsync();
			Transaction = null;
		}

		await DisposeConnection();
	}

	private async Task DisposeConnection()
	{
		if (Connection is { } conn)
		{
			await conn.DisposeAsync();
			Connection = null;
		}
	}
}

public class Session
{
	protected ConnectionState ConnectionState { get; set; }

	public CancellationToken CancellationToken { get; }

	public SessionStatus Status { get; set; } = SessionStatus.Success;

	public Session(ConnyFactory connyFactory, CancellationToken cancellationToken)
	{
		CancellationToken = cancellationToken;
		ConnectionState = new(connyFactory);
	}

	public Session(ConnectionState connectionState, CancellationToken cancellationToken)
	{
		ConnectionState = connectionState;
		CancellationToken = cancellationToken;
	}

	public async Task<NpgsqlConnection> GetConnection()
	{
		return await ConnectionState.Get(CancellationToken);
	}

	public async Task<NpgsqlConnection> GetTransactionConnection()
	{
		return await ConnectionState.GetTransaction(CancellationToken);
	}

	public async Task Commit()
	{
		await ConnectionState.Commit(CancellationToken);
	}

	public async Task Rollback(CancellationToken ct)
	{
		await ConnectionState.Rollback(ct);
	}

	public static Session<T> Create<T>(ConnyFactory connyFactory, T context, CancellationToken cancellationToken)
	where T : IContext
	{
		return new Session<T>(connyFactory, context, cancellationToken);
	}
}

public class Session<TContext> : Session, ISession<TContext>
	where TContext : IContext
{
	public TContext Context { get; }

	public Session(ConnyFactory connyFactory, TContext context, CancellationToken cancellationToken) : base(connyFactory, cancellationToken)
	{
		Context = context;
	}
	public Session(ConnectionState connectionState, TContext context, CancellationToken cancellationToken) : base(connectionState, cancellationToken)
	{
		Context = context;
	}

	public ISession<TNextContext> Upgrade<TNextContext>(TNextContext next)
		where TNextContext : IContext
	{
		return new Session<TNextContext>(ConnectionState, next, CancellationToken);
	}
}