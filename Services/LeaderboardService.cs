using JuegoColores.Models;
using System.Text.Json;

namespace JuegoColores.Services
{
    public class LeaderboardService
    {
        private readonly string _filePath = "leaderboard.json";

        public List<LeaderboardEntry> GetEntries()
        {
            if (!File.Exists(_filePath)) return new List<LeaderboardEntry>();

            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new List<LeaderboardEntry>();
        }

        public void AddEntry(LeaderboardEntry entry)
        {
            var entries = GetEntries();
            entries.Add(entry);
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void UpdateAnnotation(string id, string annotation)
        {
            var entries = GetEntries();
            var entry = entries.FirstOrDefault(e => e.Id == id);
            if (entry != null)
            {
                entry.Annotation = annotation;
                var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }
    }
}
