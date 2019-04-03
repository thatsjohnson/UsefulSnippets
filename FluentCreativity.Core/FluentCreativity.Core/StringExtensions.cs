using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;

namespace FluentCreativity.Core
{
    public static class StringExtensions
    {
        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Append"></param>
        /// <returns></returns>
        public static string Append(this string value, params string[] append)
        {
            var result = value;
            append.ForEach(i => result += i);
            return result;
        }

        /// <summary>
        /// Get string after a.
        /// </summary>
        public static string After(this string value, string a)
        {
            var pos_a = value.LastIndexOf(a);

            if (pos_a == -1)
                return string.Empty;

            var adjusted = pos_a + a.Length;

            return adjusted >= value.Length ? string.Empty : value.Substring(adjusted);
        }

        /// <summary>
        /// Get string between a and b.
        /// </summary>
        public static string Between(this string value, string a, string b)
        {
            var pos_a = value.IndexOf(a);
            var pos_b = value.LastIndexOf(b);

            if (pos_a == -1)
                return string.Empty;

            if (pos_b == -1)
                return string.Empty;

            var adjusted = pos_a + a.Length;

            return adjusted >= pos_b ? string.Empty : value.Substring(adjusted, pos_b - adjusted);
        }

        /// <summary>
        /// Get string before a a.
        /// </summary>
        public static string Before(this string value, string a)
        {
            var pos_a = value.IndexOf(a);
            return pos_a == -1 ? string.Empty : value.Substring(0, pos_a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Capitalize(this string Value)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Value.ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Value, params char[] Values)
        {
            return Value.EndsWithAny(Values.Select(i => i.ToString()).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Value, params object[] Values)
        {
            return Value.EndsWithAny(Values.Select(i => i.ToString()).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Values"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Value, params string[] Values)
        {
            foreach (var i in Values)
            {
                if (Value.EndsWith(i))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns <see cref="Uri"/> of resource defined in assembly with given path; note, path must NOT begin with slash and slashes must be forward-facing.
        /// </summary>
        public static Uri GetResourceUri(this string AssemblyName, string ResourcePath)
        {
            return new Uri("pack://application:,,,/" + AssemblyName + ";component/" + ResourcePath, UriKind.Absolute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TypeName"></param>
        /// <returns></returns>
        public static Type FindType(this string TypeName)
        {
            var type = Type.GetType(TypeName);

            if (type != null)
                return type;

            foreach (var i in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = i.GetType(TypeName);
                if (type != null)
                    return type;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string Value)
        {
            return Regex.IsMatch(Value, @"^[a-zA-Z0-9]+$");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToCheck"></param>
        /// <returns></returns>
        public static bool IsDouble(this string ToCheck)
        {
            double n;
            return double.TryParse(ToCheck, out n);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToEvaluate"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string ToEvaluate)
        {
            return ToEvaluate.Length == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToCheck"></param>
        /// <returns></returns>
        public static bool IsInt(this string ToCheck)
        {
            int n;
            return int.TryParse(ToCheck, out n);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToCheck"></param>
        /// <returns></returns>
        public static bool IsLong(this string ToCheck)
        {
            long n;
            return long.TryParse(ToCheck, NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out n);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToEvaluate"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string ToEvaluate)
        {
            return string.IsNullOrEmpty(ToEvaluate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToEvaluate"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string ToEvaluate)
        {
            return string.IsNullOrWhiteSpace(ToEvaluate) || ToEvaluate.All(char.IsWhiteSpace);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToCheck"></param>
        /// <returns></returns>
        public static bool IsShort(this string ToCheck)
        {
            short n;
            return short.TryParse(ToCheck, out n);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Schemes"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string Value, params string[] Schemes)
        {
            Uri Uri;
            return Uri.TryCreate(Value, UriKind.Absolute, out Uri) && (Schemes.Length > 0 ? Uri.Scheme.EqualsAny(Schemes) : Uri.Scheme.EqualsAny(Uri.UriSchemeFile, Uri.UriSchemeFtp, Uri.UriSchemeHttp, Uri.UriSchemeHttps, Uri.UriSchemeMailto));
        }

        /// <summary>
        /// Parses <see cref="string"/> to <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="Value"></param>
        /// <param name="IgnoreCase"></param>
        /// <returns></returns>
        public static TEnum ParseEnum<TEnum>(this string Value, bool IgnoreCase = true) where TEnum : struct, IFormattable, IComparable, IConvertible
        {
            return (TEnum)Enum.Parse(typeof(TEnum), Value, IgnoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Prepend"></param>
        /// <returns></returns>
        public static string Prepend(this string value, params string[] prepend)
        {
            var result = string.Empty;
            prepend.ForEach(i => result += i);
            return result + value;
        }

        /// <summary>
        /// Parses string to boolean.
        /// </summary>
        public static string SplitCamelCase(this string ToConvert)
        {
            return Regex.Replace(Regex.Replace(ToConvert, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        /// <summary>
        /// Determines whether [source] contains [toCheck] value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="toCheck">To check.</param>
        /// <param name="comp">The comp.</param>
        /// <returns>
        ///   <c>true</c> if [source] contains [toCheck]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Parses string to boolean (evaluates "true" and "false"; everything else is parsed to an int).
        /// </summary>
        public static bool? ToBool(this string ToConvert)
        {
            switch (ToConvert.ToLower())
            {
                case "true":
                case "t":
                case "1":
                    return true;
                case "false":
                case "f":
                case "0":
                    return false;
            }
            return null;
        }

        /// <summary>
        /// Parses string to byte.
        /// </summary>
        public static byte ToByte(this string ToConvert)
        {
            byte Value = default(byte);
            byte.TryParse(ToConvert, out Value);
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static char ToChar(this string Value)
        {
            var Result = default(char);
            char.TryParse(Value, out Result);
            return Result;
        }

        /// <summary>
        /// Parses string to short.
        /// </summary>
        public static short ToInt16(this string ToConvert)
        {
            short Value = default(short);
            short.TryParse(ToConvert, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to int.
        /// </summary>
        public static int ToInt32(this string ToConvert)
        {
            int Value = default(int);
            int.TryParse(ToConvert, NumberStyles.AllowThousands |
                                    NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to long.
        /// </summary>
        public static long ToInt64(this string ToConvert)
        {
            long Value = default(long);
            long.TryParse(ToConvert, out Value);
            return Value;
        }

        public static IEnumerable<int> ToInt32Array(this string Value, char Separator = ',')
        {
            return Value.ToInt32Array(Separator as char?);
        }

        public static IEnumerable<int> ToInt32Array(this string Value, char? Separator)
        {
            if (String.IsNullOrEmpty(Value))
                yield break;

            if (Separator == null)
            {
                foreach (var i in Value.ToArray())
                    yield return i.ToString().ToInt32();
            }
            else
            {
                foreach (var i in Value.Split(Separator.Value))
                    yield return i.ToInt32();
            }
        }

        /// <summary>
        /// Parses string to DateTime.
        /// </summary>
        public static DateTime ToDateTime(this string ToConvert)
        {
            DateTime Value = default(DateTime);
            DateTime.TryParse(ToConvert, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to decimal.
        /// </summary>
        public static decimal ToDecimal(this string ToConvert)
        {
            decimal Value = default(decimal);
            decimal.TryParse(ToConvert, NumberStyles.AllowLeadingSign |
                                        NumberStyles.AllowThousands |
                                        NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to double.
        /// </summary>
        public static double ToDouble(this string ToConvert)
        {
            double Value = default(double);
            double.TryParse(ToConvert, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to int.
        /// </summary>
        [Obsolete("Use ToInt16 instead.")]
        public static int ToInt(this string ToConvert)
        {
            int Value = default(int);
            int.TryParse(ToConvert, out Value);
            return Value;
        }

        /// <summary>
        /// Parses string to long.
        /// </summary>
        [Obsolete("Use ToInt32 instead.")]
        public static long ToLong(this string ToConvert)
        {
            long Value = default(long);
            long.TryParse(ToConvert, out Value);
            return Value;
        }

        public static SecureString ToSecureString(this string ToConvert)
        {
            var Result = new SecureString();
            if (!ToConvert.IsNullOrWhiteSpace())
            {
                foreach (char c in ToConvert)
                    Result.AppendChar(c);
            }
            return Result;
        }

        /// <summary>
        /// Parses string to short.
        /// </summary>
        [Obsolete("Use ToInt16 instead.")]
        public static short ToShort(this string ToConvert)
        {
            short Value = default(short);
            short.TryParse(ToConvert, out Value);
            return Value;
        }

        public static string DateToFileFormat(this DateTime date, bool includeTime = false)
        {
            var timeString = "";
            if (includeTime)
                timeString = TwoDigitNumberFormat(date.Hour) + TwoDigitNumberFormat(date.Minute) + TwoDigitNumberFormat(date.Second);
            return TwoDigitNumberFormat(date.Month + 1) + TwoDigitNumberFormat(date.Day) + date.Year + timeString;
        }

        private static string TwoDigitNumberFormat(int n)
        {
            return n.ToString().Length < 2 ? n.ToString().Insert(0, "0") : n.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="Delimiter"></param>
        /// <returns></returns>
        public static Version ToVersion(this string raw, char Delimiter = '.')
        {
            int major = 0, minor = 0, build = 0;
            string[] tokens = raw.Split(Delimiter);
            if (tokens.Length > 0)
            {
                int.TryParse(tokens[0], out major);
                if (tokens.Length > 1)
                {
                    int.TryParse(tokens[1], out minor);
                    if (tokens.Length > 2)
                        int.TryParse(tokens[2], out build);
                }
            }
            return new Version(major, minor, build);
        }

        /// <summary>
        /// Attempts to parse <see cref="string"/> to <see cref="Enum"/>.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        /// <param name="IgnoreCase"></param>
        /// <returns></returns>
        public static bool TryParseEnum<TEnum>(this string OldValue, out TEnum NewValue, bool IgnoreCase = true) where TEnum : struct, IFormattable, IComparable, IConvertible
        {
            NewValue = default(TEnum);
            return Enum.TryParse(OldValue, IgnoreCase, out NewValue);
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public static string TrimZeros(this string str)
        {
            var result = str.TrimEnd('0');
            return result.EndsWith(".") ? result.TrimEnd('.') : result;
        }

        public static string ToSpacedTitleCase(this string s)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo
                .ToTitleCase(Regex.Replace(s,
                    "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 "));
        }

        #endregion
    }
}
