using ClosedXML.Excel;
using System;
using System.IO;

namespace SmartCalcApp
{
    public static class Util
    {
        public static bool IsTheSame(this string value, string other)
        {
            return string.Compare(value, other, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static int ReadAsInt(this IXLCell cell)
        {
            if (int.TryParse(cell.Value.ToString(), out var val))
            {
                return val;
            }

            return 0;
        }

        public static decimal ReadAsDecimal(this IXLCell cell)
        {
            if (decimal.TryParse(cell.Value.ToString(), out var val))
            {
                return val;
            }

            return 0m;
        }

        public static bool IsAbsolutePath(this string path)
        {
            if (path is null)
            { 
                throw new ArgumentNullException(nameof(path)); 
            }

            return Path.IsPathRooted(path) && 
                   !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) &&
                   !Path.GetPathRoot(path).Equals(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}