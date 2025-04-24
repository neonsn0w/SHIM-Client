using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;    

namespace ClientGUI_MultipleClientsChatTest
{
    public class ConcurrentDictionarySerializer<TKey, TValue>
    {
        public void SaveToFile(ConcurrentDictionary<TKey, TValue> dictionary, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(dictionary, options);
            File.WriteAllText(filePath, json);
        }

        public ConcurrentDictionary<TKey, TValue> LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var dictionary = JsonSerializer.Deserialize<ConcurrentDictionary<TKey, TValue>>(json);
            return dictionary ?? new ConcurrentDictionary<TKey, TValue>();
        }
    }

}
