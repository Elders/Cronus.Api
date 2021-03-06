﻿using System.Text.RegularExpressions;

namespace Elders.Cronus.Api.Converters
{
    public static class StringExtensions
    {
        static Regex isOnlyNumbersPattern = new Regex(@"^[0-9]+$");

        public static bool IsOnlyNumbers(this string value)
        {
            return isOnlyNumbersPattern.Match(value).Success;
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static bool CanUrlDecode(this string input)
        {
            if (input == null)
                return false;

            //  We have problems with string containing only numbers. So we do not like getting these string. If it actually happens it must be extremely rare.
            if (input.IsOnlyNumbers())
                return false;

            int len = input.Length;
            if (len < 1)
                return true;

            ///////////////////////////////////////////////////////////////////
            // Step 1: Calculate the number of padding chars to append to this string.
            //         The number of padding chars to append is stored in the last char of the string.
            int numPadChars = (int)input[len - 1] - (int)'0';
            if (numPadChars < 0 || numPadChars > 10)
                return false;


            ///////////////////////////////////////////////////////////////////
            // Step 2: Create array to store the chars (not including the last char)
            //          and the padding chars
            char[] base64Chars = new char[len - 1 + numPadChars];


            ////////////////////////////////////////////////////////
            // Step 3: Copy in the chars. Transform the "-" to "+", and "*" to "/"
            for (int iter = 0; iter < len - 1; iter++)
            {
                char c = input[iter];

                switch (c)
                {
                    case '-':
                        base64Chars[iter] = '+';
                        break;

                    case '_':
                        base64Chars[iter] = '/';
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            ////////////////////////////////////////////////////////
            // Step 4: Add padding chars
            for (int iter = len - 1; iter < base64Chars.Length; iter++)
            {
                base64Chars[iter] = '=';
            }

            return new string(base64Chars).IsBase64String();
        }

        public static string UrlDecode(this string self)
        {
            return System.Uri.UnescapeDataString(self);
        }

        public static string UrlEncode(this string self)
        {
            return System.Uri.EscapeDataString(self);
        }

        public static bool IsUrn(this string s)
        {
            return Regex.IsMatch(s, @"\b(urn):([a-z0-9][a-z0-9-]{0,31}):([a-z0-9()+,\-.=@;$_!:*'%\/?#]*[a-z0-9+=@$\/])", RegexOptions.None);
        }
    }
}
