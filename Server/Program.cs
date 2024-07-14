using CommandLine;
using Server.Commands;

//AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, __) => cts.Cancel();

var parser = new Parser(with =>
{
	with.CaseInsensitiveEnumValues = true;
	with.AutoHelp = true;
	with.AutoVersion = true;
	with.HelpWriter = Parser.Default.Settings.HelpWriter;
});

var task = parser.ParseArguments<ServerCommand.Options, TestClientCommand.Options>(args).MapResult(
	(ServerCommand.Options options) => ServerCommand.Run(options, cts.Token),
	(TestClientCommand.Options options) => TestClientCommand.Run(options, cts.Token),
	_ => Task.FromResult(1));

Environment.ExitCode = await task;