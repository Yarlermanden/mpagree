using Grpc.Core;
using Server.Db;
using Server.Repos;
using Server.Utils;
using Service;
using static Service.Queue;

namespace Server.Grpc;

public class QueueGrpcService(QueueRepo queueRepo) : QueueBase
{
	public override async Task<ConnectPlayerResponse> ConnectPlayer(ConnectPlayerRequest request, ServerCallContext context)
	{
		var playerId = await queueRepo.ConnectPlayer(context.GetSession<IEventContext>(), request);
		return new ConnectPlayerResponse() { PlayerId = playerId };
	}

	public override async Task<QueueEventResponse> QueueEvent(QueueEventRequest request, ServerCallContext context)
	{
		await queueRepo.AddEvent(context.GetSession<IEventContext>(), request);
		return new QueueEventResponse { PlayerId = request.Event.PlayerId };
	}

	public override async Task<GetAndClearEventsResponse> GetAndClearEvents(Google.Protobuf.WellKnownTypes.Empty empty, ServerCallContext context)
	{
		var events = await queueRepo.GetEvents(context.GetSession<IEventContext>());
		return new GetAndClearEventsResponse { Events = { events } };
	}
}