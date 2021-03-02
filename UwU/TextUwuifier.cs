using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DiscordUwuBot.UwU
{
    public interface ITextUwuifier
    {
        public string UwuifyText(string text);
    }

    public record StringReplacement(Regex MatchRegex, MatchEvaluator MatchReplacer);
    
    public class TextUwuifier : ITextUwuifier
    {
        public IEnumerable<StringReplacement> UwuReplacements { get; init; } = GetDefaultUwuReplacements();

        public string UwuifyText(string text)
        {
            foreach (var replacement in UwuReplacements)
            {
                text = replacement.MatchRegex.Replace(text, replacement.MatchReplacer);
            }

            text += " UwU!";
            return text;
        }
        
        private static string MatchCase(string casePattern, string textPattern)
        {
            var matchLength = Math.Min(casePattern.Length, textPattern.Length);

            var resultBuilder = new StringBuilder();
            for (var i = 0; i < matchLength; i++)
            {
                var isUpper = char.IsUpper(casePattern[i]);
                resultBuilder.Append(isUpper ? char.ToUpper(textPattern[i]) : char.ToLower(textPattern[i]));
            }

            if (matchLength < textPattern.Length)
            {
                resultBuilder.Append(textPattern.Substring(matchLength));
            }

            return resultBuilder.ToString();
        }
        
        private const RegexOptions RegexCI = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 0, 0, 50);
        
        private static IEnumerable<StringReplacement> GetDefaultUwuReplacements()
        {
            return new StringReplacement[] {
                new(new Regex("ll",  RegexCI, DefaultTimeout), m => MatchCase(m.Value, "wl")),
                new(new Regex("r",  RegexCI, DefaultTimeout), m => MatchCase(m.Value, "w")),
                new(new Regex("th", RegexCI, DefaultTimeout), m => MatchCase(m.Value, "dw")),
                new(
                    new Regex("(n)([aeiou])", RegexCI, DefaultTimeout), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel =  m.Groups[2].Value;
                        return letter + MatchCase(vowel, "y") + vowel;
                    }
                ),
                new (
                    new Regex("(f)([aeiou])", RegexCI, DefaultTimeout), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel =  m.Groups[2].Value;
                        return letter + MatchCase(vowel, "w") + vowel;
                    }
                ),
                new (
                    new Regex("([aeiou])(d)", RegexCI, DefaultTimeout), m =>
                    {
                        var vowel =  m.Groups[1].Value;
                        var letter = m.Groups[2].Value;
                        return vowel + MatchCase(letter, "w") + letter;
                    }
                ),
                new(
                    new Regex("(f|b|sh)(uck|itch|it)", RegexCI, DefaultTimeout), m =>
                    {
                        var start =  m.Groups[1].Value;
                        var end = m.Groups[2].Value;
                        return start + MatchCase(end, "w") + end;
                    }
                ),
                new(
                    new Regex("(d)(amn)", RegexCI, DefaultTimeout), m =>
                    {
                        var start =  m.Groups[1].Value;
                        var end = m.Groups[2].Value;
                        return start + MatchCase(end, "y") + end;
                    }
                )
            };
        }
    }
}