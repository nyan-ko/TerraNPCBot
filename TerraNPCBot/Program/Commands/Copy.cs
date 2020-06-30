using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Copy : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => Messages.Copy;

        public override string ExampleMessage => Messages.CopyExample;

        public override string InitialPermission => Permissions.BotCopy;

        protected override void Execute(BotCommandArgs args) {
            TSPlayer tstarget;
            List<string> currentSection = args.CurrentSection;

            var bot = args.SelectedBot;
            if (bot == null) {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
                return;
            }

            if (currentSection.Count > 1) {
                if (!args.Player.HasPermission(Permissions.BotCopyOther)) {
                    args.Player?.SendErrorMessage("You do not have permission to copy other players.");
                    return;
                }

                string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));

                var found = TSPlayer.FindByNameOrID(nameOrIndex);
                if (!args.Player.HandleListFromSearches(nameOrIndex, found))
                    return;
                tstarget = found[0];
            }
            else {
                tstarget = args.Player;
            }

            if (tstarget.Index != args.Player.Index &&
                !PluginMain.Players[tstarget.Index].CanBeCopied
                && !args.Player.HasPermission(Permissions.BotBypassCopy)) {
                args.Player?.SendErrorMessage("This player has disabled inventory copying.");
                return;
            }

            bot.Actions.FullCopy(tstarget.TPlayer);

            args.Player?.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now copying \"{tstarget.Name}\".");
        }
    }
}
