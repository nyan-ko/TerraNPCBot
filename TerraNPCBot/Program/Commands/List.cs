using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class List : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => string.Empty;

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer target = null;
            if (args.Parameters.Count == 1) {
                target = args.Player.ToBTSPlayer();
                args.Player.SendInfoMessage("Listing bots for yourself...");
            }
            else {
                List<string> currentSection = args.CurrentSection;

                string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));

                var found = TSPlayer.FindByNameOrID(nameOrIndex);

                if (args.Player?.HandleListFromSearches(nameOrIndex, found) ?? false)
                    return;
                target = found[0].ToBTSPlayer();
                args.Player.SendInfoMessage($"Listing bots for {found[0].Name}...");
            }

            if (target.OwnedBots.Count == 0) {
                args.Player.SendInfoMessage("Target does not have any bots.");
                return;
            }
            foreach (Bot bot in target.OwnedBots) {
                args.Player.SendInfoMessage($"[{bot.IndexInOwnerBots}] {bot.Name} | Currently running: {bot.Running}");
            }
            args.Player.SendInfoMessage($"Owned bots: ({target.OwnedBots.Count}/{target.BotLimit})");
        }
    }
}
