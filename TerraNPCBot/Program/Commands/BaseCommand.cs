using System;
using System.Collections.Generic;
using TerraNPCBot.Data;
using TShockAPI;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Program.Commands {
    public abstract class BaseCommand {
        public abstract bool ValidForeachAction { get; }

        public abstract string HelpMessage { get; }

        public abstract string ExampleMessage { get; }

        public abstract string InitialPermission { get; }

        public void Invoke(BotCommandArgs args) {
            if (InitialPermission != string.Empty && !args.Player.HasPermission(InitialPermission)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }
            if (HelpMessage != string.Empty && ExampleMessage != string.Empty && NeedsHelpOrExample(args, HelpMessage, ExampleMessage))
                return;

            Execute(args);
        }

        protected abstract void Execute(BotCommandArgs args);

        private static bool NeedsHelp(List<string> args) {
            return args.Exists(x => x.ToLower() == "help");
        }

        private static bool NeedsExample(List<string> args) {
            return args.Exists(x => x.ToLower() == "example");
        }

        private static bool NeedsHelpOrExample(BotCommandArgs args, string helpmsg, string examplemsg) {
            bool help = NeedsHelp(args.CurrentSection);
            bool example = NeedsExample(args.CurrentSection);

            if (help) {
                args.Player.MultiMsg(helpmsg, Color.Orange);
            }
            else if (example) {
                args.Player.MultiMsg(examplemsg, Color.Orange);
            }

            return help || example;
        }
    }
}
