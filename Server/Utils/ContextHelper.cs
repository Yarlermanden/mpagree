using Grpc.Core;
using Server.Db;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Server.Utils;

public static class ContextHelper
{
	public static ISession<TContext> GetSession<TContext>(this ServerCallContext context)
		where TContext : IContext
		=> context.GetHttpContext().GetSession<TContext>();

}