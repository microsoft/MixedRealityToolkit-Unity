// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.MixedReality.Toolkit.Data
{
    internal class TemplateString
    {
        private struct VarTokens
        {
            public string name;
            public string prefix;
        }

        private string _originalTemplate;
        private VarTokens[] _tokens;
        private string _tokenPostfix;
        private StringBuilder _builder = new StringBuilder();

        public string OriginalTemplate => _originalTemplate;
        public int VariableCount => _tokens.Length;
        public IEnumerable<string> VariableNames { get { foreach (var tok in _tokens) { yield return tok.name; } } }

        public TemplateString(string template)
        {
            SetTemplateString(template);
        }

        public void SetTemplateString(string template)
        {
            int count = DataParser.CountVariables(template);

            _tokenPostfix = null;
            _originalTemplate = template;
            _tokens = new VarTokens[count];

            int curr = 0;
            int idx = 0;
            while (DataParser.FindVariable(template, curr, out int start, out int end, out int tokenStart, out int tokenEnd))
            {
                _tokens[idx].name = template.Substring(start, end - start);
                _tokens[idx].prefix = (tokenStart - curr) > 0 ? template.Substring(curr, tokenStart - curr) : null;
                curr = tokenEnd;
                idx += 1;

                if (idx > count) { throw (new Exception("DataParser.CountVariables and DataParser.FindVariables have drifted apart!")); }
            }
            if (curr < template.Length) { _tokenPostfix = template.Substring(curr, template.Length - curr); }
        }

        public string Build(Func<string, string> lookup)
        {
            _builder.Clear();
            for (int i = 0; i < _tokens.Length; i++)
            {
                _builder.Append(_tokens[i].prefix);
                _builder.Append(lookup(_tokens[i].name));
            }
            _builder.Append(_tokenPostfix);
            return _builder.ToString();
        }
    }

    internal static class DataParser
    {
        /// <summary>Searches for items enclosed in moustache templating tags
        /// in the form of '{{ variable }}'.</summary>
        /// <param name="templateStr">Source string to search.</param>
        /// <param name="searchFromIndex">Where to start searching in this
        /// string. You can pass in the 'end' parameter from previous calls to
        /// this function.</param>
        /// <param name="varStart">If the function returns true, this is the
        /// start of the variable substring.</param>
        /// <param name="varEnd">If the function returns true, this is the end
        /// of the variable substring.</param>
        /// <param name="tokenStart">If the function returns true, this is the
        /// start of the variable's entire token substring.</param>
        /// <param name="tokenEnd">If the function returns true, this is the end
        /// of the variable's entire token substring.</param>
        /// <returns>True if a variable was found, false if not.</returns>
        public static bool FindVariable(string templateStr, int searchFromIndex, out int varStart, out int varEnd, out int tokenStart, out int tokenEnd)
        {
            // Based on this regex:
            // {{\s*([a-zA-Z0-9\[\]\-._]+)\s}}

            tokenStart = -1;
            tokenEnd = -1;
            varStart = -1;
            varEnd = -1;

            int openCt = 0;
            int closeCt = 0;
            for (int i = searchFromIndex; i < templateStr.Length; i++)
            {
                char c = templateStr[i];

                if (openCt != 2)
                {
                    // We're still searching for an 'open moustache' indicator,
                    // 2 uninterrupted {{ in a row.
                    if (c == '{')
                    {
                        openCt++;
                        if (openCt == 1) { tokenStart = i; }
                    }
                    else { openCt = 0; }
                }
                else
                {
                    // We're inside of the variable indicator, and we'll track
                    // the non-whitespace characters until we hit 2
                    // uninterrupted }} characters.
                    if (c == '}')
                    {
                        closeCt++;
                        if (closeCt == 2)
                        {
                            // If we closed the variable, but didn't find
                            // anything other than whitespace, then set up an
                            // empty substring. Return true either way, as it
                            // matches the {{}} format.
                            if (varStart == -1) { varStart = varEnd = i; }

                            tokenEnd = i + 1;
                            return true;
                        }
                    }
                    else
                    {
                        closeCt = 0;
                        // Track it, as long as it's not leading or trailing
                        // whitespace.
                        if (!char.IsWhiteSpace(c))
                        {
                            if (varStart == -1) { varStart = i; }
                            varEnd = i + 1;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>This counts the number of moustache variables that are
        /// embedded in the provided template string.</summary>
        /// <param name="templateStr">A string containing {{ }} templated var
        /// names. Or not, won't cause problems.</param>
        /// <returns>The number of variables found in the template string.
        /// </returns>
        public static int CountVariables(string templateStr)
        {
            int result = 0;
            int openCt = 0;
            int closeCt = 0;
            for (int i = 0; i < templateStr.Length; i++)
            {
                char c = templateStr[i];
                if (openCt != 2)
                {
                    // We're still searching for an 'open moustache' indicator,
                    // 2 uninterrupted {{ in a row.
                    if (c == '{') { openCt++; }
                    else { openCt = 0; }
                }
                else
                {
                    // We're inside of the variable indicator, now we're
                    // looking for 2 uninterrupted }} characters.
                    if (c == '}')
                    {
                        closeCt++;
                        if (closeCt == 2)
                        {
                            result++;
                            openCt = 0;
                            closeCt = 0;
                        }
                    }
                    else { closeCt = 0; }
                }
            }
            return result;
        }

        /// <summary> This parses a string to find the first occurrence of a
        /// '[]' token. You can get the token contents excluding the brackets
        /// with a Substring call:
        /// `str.Substring(start, end - start)`
        /// 
        /// This function is a (not 1:1) functionally equivalent performance
        /// replacement for a regex pattern:
        /// ^\s*\[\s*([a-zA-Z0-9\-_]*?)\s*\]</summary>
        /// <param name="str">A string in a format similar to "text[12].var"</param>
        /// <param name="start">The index of the first character inside_ of a '[]' token, -1 if no token is found.</param>
        /// <param name="end">The index of the ']' character in the '[]' token, -1 if no token is found.</param>
        /// <returns>True if a '[]' token was found, false otherwise.</returns>
        public static bool FindKeypathArrayToken(string str, out int start, out int end)
        {
            start = -1;
            end = -1;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (start == -1 && (!char.IsWhiteSpace(ch) && ch != '[')) { return false; } // After leading whitespaces, this string starts with a character that is NOT an array bracket, fail out
                else if (start == -1 && ch == '[') { start = i + 1; }                              // This character is the first open bracket in the string
                else if (start != -1 && ch == ']') { end = i; return true; }                   // This is the first close bracket after the first open bracket
            }
            // No close bracket was found, potentially no open bracket either. This could potentially be a malformed string.
            return false;
        }


        /// <summary>
        /// 
        /// This function is a (not 1:1) functionally equivalent performance
        /// replacement for a regex pattern:
        /// ^\s*([a-zA-Z0-9\-_]+?)(?:[.\[]|$)
        /// </summary>
        public static bool FindKeypathToken(string str, out int start, out int end)
        {
            start = -1;
            end = -1;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                if (ch == '[' || ch == '.') { end = i; break; } // These characters define the end of a token.
                else if (start == -1 && !char.IsWhiteSpace(ch)) { start = i; }        // The first non-whitespace character is the start of the token.
            }
            if (end == -1) { end = str.Length; } // If we didn't find an end token character, then end of the string is also good.
            return start != -1;                  // If we never found a start character, this isn't a token.
        }

        /// <summary>This is a simplified integer parser that works from a
        /// substring, avoiding the need to allocate a separate substring
        /// variable. It does _not_ handle trailing or leading spaces.
        /// <param name="str">String parent to parse.</param>
        public static bool TryParseIntSubstring(string str, int start, int end, out int result)
        {
            int multiplier = 1;
            result = 0;
            for (int i = end - 1; i >= start; i--)
            {
                char ch = str[i];
                if (char.IsDigit(ch))
                {
                    if (result < 0) { return false; } //
                    result += (ch - '0') * multiplier;
                    multiplier *= 10;
                }
                else if (ch == '-')
                {
                    result *= -1;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
