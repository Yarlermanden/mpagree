using System.Diagnostics.CodeAnalysis;

namespace Server.Db;

public interface IContext { }

public interface IHttpRequestContext : IContext
{
	string Ip { get; }
}

public interface IEventContext : IContext
{
}


public interface IEventHttpRequestContext : IEventContext, IHttpRequestContext { }

public class EventHttpRequestContext : IEventHttpRequestContext
{
	public required string Ip { get; set; }
}

public class SystemContext : IContext
{
}

public class HttpRequestContext : IHttpRequestContext
{
	public required string Ip { get; set; }

}