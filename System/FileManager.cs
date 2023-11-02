using ProjectArrow.Helpers;
using System.IO;
using System.Text;
using System.Text.Json;
using static System.Environment;

namespace ProjectArrow.System
{
    public static class FileManager
    {

        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };

        public static string gameFolderPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData), "ProjectArrow");
        public static string saveFilePath = Path.Combine(gameFolderPath, "settings.json");

        public static Settings CurrentSettings { get; private set; }

        public static void Initialize()
        {

            if (!Directory.Exists(gameFolderPath))
            {
                Directory.CreateDirectory(gameFolderPath);
            }

            if (!File.Exists(saveFilePath))
            {
                //Default Save Settings
                Settings settings = new()
                {
                    UIScale = 2,
                    Zoom = 2,
                    Vsync = false
                };

                using (FileStream fs = new FileStream(saveFilePath, FileMode.CreateNew))
                {
                    var jsonString = JsonSerializer.Serialize(settings, _options);
                    byte[] info = new UTF8Encoding(true).GetBytes(jsonString);
                    fs.Write(info, 0, info.Length);
                }
            }

            if (File.Exists(saveFilePath))
            {
                using (var stream = File.Open(saveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var jsonString = File.ReadAllText(saveFilePath);
                    CurrentSettings = JsonSerializer.Deserialize<Settings>(jsonString, _options);
                }
            }
        }


        public static void SaveSettings()
        {
            Settings settings = new()
            {
                UIScale = GameData.UIScale,
                Zoom = GameData.CurrentZoom,
                Vsync = GameData.AllowVysnc
            };

            using (FileStream fs = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
            {
                var jsonString = JsonSerializer.Serialize(settings, _options);
                byte[] info = new UTF8Encoding(true).GetBytes(jsonString);
                fs.Write(info, 0, info.Length);
            }
        }
    }
}
