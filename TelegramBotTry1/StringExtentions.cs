using System;

namespace TelegramBotTry1
{
    public static class StringExtentions
    {
        public static string Replace(this string s, char[] separators, string newValue)
        {
            return string.Join(newValue, s.Split(separators, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}