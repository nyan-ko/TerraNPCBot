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
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1) {
                args.Player.MultiMsg(Messages.Master1, Color.Yellow);
            }
            else if (currentSection.Count == 2) {
                switch (currentSection[1]) {
                    case "1":
                        args.Player.MultiMsg(Messages.Master1, Color.Yellow);
                        break;
                    case "2":
                        args.Player.MultiMsg(Messages.Master2, Color.Yellow);
                        break;
                    case "3":
                        args.Player.MultiMsg(Messages.Master3, Color.Yellow);
                        break;
                    default:
                        args.Player.MultiMsg(Messages.Master1, Color.Yellow);
                        break;
                }
            }
            else {
                args.Player.MultiMsg(Messages.Master1, Color.Yellow);
            }
        }
    }
}
