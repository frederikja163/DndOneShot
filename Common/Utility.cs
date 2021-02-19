using System.Text;

namespace Common
{
    public static class Utility
    {
        public static readonly string JiberrishLetters = "ᔑʖᓵ↸ᒷ⎓⊣⍑╎⋮ꖌꖎᒲリ𝙹!¡ᑑ∷ᓭℸ ̣ ⚍⍊ ̇/||⨅";

        public static string ConvertToJibberish(string str)
        {
            StringBuilder builder = new StringBuilder(str.Length);
            foreach (var letter in str.ToLower())
            {
                var index = letter - 'a';
                if (char.IsLetter(letter) && index < JiberrishLetters.Length)
                {
                    builder.Append(JiberrishLetters[index]);
                }
                else
                {
                    builder.Append(letter);
                }

            }

            return builder.ToString();
        }
    }
}