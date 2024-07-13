using Npgsql;

namespace Server.Db;

public class Options
{
	public string ConnectionString = default!;
	public string Schema = default!;

	public static string BuildConnstr(string host, string user = "dp", string pass = "dp", string db = "dp", int poolSize = 75, int bufferSize = 65535, string applicationName = "dp")
	{
		return $"Host={host};" +
				$"Username={user};" +
				$"Password={pass};" +
				$"Database={db};" +
				$"Maximum Pool Size={poolSize};" +
				$"Application Name={applicationName};" +
				"Max Auto Prepare=100;" +
				"Command Timeout=100;" +
				//"No Reset On Close=true;" +
				$"Write Buffer Size={bufferSize};" +
				$"Read Buffer Size={bufferSize};" +
				$"ArrayNullabilityMode={ArrayNullabilityMode.Always}";
	}
}