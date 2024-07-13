namespace Server.Utils;

public static class EnumHelper
{
	public static bool IsDefined<T>(T val)
		where T : struct, IConvertible
	{
		return Enum.IsDefined(typeof(T), val);
	}

	public static T? DefinedOrNull<T>(IConvertible? val, Type? underlyingEnumType = null)
		where T : struct, IConvertible
	{
		if (val == null)
		{
			return default;
		}

		if (val is string s)
		{
			if (Enum.TryParse(s, true, out T parsed) && IsDefined(parsed))
			{
				return parsed;
			}

			return default;
		}

		var preparedValue = underlyingEnumType is not { IsPrimitive: true }
			? Convert.ToInt32(val)
			: Convert.ChangeType(val, underlyingEnumType);
		var converted = (T)preparedValue;
		return IsDefined(converted) ? converted : default(T?);
	}
}