using Grpc.Core;
using Server.Db;
using Server.Repos;
using Server.Utils;
using Service;
using static Engine.Engine;
using static Service.QueueService;

namespace Server.Grpc;

//public class QueueGrpcService(EngineClient client, QueueRepo queueRepo) : QueueServiceBase
public class QueueGrpcService(QueueRepo queueRepo) : QueueServiceBase
{
	public override async Task<ConnectPlayerResponse> ConnectPlayer(ConnectPlayerRequest request, ServerCallContext context)
	{
		var playerId = await queueRepo.ConnectPlayer(context.GetSession<IEventContext>(), request);
		// var req = new Engine.ConnectPlayerRequest
		// {
		// 	IpAddress = request.IpAddress,
		// 	Username = request.Username,
		// };
		// var resp = await client.ConnectPlayerAsync(req);
		return new ConnectPlayerResponse() { PlayerId = playerId };
	}

	public override async Task<QueueEventResponse> QueueEvent(QueueEventRequest request, ServerCallContext context)
	{
		await queueRepo.AddEvent(context.GetSession<IEventContext>(), request);
		return new QueueEventResponse { PlayerId = request.Event.PlayerId };
	}
}