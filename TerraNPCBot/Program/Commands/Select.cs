using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Select : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.Select;

        public override string ExampleMessage => Messages.SelectExample;

        public override string InitialPermission => Permissions.Bot;

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;

            if (player.OwnedBots.Count == 0) {
                args.Player?.SendErrorMessage(Messages.NoOwnedBots);
                return;
            }

            if (currentSection.Count > 1) {
                string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));
                List<Bot> foundbots = player.GetBotFromIndexOrName(nameOrIndex);

                if (!args.Player.HandleListFromSearches(nameOrIndex, foundbots))
                    return;
                player.Selected = foundbots[0].IndexInOwnerBots;
                args.Player?.SendSuccessMessage($"Selected bot \"{foundbots[0].Name}\" with index {foundbots[0].IndexInOwnerBots}.");
            }
            else {
                args.Player.SendMultipleMessage(Messages.Select, Color.Yellow);
            }
        }
    }
}
