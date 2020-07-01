using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Utils;
using Microsoft.Xna.Framework;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Info : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => string.Empty;

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1) {
                args.Player.SendMultipleMessage(Color.Yellow, $"Bot limit: {player.BotLimit}",
                    $"Currently owned bots: {player.OwnedBots.Count}",
                    $"Currently selected bot: {player.SelectedBot?.Name ?? "none"}",
                    $"Allow bot copying: {player.CanBeCopied}",
                    $"Allow bot teleport: {player.CanBeTeleportedTo}");
            }
            else if (currentSection.Count > 2) {
                if (player.OwnedBots.Count == 0) {
                    args.Player.SendErrorMessage(Messages.NoOwnedBots);
                    return;
                }
                string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));
                var bots = player.GetBotFromIndexOrName(nameOrIndex);
                if (!args.Player.HandleListFromSearches(nameOrIndex, bots))
                    return;
                Bot foundBot = bots[0];
                if (!PluginMain.ServersByPorts.TryGetValue(foundBot.Port.ToString(), out string serverName)) {
                    serverName = "none..?";
                }
                args.Player.SendMultipleMessage(Color.Yellow, $"Bot name: {foundBot.Name}",
                    $"Bot running: {foundBot.Running}",
                    $"Bot index: {foundBot.ID}",
                    $"Bot index in owned bots: {foundBot.IndexInOwnerBots}",
                    $"Bot position: ({foundBot.TilePosition.X}, {foundBot.TilePosition.Y})",
                    $"Bot server: {serverName}");
            }
        }
    }
}
