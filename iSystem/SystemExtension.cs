// ReSharper disable UnusedMember.Global

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using static System.Math;

namespace iSystem;

/// <summary>
///     The iSysExtension class.
/// </summary>
public static class SystemExtension
{
	[Obsolete("Obsolete")]
	public static T DeepCopy<T>(this T srcObject)
	{
		if (srcObject == null) throw new ArgumentNullException(nameof(srcObject));

		using var ms = new MemoryStream();
		var formatter = new BinaryFormatter();
		formatter.Serialize(ms, srcObject);
		ms.Position = 0;
		return (T)formatter.Deserialize(ms);
	}

	/// <summary>
	///     To get category of the specified enumeration member.
	/// </summary>
	/// <param name="value">
	///     The value.
	/// </param>
	/// <returns>
	///     The <see cref="string" />.
	/// </returns>
	public static string GetCategory(this Enum value)
	{
		var runtimeField = value.GetType().GetRuntimeField(value.ToString());
		if (runtimeField is null) return string.Empty;

		return runtimeField
			.GetCustomAttributes<CategoryAttribute>()
			.FirstOrDefault()?.Category ?? string.Empty;
	}

	public static MethodBase? GetMethodBase<T>([DisallowNull] this T value)
	{
		if (value == null) throw new ArgumentNullException(nameof(value));
		return new StackTrace().GetFrame(1)?.GetMethod();
	}

	/// <summary>
	///     Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer
	///     array.
	/// </summary>
	/// <param name="hexString">The string to convert.</param>
	/// <returns>An array of 8-bit unsigned integers that is equivalent to hex string.</returns>
	public static byte[] HexStringToByteArray(this string hexString)
	{
		hexString = hexString.RemoveAllWhitespace();
		return Enumerable.Range(0, hexString.Length / 2)
			.Select(x => Convert.ToByte(hexString.Substring(x * 2, 2), 16)).ToArray();
	}

	public static byte[]?[] HexStringToByteArray(this string hexString, int chunkSizeInByte)
	{
		hexString = hexString.RemoveAllWhitespace();
		chunkSizeInByte *= 2;

		if (hexString.Length <= chunkSizeInByte)
		{
			byte[]?[] array = new byte[1][];
			array[0] = hexString.HexStringToByteArray();
			return array;
		}

		var count = (hexString.Length + chunkSizeInByte - 1) / chunkSizeInByte;
		var strings = Enumerable.Range(0, count).Select(i =>
			hexString.Substring(i * chunkSizeInByte, Min(hexString.Length - i * chunkSizeInByte, chunkSizeInByte)));

		return strings.Select(s => s.HexStringToByteArray()).ToArray();
	}

	public static bool IsFileLocked(this FileInfo file)
	{
		try
		{
			using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
			stream.Close();
		}
		catch (IOException)
		{
			//the file is unavailable because it is:
			//still being written to
			//or being processed by another thread
			//or does not exist (has already been processed)
			return true;
		}

		//file is not locked
		return false;
	}

	public static bool IsIndexExisting<T>(this T[] array, int index)
	{
		var exist = true;

		exist &= index >= 0 && index < array.Length;

		return exist;
	}

	public static bool IsIndexExisting<T>(this T[][] array, int index1, int index2)
	{
		var exist = true;

		exist &= index1 >= 0 && index1 < array.Length;
		exist &= index2 >= 0 && index2 < array[0].Length;

		return exist;
	}

	public static bool IsIndexExisting<T>(this T[][][] array, int index1, int index2, int index3)
	{
		var exist = true;

		exist &= index1 >= 0 && index1 < array.Length;
		exist &= index2 >= 0 && index2 < array[0].Length;
		exist &= index3 >= 0 && index3 < array[0][0].Length;

		return exist;
	}

	public static bool IsIndexExisting<T>(this T[][][][] array, int index1, int index2, int index3, int index4)
	{
		var exist = true;

		exist &= index1 >= 0 && index1 < array.Length;
		exist &= index2 >= 0 && index2 < array[0].Length;
		exist &= index3 >= 0 && index3 < array[0][0].Length;
		exist &= index4 >= 0 && index4 < array[0][0][0].Length;

		return exist;
	}

	public static string? RemoveAllNewLine(this string? sourceString)
	{
		var whitespaceRegex = new Regex(@"\n|\r");
		return sourceString is null ? null : whitespaceRegex.Replace(sourceString, string.Empty);
	}

	/// <summary>
	///     Removes all the white-space characters from the current string.
	/// </summary>
	/// <param name="sourceString">The string to convert.</param>
	/// <returns>The string that remains after all white-space characters are removed from the current string.</returns>
	public static string RemoveAllWhitespace(this string sourceString)
	{
		return new string(sourceString.ToCharArray()
			.Where(c => !char.IsWhiteSpace(c))
			.ToArray());
	}

	///// <summary>
	/////     Removes all the white-space characters from the current string.
	///// </summary>
	///// <param name="sourceString">The string to convert.</param>
	///// <returns>The string that remains after all white-space characters are removed from the current string.</returns>
	//public static string RemoveAllWhitespace(this string sourceString)
	//{
	//    var whitespaceRegex = new Regex(@"\s+");
	//    return sourceString is null ? null : whitespaceRegex.Replace(sourceString, string.Empty);
	//}

	public static string? ReplaceSpecifiedStringWithToken(this string? sourceString, string specifiedString,
		string? token)
	{
		if (sourceString is null) return null;
		if (string.IsNullOrEmpty(specifiedString)) return sourceString;

		var regex = new Regex(specifiedString);
		return token is null ? sourceString : regex.Replace(sourceString, token);
	}

	public static string? ReplaceWhitespaceWithToken(this string? sourceString, string? token)
	{
		if (sourceString is null) return null;

		var regex = new Regex(@"\s+");
		return token is null ? sourceString : regex.Replace(sourceString, token);
	}

	public static Bitmap ResizeBitmap(this Bitmap bmp, int width, int height)
	{
		var result = new Bitmap(width, height);
		using var g = Graphics.FromImage(result);
		g.DrawImage(bmp, 0, 0, width, height);

		return result;
	}

	public static Bitmap ResizeBitmap(this Bitmap bmp, Size size)
	{
		var result = new Bitmap(size.Width, size.Height);
		using var g = Graphics.FromImage(result);
		g.DrawImage(bmp, 0, 0, size.Width, size.Height);

		return result;
	}

	public static double RoundApproximate(this double dbl, int digits, double margin,
		MidpointRounding mode)
	{
		var fraction = dbl * Pow(10, digits);
		var value = Truncate(fraction);
		fraction -= value;
		if (fraction == 0)
			return dbl;

		var tolerance = margin * dbl;

		// Determine whether this is a midpoint value.
		if ((fraction >= .5 - tolerance) & (fraction <= .5 + tolerance))
		{
			if (mode == MidpointRounding.AwayFromZero)
				return (value + 1) / Pow(10, digits);
			if (value % 2 != 0)
				return (value + 1) / Pow(10, digits);
			return value / Pow(10, digits);
		}

		// Any remaining fractional value greater than .5 is not a midpoint value.
		if (fraction > .5)
			return (value + 1) / Pow(10, digits);
		return value / Pow(10, digits);
	}

	public static void ShowMethodInfo<T>(this T instance)
	{
		if (instance == null) return;

		var method = new StackTrace().GetFrame(1)?.GetMethod();

		if (method == null)
			return;

		var description =
			$"■Class Name : {method.ReflectedType?.Name}{Environment.NewLine}" +
			$" Method Name : {method.Name}{Environment.NewLine}";

		Debug.WriteLine(description);
	}

	public static void ShowMethodInfoAppendDescription<T>(this T instance, string description)
	{
		if (instance == null) return;

		var method = new StackTrace().GetFrame(1)?.GetMethod();

		if (method == null)
			return;

		description =
			$"■Class Name : {method.ReflectedType?.Name}{Environment.NewLine}" +
			$" Method Name : {method.Name}{Environment.NewLine}" +
			" Description : " + description;

		Debug.WriteLine(description);
	}

	/// <summary>
	///     Split a string array.
	/// </summary>
	/// <param name="source">Source string.</param>
	/// <param name="chunkSize"></param>
	/// <returns></returns>
	public static IEnumerable<string?> Split(this string? source, int chunkSize)
	{
		if (source is null) yield return null;

		if (string.IsNullOrEmpty(source) || chunkSize < 1) yield return null;

		if (source is null) yield break;
		for (var i = 0; i < source.Length; i += chunkSize)
			yield return source.Substring(i, Min(chunkSize, source.Length - i));
	}

	public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IComparable
	{
		if (string.IsNullOrEmpty(value)) return defaultValue;

		return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
	}

	public static int ToInt(this bool value)
	{
		return value ? 1 : 0;
	}
}