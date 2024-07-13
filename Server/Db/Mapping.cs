using System.Globalization;

namespace Server.Db;

public class DbRow
{
	private readonly IDictionary<string, object> _data;

	public DbRow(IDictionary<string, object> data)
	{
		_data = new Dictionary<string, object>(data, StringComparer.OrdinalIgnoreCase);
	}

	public bool? Bool(string columnName)
	{
		return ObjectMapper.Bool(Value(columnName));
	}

	public string? String(string columnName)
	{
		return ObjectMapper.String(Value(columnName));
	}

	public int? Int(string columnName)
	{
		return ObjectMapper.Int(Value(columnName));
	}

	public long? Long(string columnName)
	{
		return ObjectMapper.Long(Value(columnName));
	}

	public double? Double(string columnName)
	{
		return ObjectMapper.Double(Value(columnName));
	}

	public DateTime? DateTime(string columnName)
	{
		return ObjectMapper.DateTime(Value(columnName));
	}

	public TEnum? Enum<TEnum>(string columnName)
		where TEnum : struct, Enum, IConvertible
	{
		return ObjectMapper.Enum<TEnum>(Value(columnName));
	}

	public object? Value(string columnName)
	{
		return _data.TryGetValue(columnName, out var value) ? value : null;
	}
}

public static class ObjectMapper
{
	public static TEnum Infer<TEnum>(this TEnum e)
		where TEnum : struct, Enum, IConvertible
	{
		return default;
	}

	public static bool? Bool(object? val)
	{
		if (val is bool b)
		{
			return b;
		}

		var l = Long(val);
		if (l != null)
		{
			return l > 0;
		}

		var s = String(val);
		if (s != null)
		{
			if (s.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			if (s.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}

		return null;
	}

	public static string? String(object? val)
	{
		if (val is string s)
		{
			return s;
		}

		return val?.ToString();
	}

	public static int? Int(object? val)
	{
		if (val is int i || (val is string s && int.TryParse(s, out i)))
		{
			return i;
		}

		return null;
	}

	public static long? Long(object? val)
	{
		if (val is long l || (val is string s && long.TryParse(s, out l)))
		{
			return l;
		}

		return null;
	}

	public static double? Double(object? val)
	{
		if (val is double d || (val is string s && double.TryParse(s, out d)))
		{
			return d;
		}

		return null;
	}

	public static DateTime? DateTime(object? val)
	{
		if (val is DateTime dt || (val is string s && (System.DateTime.TryParseExact(s, "dd-MM-yyyy HH:mm:ss", null, DateTimeStyles.None, out dt) || System.DateTime.TryParse(s, out dt))))
		{
			return dt;
		}

		return null;
	}

	public static TEnum? Enum<TEnum>(object? val)
		where TEnum : struct, Enum, IConvertible
	{
		if (val == null)
		{
			return null;
		}

		if (val is string s)
		{
			return System.Enum.TryParse(s, true, out TEnum e) ? e : default;
		}

		int? numeric = val switch
		{
			ushort ush => ush,
			short sh => sh,
			int i => i,
			_ => null
		};

		if (numeric != null)
		{
			return System.Enum.IsDefined(typeof(TEnum), numeric.Value) ? (TEnum)(object)numeric.Value : default;
		}

		return default;
	}
}