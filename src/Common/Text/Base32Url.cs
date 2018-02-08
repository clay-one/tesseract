using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Tesseract.Common.Text
{
    public class Base32Url
    {
        public const char StandardPaddingChar = '=';
        public const string Base32StandardAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        public const string ZBase32Alphabet = "ybndrfg8ejkmcpqxot1uwisza345h769";

        private static readonly Dictionary<string, Dictionary<string, uint>> Indexes =
            new Dictionary<string, Dictionary<string, uint>>(2, StringComparer.InvariantCulture);

        private readonly string _alphabet;
        private Dictionary<string, uint> _index;
        public bool IgnoreWhiteSpaceWhenDecoding;
        public bool IsCaseSensitive;
        public char PaddingChar;
        public bool UsePadding;

        public Base32Url()
            : this(false, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567")
        {
        }

        public Base32Url(bool padding)
            : this(padding, false, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567")
        {
        }

        public Base32Url(bool padding, bool caseSensitive)
            : this(padding, caseSensitive, false, "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567")
        {
        }

        public Base32Url(bool padding, bool caseSensitive, bool ignoreWhiteSpaceWhenDecoding)
            : this(padding, caseSensitive, ignoreWhiteSpaceWhenDecoding, "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567")
        {
        }

        public Base32Url(string alternateAlphabet)
            : this(false, false, false, alternateAlphabet)
        {
        }

        public Base32Url(bool padding, bool caseSensitive, bool ignoreWhiteSpaceWhenDecoding, string alternateAlphabet)
        {
            if (alternateAlphabet.Length != 32)
            {
                throw new ArgumentException("Alphabet must be exactly 32 characters long for base 32 encoding.");
            }

            PaddingChar = '=';
            UsePadding = padding;
            IsCaseSensitive = caseSensitive;
            IgnoreWhiteSpaceWhenDecoding = ignoreWhiteSpaceWhenDecoding;
            _alphabet = alternateAlphabet;
        }

        public static byte[] FromBase32String(string input)
        {
            return new Base32Url().Decode(input);
        }

        public static string ToBase32String(byte[] data)
        {
            return new Base32Url().Encode(data);
        }

        public string Encode(byte[] data)
        {
            var stringBuilder = new StringBuilder(Math.Max((int) Math.Ceiling(data.Length * 8 / 5.0), 1));
            var numArray1 = new byte[8];
            var numArray2 = new byte[8];
            var sourceIndex = 0;
            while (sourceIndex < data.Length)
            {
                var length = Math.Min(data.Length - sourceIndex, 5);
                Array.Copy(numArray1, numArray2, numArray1.Length);
                Array.Copy(data, sourceIndex, numArray2, numArray2.Length - (length + 1), length);
                Array.Reverse(numArray2);
                var uint64 = BitConverter.ToUInt64(numArray2, 0);
                var num = (length + 1) * 8 - 5;
                while (num > 3)
                {
                    stringBuilder.Append(_alphabet[(int) ((long) (uint64 >> num) & 31L)]);
                    num -= 5;
                }

                sourceIndex += 5;
            }

            if (UsePadding)
            {
                stringBuilder.Append(
                    string.Empty.PadRight(stringBuilder.Length % 8 == 0 ? 0 : 8 - stringBuilder.Length % 8,
                        PaddingChar));
            }

            return stringBuilder.ToString();
        }

        public byte[] Decode(string input)
        {
            if (IgnoreWhiteSpaceWhenDecoding)
            {
                input = Regex.Replace(input, "\\s+", "");
            }

            if (UsePadding)
            {
                if (input.Length % 8 != 0)
                {
                    throw new ArgumentException("Invalid length for a base32 string with padding.");
                }

                input = input.TrimEnd(PaddingChar);
            }

            EnsureAlphabetIndexed();
            var memoryStream = new MemoryStream(Math.Max((int) Math.Ceiling(input.Length * 5 / 8.0), 1));
            var num1 = 0;
            while (num1 < input.Length)
            {
                var num2 = Math.Min(input.Length - num1, 8);
                ulong num3 = 0;
                var count = (int) Math.Floor(num2 * 0.625);
                for (var index = 0; index < num2; ++index)
                {
                    uint num4;
                    if (!_index.TryGetValue(input.Substring(num1 + index, 1), out num4))
                    {
                        throw new ArgumentException("Invalid character '" + input.Substring(num1 + index, 1) +
                                                    "' in base32 string, valid characters are: " + _alphabet);
                    }

                    num3 |= (ulong) num4 << ((count + 1) * 8 - index * 5 - 5);
                }

                var bytes = BitConverter.GetBytes(num3);
                Array.Reverse(bytes);
                memoryStream.Write(bytes, bytes.Length - (count + 1), count);
                num1 += 8;
            }

            return memoryStream.ToArray();
        }

        private void EnsureAlphabetIndexed()
        {
            if (_index != null)
            {
                return;
            }

            var key = (IsCaseSensitive ? "S" : "I") + _alphabet;
            Dictionary<string, uint> dictionary;
            if (!Indexes.TryGetValue(key, out dictionary))
            {
                lock (Indexes)
                {
                    if (!Indexes.TryGetValue(key, out dictionary))
                    {
                        dictionary = new Dictionary<string, uint>(_alphabet.Length,
                            IsCaseSensitive
                                ? StringComparer.InvariantCulture
                                : StringComparer.InvariantCultureIgnoreCase);
                        for (var startIndex = 0; startIndex < _alphabet.Length; ++startIndex)
                        {
                            dictionary[_alphabet.Substring(startIndex, 1)] = (uint) startIndex;
                        }

                        Indexes.Add(key, dictionary);
                    }
                }
            }

            _index = dictionary;
        }
    }
}