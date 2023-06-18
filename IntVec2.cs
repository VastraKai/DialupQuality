#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Memory.Types;

public struct IntVec2 : IEquatable<IntVec2>, IFormattable
{
    public int X;
    public int Y;

    public IntVec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public IntVec2(ReadOnlySpan<int> values)
    {
        if (values.Length < 2)
            throw new ArgumentException("values must have a length of at least 2", nameof(values));

        this = Unsafe.ReadUnaligned<IntVec2>(ref Unsafe.As<int, byte>(ref MemoryMarshal.GetReference(values)));
    }

    public static IntVec2 Zero => default;

    public static IntVec2 UnitX => new(1, 0);
    public static IntVec2 UnitY => new(0, 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator +(IntVec2 left, IntVec2 right) =>
        new(
            left.X + right.X,
            left.Y + right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator -(IntVec2 left, IntVec2 right) =>
        new(
            left.X - right.X,
            left.Y - right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator /(IntVec2 left, IntVec2 right) =>
        new(
            left.X / right.X,
            left.Y / right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator *(IntVec2 left, IntVec2 right) =>
        new(
            left.X * right.X,
            left.Y * right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator *(IntVec2 left, int right) =>
        new(
            left.X * right,
            left.Y * right
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator *(int left, IntVec2 right) =>
        new(
            left * right.X,
            left * right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator /(IntVec2 left, int right) =>
        new(
            left.X / right,
            left.Y / right
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator /(int left, IntVec2 right) =>
        new(
            left / right.X,
            left / right.Y
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator +(IntVec2 left, int right) =>
        new(
            left.X + right,
            left.Y + right
        );
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator +(int left, IntVec2 right) =>
        new(
            left + right.X,
            left + right.Y
        );
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator -(IntVec2 left, int right) =>
        new(
            left.X - right,
            left.Y - right
        );
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntVec2 operator -(int left, IntVec2 right) =>
        new(
            left - right.X,
            left - right.Y
        );
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("value")]
    public static IntVec2? operator -(IntVec2? value) =>
        value.HasValue
            ? new IntVec2(-value.Value.X, -value.Value.Y)
            : null;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(IntVec2 left, IntVec2 right) =>
        left.X == right.X && left.Y == right.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(IntVec2 left, IntVec2 right) =>
        left.X != right.X || left.Y != right.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(IntVec2 left, IntVec2 right) =>
        left.X < right.X && left.Y < right.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(IntVec2 left, IntVec2 right) =>
        left.X > right.X && left.Y > right.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is IntVec2 other && Equals(other);

    public readonly bool Equals(IntVec2 other) => this == other;

    public readonly override int GetHashCode() => HashCode.Combine(X, Y);

    public readonly override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public readonly string ToString(string? format) => ToString(format, CultureInfo.CurrentCulture);
    
    public readonly string ToString(string? format, IFormatProvider? formatProvider) =>
        $"{X.ToString(format, formatProvider)}x{Y.ToString(format, formatProvider)}";
    // Methods for Parsing and Converting
    public static bool TryParse(string? s, out IntVec2 result)
    {
        result = default;
        if (s is null)
            return false;
        var split = s.Split('x');
        if (split.Length != 2)
            return false;
        if (!int.TryParse(split[0], out var x))
            return false;
        if (!int.TryParse(split[1], out var y))
            return false;
        result = new IntVec2(x, y);
        return true;
    }
    // TryParse for strings like 10x20 or 10,20
    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider, out IntVec2 result)
    {
        result = default;
        if (s is null)
            return false;
        var split = s.Split('x');
        if (split.Length != 2)
            return false;
        if (!int.TryParse(split[0], style, provider, out var x))
            return false;
        if (!int.TryParse(split[1], style, provider, out var y))
            return false;
        result = new IntVec2(x, y);
        return true;
    }
    // Normal parsing, that throws exceptions
    public static IntVec2 Parse(string s)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));
        var split = s.Split('x');
        if (split.Length != 2)
            throw new FormatException("Input string was not in a correct format. invalid length");
        if (!int.TryParse(split[0], out var x))
            throw new FormatException("Input string was not in a correct format.");
        if (!int.TryParse(split[1], out var y))
            throw new FormatException("Input string was not in a correct format.");
        return new IntVec2(x, y);
    }
    // Parsing with NumberStyles and IFormatProvider
    public static IntVec2 Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        if (s is null)
            throw new ArgumentNullException(nameof(s));
        var split = s.Split('x');
        if (split.Length != 2)
            throw new FormatException("Input string was not in a correct format. invalid length");
        if (!int.TryParse(split[0], style, provider, out var x))
            throw new FormatException("Input string was not in a correct format.");
        if (!int.TryParse(split[1], style, provider, out var y))
            throw new FormatException("Input string was not in a correct format.");
        return new IntVec2(x, y);
    }
    // Conversions
    public static explicit operator IntVec2(Vector2 vec) => new IntVec2((int)vec.X, (int)vec.Y);
    public static explicit operator IntVec2(Vector3 vec) => new IntVec2((int)vec.X, (int)vec.Y);
    public static explicit operator IntVec2(Vector4 vec) => new IntVec2((int)vec.X, (int)vec.Y);








}