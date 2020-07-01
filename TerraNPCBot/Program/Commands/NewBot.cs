using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class NewBot : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.New;

        public override string ExampleMessage => Messages.NewExample;

        public override string InitialPermission => Permissions.BotCreate;

        protected override void Execute(BotCommandArgs args) {
            if (args.BPlayer.OwnedBots.Count + 1 > args.BPlayer.BotLimit) {
                args.Player?.SendErrorMessage($"You have reached the maximum number of bots you can create: {args.BPlayer.BotLimit}");
                return;
            }

            BTSPlayer bp = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            string name = "Michael Jackson";

            if (currentSection.Count > 1) {
                name = StringUtils.CreateBotName(currentSection.Skip(1), args.Player);
            }

            Bot bot = new Bot(name, args.Player.Index, bp.OwnedBots.Count);

            bp.OwnedBots.Add(bot);
            bp.Selected = bp.OwnedBots.Count - 1;

            args.Player?.SendInfoMessage($"Created a new bot named \"{name}\".");
        }
    }
}
