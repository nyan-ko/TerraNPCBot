using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using Microsoft.Xna.Framework;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class DeleteBot : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => Messages.Delete;

        public override string ExampleMessage => Messages.DeleteExample;

        public override string InitialPermission => Permissions.BotCreate;

        protected override void Execute(BotCommandArgs args) {
            if (args.BPlayer.OwnedBots.Count == 0) {
                args.Player?.SendErrorMessage(Messages.NoOwnedBots);
                return;
            }

            BTSPlayer bPlayer = args.BPlayer;
            TSPlayer sPlayer = args.Player;
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1 && bPlayer.Selected != -1) {
                // Default selected bot
                bPlayer.SetSelectedDelete(bPlayer.Selected);
                sPlayer?.SendSuccessMessage("Currently selected bot will be deleted upon confirmation: /confirm or /deny.");

                sPlayer?.AddResponse("confirm", new Action<object>(ConfirmedDelete));
                sPlayer?.AddResponse("deny", new Action<object>(RefuseDelete));
            }
            else if (currentSection.Count > 1) {
                // User specified bot
                string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));
                List<Bot> foundbots = bPlayer.GetBotFromIndexOrName(nameOrIndex);

                if (!sPlayer.HandleListFromSearches(nameOrIndex, foundbots))
                    return;
                bPlayer.SetSelectedDelete(foundbots[0].IndexInOwnerBots);
                sPlayer?.SendSuccessMessage($"Selecting bot \"{foundbots[0].Name}\" with index {foundbots[0].IndexInOwnerBots} to delete: /confirm or /deny.");

                sPlayer?.AddResponse("confirm", new Action<object>(ConfirmedDelete));
                sPlayer?.AddResponse("deny", new Action<object>(RefuseDelete));
            }
            else {
                sPlayer?.SendMultipleMessage(Messages.Delete, Color.Yellow);
            }
        }

        private static void ConfirmedDelete(object obj) {
            BotCommandArgs args = new BotCommandArgs((CommandArgs)obj);

            var player = args.BPlayer;
            player.SelectedBot.Shutdown();
            player.OwnedBots.RemoveAt(player.GetSelectedDelete());
            player.SPlayer?.SendSuccessMessage("Successfully deleted bot.");
        }

        private static void RefuseDelete(object obj) {
            BotCommandArgs args = new BotCommandArgs((CommandArgs)obj);

            // Remove /confirm from awaiting responses
            args.Player.AddResponse("confirm", null);
            args.BPlayer.SetSelectedDelete(-1);
        }
    }
}
