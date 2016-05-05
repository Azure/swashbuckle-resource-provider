using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Swashbuckle.Swagger
{
    internal static class SwaggerExtensions
    {
        public static IEnumerable<KeyValuePair<HttpMethod, Operation>> Operations(this PathItem pathItem)
        {
            var allResults = new Dictionary<HttpMethod, Operation>
            {
                { HttpMethod.Delete,        pathItem.delete },
                { HttpMethod.Get,           pathItem.get },
                { HttpMethod.Head,          pathItem.head },
                { HttpMethod.Options,       pathItem.options },
                { new HttpMethod("Patch"),  pathItem.patch },
                { HttpMethod.Post,          pathItem.post },
                { HttpMethod.Put,           pathItem.put },
            };

            return from i in allResults
                   where i.Value != null
                   select i;
        }

        public static void DeleteOperation(this PathItem pathItem, Operation operation)
        {
            DeleteIfMatches(operation, ref pathItem.delete);
            DeleteIfMatches(operation, ref pathItem.get);
            DeleteIfMatches(operation, ref pathItem.head);
            DeleteIfMatches(operation, ref pathItem.options);
            DeleteIfMatches(operation, ref pathItem.patch);
            DeleteIfMatches(operation, ref pathItem.post);
            DeleteIfMatches(operation, ref pathItem.put);
        }

        private static void DeleteIfMatches(Operation operation, ref Operation op)
        {
            if (operation == op)
            {
                op = null;
            }
        }

        // Look for the value between:
        //  - the prefix of:
        //    a. optional backslash and opening quotation mark
        //    b. adminpassword, sshkey, or password
        //    c. optional backslash and closing quotation mark
        //    d. optional space around a colon
        //    e. optional backslash and opening quotation mark
        //
        //  - a suffix of:
        //    a. optional backslash and closing quotation mark
        private static readonly Regex _secretMatcher = new Regex(
            @"(?<=\\*""(adminpassword|sshkey|password)\\*""\s*:\s*\\*"").*?(?=\\*"")",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Makes the first letter of a string lower cased
        /// </summary>
        public static string ToCamelCase(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString) ||
                Char.IsLower(theString[0]))
            {
                return theString;
            }

            var letters = theString.ToCharArray();
            letters[0] = Char.ToLower(letters[0]);

            return new string(letters);
        }

        public static string SafeSubstring(this string theString, int startIndex, int maxLength)
        {
            if (theString == null)
            {
                return theString;
            }

            if (startIndex + maxLength >= theString.Length)
            {
                return startIndex == 0 ? theString : theString.Substring(startIndex);
            }

            return theString.Substring(startIndex, maxLength);
        }

        /// <summary>
        /// Makes the first letter of a string upper cased
        /// </summary>
        public static string ToPascalCase(this string theString)
        {
            if (string.IsNullOrWhiteSpace(theString) ||
                Char.IsUpper(theString[0]))
            {
                return theString;
            }

            var letters = theString.ToCharArray();
            letters[0] = Char.ToUpper(letters[0]);

            return new string(letters);
        }

        public static string TrimEnd(this string theString, string suffix)
        {
            if (theString != null && theString.EndsWith(suffix))
            {
                return theString.Substring(0, theString.Length - suffix.Length);
            }

            return theString;
        }

        public static string ReplaceEnd(this string theString, string from, string to)
        {
            if (theString != null && theString.EndsWith(from))
            {
                return theString.Substring(0, theString.Length - from.Length) + to;
            }

            return theString;
        }

        public static string TrimStart(this string theString, string prefix)
        {
            if (theString != null && theString.StartsWith(prefix))
            {
                return theString.Substring(prefix.Length);
            }

            return theString;
        }

        public static bool SafeStartWith(this string theString, string value)
        {
            if (theString == null)
            {
                return false;
            }

            return theString.StartsWith(value);
        }

    }
}
