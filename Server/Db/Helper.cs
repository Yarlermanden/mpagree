using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;
using Server.Utils;

namespace Server.Db;

public static class Helper
{
	public static ISession<TContext> GetSession<TContext>(this HttpContext context)
		where TContext : IContext
	{
		if (context.Items.TryGetValue(SessionHttpKey, out var ctx) && ctx is ISession<TContext> c)
		{
			return c;
		}

		throw new Exception($"Excepted {typeof(ISession<TContext>)} but found {(ctx is null ? "no context" : ctx?.GetType())}");
	}

	public static TEnum[] GetEnumValues<TEnum>(this HttpContext context, string claim)
		where TEnum : struct, IConvertible
	{
		return (context?.User?.Claims ?? []).Where(x => x.Type == claim)
			.Select(x => int.TryParse(x.Value, out var val) ? EnumHelper.DefinedOrNull<TEnum>(val) : null)
			.Where(x => x is not null)
			.Cast<TEnum>()
			.ToArray();
	}

	private static IContext CreateContext(HttpContext httpCtx)
	{
		var ip = httpCtx.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
		return new EventContext
		{
			Ip = ip,
		};
	}

	private static Session CreateSession(ConnyFactory connyFactory, HttpContext httpCtx)
	{
		var ct = httpCtx.RequestAborted;
		return CreateContext(httpCtx) switch
		{
			EventContext x => Session.Create(connyFactory, x, ct),
			HttpRequestContext x => Session.Create(connyFactory, x, ct),
			_ => throw new NotImplementedException(),
		};
	}

	public static IApplicationBuilder UseSessionLifecycle(this WebApplication app)
	{
		var connyFactory = app.Services.GetRequiredService<ConnyFactory>();

		return app.Use(async (context, next) =>
			{
				var sess = CreateSession(connyFactory, context);
				context.Items[SessionHttpKey] = sess;

				try
				{
					await next();
				}
				catch (Exception)
				{
					await sess.Rollback(CancellationToken.None);
					throw;
				}

				var grpcFailure = context.Response.Headers.GrpcStatus is { } grpcs
					&& int.TryParse(grpcs, out var status)
					&& status is not 0; // Grpc status code for OK is 0

				var httpFailure = context.Response.StatusCode > 399;

				if (grpcFailure || httpFailure || sess.Status != SessionStatus.Success)
				{
					await sess.Rollback(CancellationToken.None);
				}
				else
				{
					await sess.Commit();
				}

				context.Items.Remove(SessionHttpKey);
			});
	}

	public static IApplicationBuilder UseSessionLifecycleUpgrade(this WebApplication app)
	{
		return app.Use(async (context, next) =>
		{
			if (context.Items[SessionHttpKey] is ISession<IContext> sess)
			{
				context.Items[SessionHttpKey] = CreateContext(context) switch
				{
					EventContext x => sess.Upgrade(x),
					HttpRequestContext x => sess.Upgrade(x),
					_ => throw new NotImplementedException(),
				};
			};
			await next();
		});
	}

	private const string SessionHttpKey = "Sheltr-Session";
}