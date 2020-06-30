using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;
using System.IO;

namespace TerraNPCBot.Program {
    public class Config {
        public string DBConnectionString;
        public int DefaultBotLimit;

        public void Save() {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Load() {
            Config toReturn;

            if (!File.Exists(ConfigPath)) {
                toReturn = CreateDefault();
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(CreateDefault(), Formatting.Indented));
            }
            else {
                toReturn = JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath));
            }

            return toReturn;
        }

        private static Config CreateDefault() {
            return new Config {
                DBConnectionString = $"mongodb://localhost:27017",
                DefaultBotLimit = 1
            };
        }

        private static string ConfigPath => Path.Combine(PluginMain.PluginFolderLocation, "TerraNPCBotConfig.json");
    }
}
