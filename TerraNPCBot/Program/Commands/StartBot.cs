using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class StartBot : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => Permissions.Bot;

        protected override void Execute(BotCommandArgs args) {
            TSPlayer sPlayer = args.Player;
            var bot = args.SelectedBot;
            if (bot == null) {
                sPlayer?.SendErrorMessage(Messages.BotErrorNotFound);
            }
            else if (bot.Running) {
                sPlayer?.SendErrorMessage(string.Format(Messages.BotErrorAlreadyRunning, bot.Name));
            }
            else if (!bot.Start()) {
                sPlayer?.SendErrorMessage(string.Format(Messages.BotErrorCouldNotStart, bot.Name));
            }
            else {
                sPlayer?.SendSuccessMessage(string.Format(Messages.BotSuccessStarted, bot.Name));
            }
        }
    }
}
