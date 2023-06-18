using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;

namespace DialupQuality;

public static class StringExtensions
{
    public static string Repeat(this string str, int count)
    {
        var sb = new StringBuilder(str.Length * count);
        for (var i = 0; i < count; i++)
            sb.Append(str);
        return sb.ToString();
    }
}