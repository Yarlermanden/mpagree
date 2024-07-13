

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Server.Db;
using Service;

namespace Server.Repos;

//public class QueueRepo : IQueueRepo
public class QueueRepo : IRepo
{
	public async Task<long> ConnectPlayer(ISession<IEventContext> session, ConnectPlayerRequest request)
	{
		var conn = await session.GetConnection();

		var id = await conn.QuerySingleAsync<long>(
			@"
				insert into player (username, ip)
				values (@username, @ip)
				returning id
			",
			new
			{
				username = request.Username,
				ip = request.IpAddress,
			},
			session.CancellationToken
		);
		return id;
	}

	public async Task AddEvent(ISession<IEventContext> session, QueueEventRequest request)
	{
		var conn = await session.GetConnection();

		var evt = request.Event;

		await conn.ExecuteAsync(
			@"
				insert into event (player_id, event_type, queued_time, requested_time, data)
				values (@player_id, @event_type, current_timestamp, @requested_time, @data)
			",
			new
			{
				player_id = evt.PlayerId,
				event_type = evt.EventCase,
				queued_time = DateTime.UtcNow,
				requested_time = evt.RequestedTime.ToDateTime(),
				data = evt.Move.ToByteArray(),
			},
			session.CancellationToken
		);

	}

	//get events
}