using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordUwuBot.Bot.Util
{
    public static class DictionaryRemoveWhere
    {
        /// <summary>
        /// Filters a dictionary to remove all entries that match a predicate.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.HashSet{T}.RemoveWhere"/>
        /// <param name="dictionary">Dictionary to remove from</param>
        /// <param name="match">Predicate to match entries</param>
        /// <typeparam name="TKey">Type of key</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <returns>Returns the original dictionary that was passed in</returns>
        public static Dictionary<TKey, TValue> RemoveWhere<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> match)
        {
            dictionary
                // Get all matching keys
                .Where(entry => match(entry))
                    
                // Convert to list to avoid double enumeration
                .ToList()
                    
                // Remove from dictionary
                .ForEach(entry => dictionary.Remove(entry.Key));

            // Return dictionary for chaining purposes
            return dictionary;
        }
    }
}