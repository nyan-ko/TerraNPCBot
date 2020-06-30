using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Ignore : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.Ignore;

        public override string ExampleMessage => Messages.NoExample;

        public override string InitialPermission => Permissions.BotIgnore;

        protected override void Execute(BotCommandArgs args) {
            args.BPlayer.ToggleBotVisibility(args.BPlayer.IgnoreBots);
            args.Player.SendInfoMessage($"You are {(args.BPlayer.IgnoreBots ? "now" : "no longer")} ignoring server bots.");
        }
    }
}
