using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Service;

namespace Server.Commands;

public class ClientCommand : WorkerCommand<ClientCommand.Executor, ClientCommand.Options>
{
	[Verb("client", HelpText = "Run client")]
	public class Options : Command.BaseOptions
	{
		[Option("svc-host", Default = "http://localhost:5002", HelpText = "Port number to service")]
		public string SvcHost { get; set; }

		public override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			services.AddGrpcClient<QueueService.QueueServiceClient>(o => o.Address = new Uri(SvcHost));
		}
	}


	public class Executor() : IWorkerExecutor<Options>
	{


		public async Task<int> RunInner(Options options, CancellationToken cancellationToken)
		{
			var services = new ServiceCollection();
			options.ConfigureServices(services);

			var serviceProvider = services.BuildServiceProvider();

			var client = serviceProvider.GetRequiredService<QueueService.QueueServiceClient>();
			var response = await client.ConnectPlayerAsync(new ConnectPlayerRequest
			{
				Username = "player1",
				IpAddress = "127.0.0.1",
			});

			for (var i = 0; i < 10; i++)
			{
				Console.WriteLine("Enqueueing player update event");
				var resp = await client.QueueEventAsync(new QueueEventRequest
				{
					Event = new PlayerEvent
					{
						PlayerId = response.PlayerId,
						RequestedTime = Timestamp.FromDateTime(DateTime.UtcNow),
						Move = new MovePlayerEvent
						{
							DeltaX = -1.0f,
							DeltaY = 0,
						}
					}
				});

				if (resp.PlayerId != response.PlayerId)
				{
					Console.WriteLine("PlayerId didn't match");
				}
			}
			return 0;
		}
	}
}