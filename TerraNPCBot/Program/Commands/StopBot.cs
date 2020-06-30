using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class StopBot : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => Permissions.Bot;

        protected override void Execute(BotCommandArgs args) {
            var bot = args.SelectedBot;
            if (bot != null) {
                if (bot.Running) {
                    bot.Shutdown();
                    args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStopped, bot.Name));
                }
                else {
                    args.Player?.SendErrorMessage(string.Format(Messages.BotErrorNotRunning, bot.Name));
                }
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
    }
}
