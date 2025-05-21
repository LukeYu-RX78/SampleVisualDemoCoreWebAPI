using System;
using System.Collections.Generic;
using System.IO;

namespace SampleVisualDemoCoreWebAPI.Utils
{
    public static class EmailTemplateLoader
    {
        public static string LoadAndFormat(string relativePath, Dictionary<string, string> replacements)
        {
            string baseDir = AppContext.BaseDirectory;
            string fullPath = Path.Combine(baseDir, relativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("Email template not found", fullPath);

            string content = File.ReadAllText(fullPath);

            foreach (var kvp in replacements)
            {
                content = content.Replace("{" + kvp.Key + "}", kvp.Value);
            }

            return content;
        }
    }
}