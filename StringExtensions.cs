using System;
using System.Collections.Generic;
static class StringExtensions
{
    private static readonly Dictionary<char, char> indexDigitMap = new Dictionary<char, char>()
    {
        {'0', '₀'},
        {'1', '₁'},
        {'2', '₂'},
        {'3', '₃'},
        {'4', '₄'},
        {'5', '₅'},
        {'6', '₆'},
        {'7', '₇'},
        {'8', '₈'},
        {'9', '₉'},
    };
    //convert 123... to small ₁₂₃...
    public static string ToIndexDigit(this string str)
    {
        string result = "";
        foreach (char c in str)
        {
            if (indexDigitMap.ContainsKey(c))
                result += indexDigitMap[c];
            else
                result += c;
        }
        return result;
    }
}