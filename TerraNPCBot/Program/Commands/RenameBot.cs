using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class RenameBot : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => Messages.Rename;

        public override string ExampleMessage => Messages.RenameExample;

        public override string InitialPermission => Permissions.Bot;

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer bp = args.BPlayer;
            List<string> currentSection = args.CurrentSection;

            var bot = args.SelectedBot;
            if (bot == null) {
                args.Player?.MultiMsg(Messages.BotErrorNotFound, Color.Yellow);
                return;
            }

            string name = "Michael Jackson";
            if (currentSection.Count == 1) {
                args.Player?.SendInfoMessage("Resetting bot name to Michael Jackson.");
            }
            else {
                name = StringUtils.CreateBotName(currentSection.Skip(1), args.Player);
            }

            args.Player?.SendSuccessMessage($"Changing selected bot's name to \"{name}\".");
            bot.Name = name;
        }
    }
}
