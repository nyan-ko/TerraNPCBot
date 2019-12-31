using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerrariaApi.Server;
using Terraria;
using System.Net;
using System.Net.Sockets;

namespace rt
{
    [ApiVersion(2, 1)]
    public class Program : TerrariaPlugin {

        Bot bot;

        public Program(Main game) : base(game) {

        }

        public override void Initialize() {
            Commands.ChatCommands.Add(new Command("", Start, "startbot"));
            Commands.ChatCommands.Add(new Command("", Stop, "stopbot"));

            Commands.ChatCommands.Add(new Command("", Delegation, "s"));
            Commands.ChatCommands.Add(new Command("", Record, "e"));

        }

        void Start(CommandArgs args) {
            bot = new Bot("127.0.0.1", args.Player.Index);
            if (!bot.Start()) {
                args.Player.SendErrorMessage("Something went wrong. Retry?");
            }
        }

        void Stop(CommandArgs args) {
            bot.Stop(null, null);
        }

        void Delegation(CommandArgs args) {
            
        }

        void Record(CommandArgs args) {

        }
    }
}
