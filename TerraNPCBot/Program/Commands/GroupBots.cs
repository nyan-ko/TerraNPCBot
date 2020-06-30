using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class GroupBots : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.GroupBots;

        public override string ExampleMessage => Messages.GroupBotsExample;

        public override string InitialPermission => Permissions.BotForeach;

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            List<int> selected = new List<int>();
            if (currentSection.Count == 1) {
                for (int i = 0; i < player.OwnedBots.Count; ++i) {
                    selected.Add(i);
                }

                args.Player?.SendSuccessMessage("Selected all owned bots.");
            }
            else {
                foreach (var str in currentSection.Skip(1)) {
                    if (!int.TryParse(str, out int selection)) {
                        args.Player?.SendErrorMessage($"Invalid syntax, expected integer index of bot but got \"{str}\" instead.");
                        return;
                    }
                    selected.Add(selection);
                }
                args.Player?.SendSuccessMessage("Selected specified bots.");
            }
            if (selected.Count != 0 && player.GroupedBots.Count != 0)
                player.GroupedBots.Clear();
            player.GroupedBots.AddRange(selected);
        }
    }
}
