using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DiscordUwuBot.UwU
{
    /// <summary>
    /// Converts text into UwU-speak
    /// </summary>
    public interface ITextUwuifier
    {
        /// <summary>
        /// Transform a snippet of English text into UwU-speak.
        /// </summary>
        /// <remarks>
        /// The exact rules used to convert text are implementation-specific.
        /// They should, however, be designed for (or at least support) English inputs.
        /// Implementations are not required to support non-english text.
        /// </remarks>
        /// <example>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Input</term>
        ///         <term>Output</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Hello, world!</term>
        ///         <term>Hewlo, wowld! UwU!</term>
        ///     </item>
        ///     <item>
        ///         <term>Lorem ipsum dolar sit amet</term>
        ///         <term>Lowem ipsum dolaw sit amet UwU!</term>
        ///     </item>
        /// </list>
        /// </example>
        /// <param name="text">Text to transform</param>
        /// <returns>Text translated to UwU</returns>
        public string UwuifyText(string text);
    }

    /// <summary>
    /// A conditional text transformation.
    /// If <see cref="MatchRegex"/> matches, then <see cref="MatchReplacer"/> is called to transform the matched substring. 
    /// </summary>
    public record TextTransformation(Regex MatchRegex, MatchEvaluator MatchReplacer);
    
    /// <summary>
    /// Default implementation of <see cref="ITextUwuifier"/>
    /// </summary>
    public class TextUwuifier : ITextUwuifier
    {
        /// <summary>
        /// Collection of <see cref="TextTransformation"/> that are applied to transform English text into UwU-speak.
        /// </summary>
        public IEnumerable<TextTransformation> UwuReplacements { get; }

        /// <summary>
        /// Create a new TextUwuifier with a custom set of transformations.
        /// </summary>
        /// <param name="uwuReplacements">Custom set of text transformations to use with this instance</param>
        public TextUwuifier(IEnumerable<TextTransformation> uwuReplacements)
        {
            UwuReplacements = uwuReplacements;
        }

        /// <summary>
        /// Create a new TextUwuifier with the default transformations.
        /// Default transformations are provided by <see cref="GetDefaultUwuReplacements"/>.
        /// </summary>
        public TextUwuifier()
        {
            UwuReplacements = GetDefaultUwuReplacements();
        }
        
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
        
        /// <summary>
        /// Combined RegexOptions for Compiled and Case-insensitive.
        /// </summary>
        private const RegexOptions RegexCI = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        
        /// <summary>
        /// Default timeout for matching regular expressions.
        /// </summary>
        private static readonly TimeSpan DefaultTimeout = new TimeSpan(0, 0, 0, 0, 25);
        
        /// <summary>
        /// Gets the set of default text transformations that convert English to UwU-speak.
        /// </summary>
        /// <remarks>
        /// The default transformations are:
        /// <list type="bullet">
        ///     <item><description>ll => wl</description></item>
        ///     <item><description>r => w</description></item>
        ///     <item><description>th => dw</description></item>
        ///     <item><description>n(vowel) => ny(vowel)</description></item>
        ///     <item><description>f(vowel) => w(vowel)</description></item>
        ///     <item><description>(vowel)d => (vowel)wd</description></item>
        ///     <item><description>fuck|bitch|shit => fwuck|bwitch|shwit</description></item>
        ///     <item><description>damn => dyamn</description></item>
        /// </list>
        /// </remarks>
        /// <returns>The collection of <see cref="TextTransformation"/> that implement the default transformation logic.</returns>
        public static IEnumerable<TextTransformation> GetDefaultUwuReplacements()
        {
            return new TextTransformation[] {
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