using CommandLine;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service;

namespace Server.Commands;

public class TestClientCommand : WorkerCommand<TestClientCommand.Executor, TestClientCommand.Options>
{
	[Verb("test-client", HelpText = "Run client")]
	public class Options : Command.BaseOptions
	{
		[Option("svc-host", Default = "http://localhost:5002", HelpText = "Port number to service")]
		public string SvcHost { get; set; } = default!;

		[Option("clear-events", Default = false, HelpText = "Should clear events after")]
		public bool ClearEvents { get; set; } = default!;

		public override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			services.AddGrpcClient<Queue.QueueClient>(o => o.Address = new Uri(SvcHost));
		}
	}


	public class Executor() : IWorkerExecutor<Options>
	{


		public async Task<int> RunInner(Options options, CancellationToken cancellationToken)
		{
			var services = new ServiceCollection();
			options.ConfigureServices(services);

			var serviceProvider = services.BuildServiceProvider();

			var client = serviceProvider.GetRequiredService<Queue.QueueClient>();
			var response = await client.ConnectPlayerAsync(new ConnectPlayerRequest
			{
				Username = "player1",
				IpAddress = "127.0.0.1",
			}, cancellationToken: cancellationToken);

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
				}, cancellationToken: cancellationToken);

				if (resp.PlayerId != response.PlayerId)
				{
					Console.WriteLine("PlayerId didn't match");
				}
			}

			if (options.ClearEvents)
			{
				var events = await client.GetAndClearEventsAsync(new GetAndClearEventsRequest(), cancellationToken: cancellationToken);

				Console.WriteLine($"Found and cleared {events.Events.Count()} events");
			}
			return 0;
		}
	}
}