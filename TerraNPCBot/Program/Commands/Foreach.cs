using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Foreach : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.Foreach;

        public override string ExampleMessage => Messages.ForeachExample;

        public override string InitialPermission => Permissions.BotForeach;

        protected override void Execute(BotCommandArgs args) {

            // Example command
            // /bot foreach (copy "Cy")
            // All selected bots with /groupbots now copy cy

            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;

            if (player.GroupedBots.Count == 0) {
                args.Player?.SendErrorMessage("No grouped bots found. Use the /group subcommand.");
                return;
            }
            if (currentSection.Count == 1) {
                args.Player?.SendErrorMessage("No actions for bots found.");
                return;
            }
            string message = StringUtils.JoinAndTrimList(currentSection);
            List<string> parameters = StringUtils.JoinAndTrimList(currentSection.Skip(1)).Trim('(', ')', ' ').Split(' ').ToList();
            TSPlayer tsplayer = args.Player;

            foreach (var selected in player.GroupedBots) {
                PluginCommands.AddInternalCommand(new BotCommandArgs(
                    new CommandArgs(message, tsplayer, parameters), true).SetSelectedBotOverride(player.OwnedBots[selected]));
            }
        }
    }
}
