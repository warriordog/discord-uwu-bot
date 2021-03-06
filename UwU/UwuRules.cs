using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace DiscordUwuBot.UwU
{
    /// <summary>
    /// A conditional text transformation.
    /// If <see cref="MatchRegex"/> matches, then <see cref="MatchReplacer"/> is called to transform the matched substring. 
    /// </summary>
    public record TextTransformation(Regex MatchRegex, MatchEvaluator MatchReplacer);
    
    /// <summary>
    /// Parameters and logic for translating English to UwU
    /// </summary>
    public interface IUwuRules
    {
        /// <summary>
        /// Text transformations that should be applied to input
        /// </summary>
        public IEnumerable<TextTransformation> UwuTransformations { get; }
    }

    /// <summary>
    /// Settings to configure the UwU translation.
    /// </summary>
    public class UwuOptions
    {
        /// <summary>
        /// If true (default), append a trailing "UwU!" to the text.
        /// </summary>
        public bool AppendUwu { get; init; } = true;
        
        /// <summary>
        /// If true (default), make curse words cuter.
        /// </summary>
        public bool MakeCuteCurses { get; init; } = true;
    }
    
    /// <summary>
    /// Default implementation of UwU rules
    /// </summary>
    public class UwuRules : IUwuRules
    {
        public IEnumerable<TextTransformation> UwuTransformations { get; }

        /// <summary>
        /// Create a new TextUwuifier.
        /// The provided UwuRulesOptions will be used to configure the UwU translation logic.
        /// </summary>
        public UwuRules(IOptions<UwuOptions> uwuRulesOptions)
        {
            UwuTransformations = BuildUwuTransformations(uwuRulesOptions.Value);
        }

        /// <summary>
        /// Combined RegexOptions for Compiled and Case-insensitive.
        /// </summary>
        private const RegexOptions RegexCI = RegexOptions.Compiled | RegexOptions.IgnoreCase;

        /// <summary>
        /// Default timeout for matching regular expressions.
        /// </summary>
        private static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 0, 0, 25);

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

        private static IEnumerable<TextTransformation> BuildUwuTransformations(UwuOptions uwuOptions)
        {
            // Initialize the rules with defaults
            var transformations = new List<TextTransformation>()
            {
                // Replace "ll" with "wl"
                new(new Regex(@"ll", RegexCI, DefaultTimeout), m => MatchCase(m.Value, "wl")),

                // Replace "r" with "w"
                new(new Regex(@"r", RegexCI, DefaultTimeout), m => MatchCase(m.Value, "w")),

                // Replace "th" with "dw"
                new(new Regex(@"th", RegexCI, DefaultTimeout), m => MatchCase(m.Value, "dw")),

                // Insert a "y" between "n" and vowels
                new(
                    new Regex(@"(n)([aeiou])", RegexCI, DefaultTimeout), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel = m.Groups[2].Value;
                        return letter + MatchCase(vowel, "y") + vowel;
                    }
                ),

                // Insert a "w" between "f" and vowels
                new(
                    new Regex(@"(f)([aeiou])", RegexCI, DefaultTimeout), m =>
                    {
                        var letter = m.Groups[1].Value;
                        var vowel = m.Groups[2].Value;
                        return letter + MatchCase(vowel, "w") + vowel;
                    }
                ),

                // Insert a "w" between vowels and "d"
                new(
                    new Regex(@"([aeiou])(d)", RegexCI, DefaultTimeout), m =>
                    {
                        var vowel = m.Groups[1].Value;
                        var letter = m.Groups[2].Value;
                        return vowel + MatchCase(letter, "w") + letter;
                    }
                ),
            };


            // Make curse words cuter, if enabled
            if (uwuOptions.MakeCuteCurses)
            {
                transformations.Add(
                    new TextTransformation(
                        new Regex(@"(f|b|sh)(uck|itch|it)", RegexCI, DefaultTimeout), m =>
                        {
                            var start = m.Groups[1].Value;
                            var end = m.Groups[2].Value;
                            return start + MatchCase(end, "w") + end;
                        }
                    )
                );

                transformations.Add(
                    new TextTransformation(
                        new Regex(@"(d)(amn)", RegexCI, DefaultTimeout), m =>
                        {
                            var start = m.Groups[1].Value;
                            var end = m.Groups[2].Value;
                            return start + MatchCase(end, "y") + end;
                        }
                    )
                );
            }

            // Add trailing UwU, if enabled
            if (uwuOptions.AppendUwu)
            {
                transformations.Add(new TextTransformation(new Regex(@"$", RegexOptions.Compiled, DefaultTimeout), _ => " UwU!"));
            }

            return transformations;
        }
    }
}