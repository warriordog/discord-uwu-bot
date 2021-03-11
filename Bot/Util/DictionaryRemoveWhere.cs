using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordUwuBot.Bot.Util
{
    public static class DictionaryRemoveWhere
    {
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