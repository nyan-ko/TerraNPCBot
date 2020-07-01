using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class BotInventory : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => throw new NotImplementedException();

        public override string ExampleMessage => throw new NotImplementedException();

        public override string InitialPermission => throw new NotImplementedException();

        protected override void Execute(BotCommandArgs args) {
            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            int count = currentSection.Count;

            if (count == 0) {

            }
            else {
                List<string> itemRows = new List<string>();
                switch (currentSection[1]) {
                    case "inventory":

                        break;
                    case "armor":
                    case "accessories":
                    case "accessory":

                        break;
                    case "vanity":
                    case "social":

                        break;
                    case "misc":

                        break;
                }
            }
        }
    }
}
