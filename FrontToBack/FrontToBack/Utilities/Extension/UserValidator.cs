using FrontToBack.Utilities.Enums;
using FrontToBack.ViewModels;
using System.Text.RegularExpressions;

namespace FrontToBack.Utilities.Extension
{
    public static class UserValidator
    {
        public static string CapitalizeName(this string name)
        {
            string[] words = name.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }

            return string.Join(" ", words);
        }
        public static bool CheeckWords(this RegisterVM userVM, string str)
        {


            if (!str.Trim().Contains(" "))
            {
                return true;
            }
            return false;
        }
        public static bool CheeckEmail(this RegisterVM userVM, string str)
        {
            string pattern = "^(?i)[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$";
            Regex regex = new Regex(pattern);
            if (regex.IsMatch(str))
            {
                return true;
            }
            return false;
        }
        public static bool IsDigit(this RegisterVM userVM, string str)
        {
            bool result = false;

            foreach (Char letter in str)
            {
                result = Char.IsDigit(letter);
            }
            return result;
        }
        public static bool IsSymbol(this RegisterVM userVM, string str)
        {
            bool result = false;

            foreach (Char letter in str)
            {
                result = Char.IsSymbol(letter);
            }
            return result;
        }
        
    }
}

