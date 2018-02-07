using System;

namespace Tesseract.Common.Text
{
    public class Base32
    {
        public static byte[] ToBytes(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            input = input.TrimEnd('=');
            var length = input.Length * 5 / 8;
            var numArray = new byte[length];
            byte num1 = 0;
            byte num2 = 8;
            var index = 0;
            foreach (var c in input)
            {
                var num3 = CharToValue(c);
                if (num2 > 5)
                {
                    var num4 = num3 << (num2 - 5);
                    num1 |= (byte) num4;
                    num2 -= 5;
                }
                else
                {
                    var num4 = num3 >> (5 - num2);
                    var num5 = (byte) (num1 | (uint) num4);
                    numArray[index++] = num5;
                    num1 = (byte) (num3 << (3 + num2));
                    num2 += 3;
                }
            }

            if (index != length)
                numArray[index] = num1;
            return numArray;
        }

        public static string ToString(byte[] input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentNullException(nameof(input));
            var length = (int) Math.Ceiling(input.Length / 5.0) * 8;
            var chArray1 = new char[length];
            byte b1 = 0;
            byte num1 = 5;
            var num2 = 0;
            foreach (var num3 in input)
            {
                var b2 = (byte) (b1 | ((uint) num3 >> (8 - num1)));
                chArray1[num2++] = ValueToChar(b2);
                if (num1 < 4)
                {
                    var b3 = (byte) ((num3 >> (3 - num1)) & 31);
                    chArray1[num2++] = ValueToChar(b3);
                    num1 += 5;
                }

                num1 -= 3;
                b1 = (byte) ((num3 << num1) & 31);
            }

            if (num2 != length)
            {
                var chArray2 = chArray1;
                var index = num2;
                var num3 = index + 1;
                var num4 = (int) ValueToChar(b1);
                chArray2[index] = (char) num4;
                while (num3 != length)
                    chArray1[num3++] = '=';
            }

            return new string(chArray1);
        }

        private static int CharToValue(char c)
        {
            var num = (int) c;
            if (num < 91 && num > 64)
                return num - 65;
            if (num < 56 && num > 49)
                return num - 24;
            if (num < 123 && num > 96)
                return num - 97;
            throw new ArgumentException("Character is not a Base32 character.", nameof(c));
        }

        private static char ValueToChar(byte b)
        {
            if (b < 26)
                return (char) (b + 65U);
            if (b < 32)
                return (char) (b + 24U);
            throw new ArgumentException("Byte is not a value Base32 value.", nameof(b));
        }
    }
}