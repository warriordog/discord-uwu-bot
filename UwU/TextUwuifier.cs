using System.Linq;

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
    /// Default implementation of <see cref="ITextUwuifier"/>
    /// </summary>
    public class TextUwuifier : ITextUwuifier
    {
        /// <summary>
        /// Collection of <see cref="TextTransformation"/> that are applied to transform English text into UwU-speak.
        /// </summary>
        private IUwuRules UwuRules { get; }
        
        public TextUwuifier(IUwuRules uwuRules)
        {
            UwuRules = uwuRules;
        }        
        
        public string UwuifyText(string text) => UwuRules.UwuTransformations.Aggregate(text, 
        (current, replacement) => replacement.MatchRegex.Replace(current, replacement.MatchReplacer)
        );
    }
}