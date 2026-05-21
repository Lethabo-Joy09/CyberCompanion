using System.Collections.Generic;

namespace CyberCompanion
{
    /// <summary>
    /// Simple in-memory key-value store to remember user information
    /// across conversation turns. Used for name, favourite topic, etc.
    /// </summary>
    public class MemoryStore
    {
        // Dictionary to hold memory items (case-sensitive keys)
        private readonly Dictionary<string, string> _data = new();

        /// <summary>Store a value under a specific key.</summary>
        public void Set(string key, string value)
        {
            _data[key] = value;
        }

        /// <summary>Retrieve a value by key. Returns null if not found.</summary>
        public string? Get(string key)
        {
            _data.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>Check if a key exists.</summary>
        public bool Has(string key) => _data.ContainsKey(key);

        /// <summary>Clear all stored memory.</summary>
        public void Clear() => _data.Clear();

        /// <summary>Get a copy of all stored data (for display).</summary>
        public Dictionary<string, string> GetAll() => new(_data);
    }
}
