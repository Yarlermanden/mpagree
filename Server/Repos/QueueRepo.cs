

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

	public async Task<List<PlayerEvent>> GetEvents(ISession<IEventContext> session, GetAndClearEventsRequest request)
	{
		var conn = await session.GetConnection();
		//should have arguments to check for certain game or something...

		var now = DateTime.UtcNow;

		var raw = await conn.QueryAsync<(long player_id, DateTimeOffset requested_time, PlayerEvent.EventOneofCase, byte[] data)>(
			@"
				with events as (
					delete from event e where e.queued_time < @now
					returning e.player_id, e.requested_time, e.event_type, e.data
				)
				select *
				from events
				order by requested_time
			",
			new
			{
				now = now,
			},
			session.CancellationToken
		);

		return raw.Select(x => new PlayerEvent
		{
			PlayerId = x.player_id,
			RequestedTime = Timestamp.FromDateTimeOffset(x.requested_time),
			Move = MovePlayerEvent.Parser.ParseFrom(x.data)
		}).ToList();

	}
}