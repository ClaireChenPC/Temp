// ReSharper disable UnusedMember.Global

namespace iSystem.Core;

/// <summary>
///     Class providing useful bit operation methods.
/// </summary>
/// <threadsafety static="true" instance="true" />
public static class Bits
{
	/// <summary>
	///     Returns the ceiling power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the ceiling power of 2.</param>
	/// <returns>The ceiling power of 2 for the <paramref name="value" />.</returns>
	public static byte CeilingPowerOf2(byte value)
	{
		var ret = (byte)(value - 1);
		ret |= (byte)(ret >> 1);
		ret |= (byte)(ret >> 2);
		ret |= (byte)(ret >> 4);
		return (byte)(ret + 1);
	}

	/// <summary>
	///     Returns the ceiling power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the ceiling power of 2.</param>
	/// <returns>The ceiling power of 2 for the <paramref name="value" />.</returns>
	public static ushort CeilingPowerOf2(ushort value)
	{
		var ret = (ushort)(value - 1);
		ret |= (ushort)(ret >> 1);
		ret |= (ushort)(ret >> 2);
		ret |= (ushort)(ret >> 4);
		ret |= (ushort)(ret >> 8);
		return (ushort)(ret + 1);
	}

	/// <summary>
	///     Returns the ceiling power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the ceiling power of 2.</param>
	/// <returns>The ceiling power of 2 for the <paramref name="value" />.</returns>
	public static uint CeilingPowerOf2(uint value)
	{
		var ret = value - 1;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		return ret + 1;
	}

	/// <summary>
	///     Returns the ceiling power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the ceiling power of 2.</param>
	/// <returns>The ceiling power of 2 for the <paramref name="value" />.</returns>
	public static ulong CeilingPowerOf2(ulong value)
	{
		var ret = value - 1;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		ret |= ret >> 32;
		return ret + 1;
	}

	/// <summary>
	///     Returns the floor power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the floor power of 2.</param>
	/// <returns>The floor power of 2 for the <paramref name="value" />.</returns>
	public static byte FloorPowerOf2(byte value)
	{
		return (byte)(NextLargestPowerOf2(value) >> 1);
	}

	/// <summary>
	///     Returns the floor power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the floor power of 2.</param>
	/// <returns>The floor power of 2 for the <paramref name="value" />.</returns>
	public static ushort FloorPowerOf2(ushort value)
	{
		return (ushort)(NextLargestPowerOf2(value) >> 1);
	}

	/// <summary>
	///     Returns the floor power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the floor power of 2.</param>
	/// <returns>The floor power of 2 for the <paramref name="value" />.</returns>
	public static uint FloorPowerOf2(uint value)
	{
		return NextLargestPowerOf2(value) >> 1;
	}

	/// <summary>
	///     Returns the floor power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the floor power of 2.</param>
	/// <returns>The floor power of 2 for the <paramref name="value" />.</returns>
	public static ulong FloorPowerOf2(ulong value)
	{
		return NextLargestPowerOf2(value) >> 1;
	}

	/// <summary>
	///     Indicates whether a given value is a power of 2.
	/// </summary>
	/// <param name="value">The value for which to find if it is a power of 2.</param>
	/// <returns><b>true</b> if <paramref name="value" /> is a power of 2, <b>false</b> otherwise.</returns>
	public static bool IsPowerOf2(uint value)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalse
		return value == 1 && (value & (value - 1)) != 0;
	}

	/// <summary>
	///     Indicates whether a given value is a power of 2.
	/// </summary>
	/// <param name="value">The value for which to find if it is a power of 2.</param>
	/// <returns><b>true</b> if <paramref name="value" /> is a power of 2, <b>false</b> otherwise.</returns>
	public static bool IsPowerOf2(ulong value)
	{
		// ReSharper disable once ConditionIsAlwaysTrueOrFalse
		return value == 1 && (value & (value - 1)) != 0;
	}

	/// <summary>
	///     Returns the least significant 1 bit (lowest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the least significant 1 bit.</param>
	/// <returns>The value with only the least significant 1 bit set, or zero (0).</returns>
	public static byte LeastSignificantOneBit(byte value)
	{
		return (byte)(value ^ (value & (value - 1)));
	}

	/// <summary>
	///     Returns the least significant 1 bit (lowest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the least significant 1 bit.</param>
	/// <returns>The value with only the least significant 1 bit set, or zero (0).</returns>
	public static ushort LeastSignificantOneBit(ushort value)
	{
		return (ushort)(value ^ (value & (value - 1)));
	}

	/// <summary>
	///     Returns the least significant 1 bit (lowest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the least significant 1 bit.</param>
	/// <returns>The value with only the least significant 1 bit set, or zero (0).</returns>
	public static uint LeastSignificantOneBit(uint value)
	{
		return value ^ (value & (value - 1));
	}

	/// <summary>
	///     Returns the least significant 1 bit (lowest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the least significant 1 bit.</param>
	/// <returns>The value with only the least significant 1 bit set, or zero (0).</returns>
	public static ulong LeastSignificantOneBit(ulong value)
	{
		return value ^ (value & (value - 1));
	}

	/// <summary>
	///     Returns the base 2 logarithm for a given number. This is the number of filled bits (bits right of the last leading
	///     zero).
	/// </summary>
	/// <param name="value">The value for which to give the base 2 logarithm.</param>
	/// <returns>The base 2 logarithm for the <paramref name="value" />. Zero (0) for <paramref name="value" />==zero (0).</returns>
	public static int Log2(byte value)
	{
		var ret = value;
		ret |= (byte)(ret >> 1);
		ret |= (byte)(ret >> 2);
		ret |= (byte)(ret >> 4);
		return Ones((byte)(ret >> 1));
	}

	/// <summary>
	///     Returns the base 2 logarithm for a given number. This is the number of filled bits (bits right of the last leading
	///     zero).
	/// </summary>
	/// <param name="value">The value for which to give the base 2 logarithm.</param>
	/// <returns>The base 2 logarithm for the <paramref name="value" />. Zero (0) for <paramref name="value" />==zero (0).</returns>
	public static int Log2(ushort value)
	{
		var ret = value;
		ret |= (ushort)(ret >> 1);
		ret |= (ushort)(ret >> 2);
		ret |= (ushort)(ret >> 4);
		ret |= (ushort)(ret >> 8);
		return Ones((ushort)(ret >> 1));
	}

	/// <summary>
	///     Returns the base 2 logarithm for a given number. This is the number of filled bits (bits right of the last leading
	///     zero).
	/// </summary>
	/// <param name="value">The value for which to give the base 2 logarithm.</param>
	/// <returns>The base 2 logarithm for the <paramref name="value" />. Zero (0) for <paramref name="value" />==zero (0).</returns>
	public static int Log2(uint value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		return Ones(ret >> 1);
	}

	/// <summary>
	///     Returns the base 2 logarithm for a given number. This is the number of filled bits (bits right of the last leading
	///     zero).
	/// </summary>
	/// <param name="value">The value for which to give the base 2 logarithm.</param>
	/// <returns>The base 2 logarithm for the <paramref name="value" />. Zero (0) for <paramref name="value" />==zero (0).</returns>
	public static int Log2(ulong value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		ret |= ret >> 32;
		return Ones(ret >> 1);
	}

	/// <summary>
	///     Returns the most significant 1 bit (highest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the most significant 1 bit.</param>
	/// <returns>The value with only the most significant 1 bit set, or zero (0).</returns>
	public static byte MostSignificantOneBit(byte value)
	{
		var ret = value;
		ret |= (byte)(ret >> 1);
		ret |= (byte)(ret >> 2);
		ret |= (byte)(ret >> 4);
		return (byte)(ret & ~(ret >> 1));
	}

	/// <summary>
	///     Returns the most significant 1 bit (highest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the most significant 1 bit.</param>
	/// <returns>The value with only the most significant 1 bit set, or zero (0).</returns>
	public static ushort MostSignificantOneBit(ushort value)
	{
		var ret = value;
		ret |= (ushort)(ret >> 1);
		ret |= (ushort)(ret >> 2);
		ret |= (ushort)(ret >> 4);
		ret |= (ushort)(ret >> 8);
		return (ushort)(ret & ~(ret >> 1));
	}

	/// <summary>
	///     Returns the most significant 1 bit (highest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the most significant 1 bit.</param>
	/// <returns>The value with only the most significant 1 bit set, or zero (0).</returns>
	public static uint MostSignificantOneBit(uint value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		return ret & ~(ret >> 1);
	}

	/// <summary>
	///     Returns the most significant 1 bit (highest numbered element of a bit set) for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the most significant 1 bit.</param>
	/// <returns>The value with only the most significant 1 bit set, or zero (0).</returns>
	public static ulong MostSignificantOneBit(ulong value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		ret |= ret >> 32;
		return ret & ~(ret >> 1);
	}

	/// <summary>
	///     Returns the next largest power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the next largest power of 2.</param>
	/// <returns>The next largest power of 2 for the <paramref name="value" />.</returns>
	public static byte NextLargestPowerOf2(byte value)
	{
		var ret = value;
		ret |= (byte)(ret >> 1);
		ret |= (byte)(ret >> 2);
		ret |= (byte)(ret >> 4);
		return (byte)(ret + 1);
	}

	/// <summary>
	///     Returns the next largest power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the next largest power of 2.</param>
	/// <returns>The next largest power of 2 for the <paramref name="value" />.</returns>
	public static ushort NextLargestPowerOf2(ushort value)
	{
		var ret = value;
		ret |= (ushort)(ret >> 1);
		ret |= (ushort)(ret >> 2);
		ret |= (ushort)(ret >> 4);
		ret |= (ushort)(ret >> 8);
		return (ushort)(ret + 1);
	}

	/// <summary>
	///     Returns the next largest power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the next largest power of 2.</param>
	/// <returns>The next largest power of 2 for the <paramref name="value" />.</returns>
	public static uint NextLargestPowerOf2(uint value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		return ret + 1;
	}

	/// <summary>
	///     Returns the next largest power of 2 for a given value.
	/// </summary>
	/// <param name="value">The value for which to find the next largest power of 2.</param>
	/// <returns>The next largest power of 2 for the <paramref name="value" />.</returns>
	public static ulong NextLargestPowerOf2(ulong value)
	{
		var ret = value;
		ret |= ret >> 1;
		ret |= ret >> 2;
		ret |= ret >> 4;
		ret |= ret >> 8;
		ret |= ret >> 16;
		ret |= ret >> 32;
		return ret + 1;
	}

	/// <summary>
	///     Gets the number of 1-bits in a value.
	/// </summary>
	/// <param name="value">The value for which to count the ones.</param>
	/// <returns>The number of 1-bits in <paramref name="value" />.</returns>
	public static int Ones(byte value)
	{
		int ret = value;
		ret -= (ret >> 1) & 0x55;
		ret = ((ret >> 2) & 0x33) + (ret & 0x33);
		return ((ret >> 4) + ret) & 0xf;
	}

	/// <summary>
	///     Gets the number of 1-bits in a value.
	/// </summary>
	/// <param name="value">The value for which to count the ones.</param>
	/// <returns>The number of 1-bits in <paramref name="value" />.</returns>
	public static int Ones(ushort value)
	{
		int ret = value;
		ret -= (ret >> 1) & 0x5555;
		ret = ((ret >> 2) & 0x3333) + (ret & 0x3333);
		ret = ((ret >> 4) + ret) & 0x0f0f;
		return ((ret * 0x0101) >> 8) & 0x1f;
	}

	/// <summary>
	///     Gets the number of 1-bits in a value.
	/// </summary>
	/// <param name="value">The value for which to count the ones.</param>
	/// <returns>The number of 1-bits in <paramref name="value" />.</returns>
	public static int Ones(uint value)
	{
		var ret = value;
		ret -= (ret >> 1) & 0x55555555;
		ret = ((ret >> 2) & 0x33333333) + (ret & 0x33333333);
		ret = ((ret >> 4) + ret) & 0x0f0f0f0f;
		return (int)((ret * 0x01010101) >> 24) & 0x3f;
	}

	/// <summary>
	///     Gets the number of 1-bits in a value.
	/// </summary>
	/// <param name="value">The value for which to count the ones.</param>
	/// <returns>The number of 1-bits in <paramref name="value" />.</returns>
	public static int Ones(ulong value)
	{
		var ret = value;
		ret -= (ret >> 1) & 0x5555555555555555;
		ret = ((ret >> 2) & 0x3333333333333333) + (ret & 0x3333333333333333);
		ret = ((ret >> 4) + ret) & 0x0f0f0f0f0f0f0f0f;
		return (int)((ret * 0x0101010101010101) >> 56);
	}
}