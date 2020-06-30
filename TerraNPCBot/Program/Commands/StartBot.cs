using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class StartBot : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => Permissions.Bot;

        protected override void Execute(BotCommandArgs args) {
            var bot = args.SelectedBot;
            if (bot == null) {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
            else if (bot.Running) {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorAlreadyRunning, bot.Name));
            }
            else if (!bot.Start()) {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorCouldNotStart, bot.Name));
            }
            else {
                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStarted, bot.Name));
            }
        }
    }
}
