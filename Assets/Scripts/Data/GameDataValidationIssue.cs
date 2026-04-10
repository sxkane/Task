using System;

namespace Data
{
    [Serializable]
    public class GameDataValidationIssue
    {
        public string Category;
        public string SourceName;
        public string Message;

        public GameDataValidationIssue(string category, string sourceName, string message)
        {
            Category = category;
            SourceName = sourceName;
            Message = message;
        }

        public override string ToString()
        {
            return $"[{Category}] {SourceName}: {Message}";
        }
    }
}
