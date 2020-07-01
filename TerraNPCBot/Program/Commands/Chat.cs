using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Chat : BaseCommand {
        public override string HelpMessage => Messages.Copy;

        public override string ExampleMessage => Messages.CopyExample;

        public override string InitialPermission => Permissions.BotChat;

        public override bool ValidForeachAction => true;

        protected override void Execute(BotCommandArgs args) {
            var bot = args.SelectedBot;
            TSPlayer sPlayer = args.Player;
            List<string> currentSection = args.CurrentSection;
            if (bot != null) {
                if (currentSection.Count == 1) {
                    sPlayer?.SendErrorMessage("Expected message as input.");
                    return;
                }
                var message = StringUtils.JoinAndTrimList(currentSection.Skip(1));

                if (message.Length > 30) {
                    sPlayer?.SendErrorMessage("Message exceeded 30 character limit.");
                    return;
                }

                bot.Actions.Chat(message);
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
    }
}
