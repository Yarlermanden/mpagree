using Server.Db;

namespace Server.Models;

public class EventContext : IEventContext, IHttpRequestContext
{
	public required string Ip { get; set; }
}