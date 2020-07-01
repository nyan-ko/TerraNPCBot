using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Program;

namespace TerraNPCBot.Utils {
    public static class PluginUtils {

        public static string GetAvailableCommands(TShockAPI.TSPlayer plr) {
            List<string> availableCommands = new List<string>();
            foreach (var command in PluginCommands.CommandsByTag) {
                if (plr.HasPermission(command.Value.InitialPermission))
                    availableCommands.Add(command.Key);
            }
            return availableCommands.ToString();
        }

        public static bool PortSpecified(string potentialPort, out int port) {
            bool containsPort = false;
            if (PluginMain.ServersByPorts.ContainsKey(potentialPort)) {
                containsPort = true;
                port = int.Parse(potentialPort);
            }
            else if (PluginMain.PortsByServers.TryGetValue(potentialPort.ToLower(), out port))
                containsPort = true;

            return containsPort;
        }
    }
}
