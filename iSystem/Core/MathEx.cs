// ReSharper disable UnusedMember.Global

using static System.Math;

namespace iSystem.Core;

public static class MathEx
{
    public static double RoundDown(double d, int decimalPlace)
    {
        var powNumber = Pow(10d, decimalPlace);
        return Floor(d * powNumber) / powNumber;
    }
}