using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Utils;
using Microsoft.Xna.Framework;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Help : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => string.Empty;

        public override string ExampleMessage => string.Empty;

        public override string InitialPermission => string.Empty;

        protected override void Execute(BotCommandArgs args) {
            args.Player?.SendMultipleMessage(Messages.Master, Color.Yellow);
        }
    }
}
