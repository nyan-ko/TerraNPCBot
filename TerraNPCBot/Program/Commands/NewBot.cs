using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class NewBot : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.New;

        public override string ExampleMessage => Messages.NewExample;

        public override string InitialPermission => Permissions.BotCreate;

        protected override void Execute(BotCommandArgs args) {
            if (args.BPlayer.OwnedBots.Count + 1 > args.BPlayer.BotLimit) {
                args.Player?.SendErrorMessage($"You have reached the maximum number of bots you can create: {args.BPlayer.BotLimit}");
                return;
            }

            BTSPlayer bp = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            string name = "Michael Jackson";
            int port = 7777;

            // If the user has specified the name and or port for the bot.
            if (currentSection.Count > 1) {
                // The last parameter which may or may not be the port
                string potentialPort = currentSection[currentSection.Count - 1];
                // Whether the user has specified a port, which is present in the last parameter
                bool hasSpecifiedPort = PluginUtils.PortSpecified(potentialPort, out port);
                // Gets the range in the parameter list that contains the user specified name.
                // If there is a port, get the range between 'new' and the port
                // Else, skip 'new' in the current secton to get the range.
                List<string> nameRange = hasSpecifiedPort ? currentSection.GetRange(1, currentSection.Count - 2) : currentSection.Skip(1).ToList();

                // Determine bot name
                string tempName = StringUtils.CreateBotName(nameRange, args.Player);

                // Notify user they either did not specify a port or they gave an invalid port.
                if (!hasSpecifiedPort) {
                    args.Player.SendInfoMessage("No specified port detected or an invalid port was given. Defaulting to 7777 (Nexus).");
                }
            }

            Bot bot = new Bot(name, args.Player.Index, bp.OwnedBots.Count, port);

            bp.OwnedBots.Add(bot);
            bp.Selected = bp.OwnedBots.Count - 1;

            args.Player?.SendInfoMessage($"Created a new bot named \"{name}\".");
        }
    }
}
