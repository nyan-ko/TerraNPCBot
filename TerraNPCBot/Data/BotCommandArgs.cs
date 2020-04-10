using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TerraNPCBot.Data {
    public class BotCommandArgs : CommandArgs {

        public Bot SelectedBot {
            get {
                if (Player.RealPlayer)
                    return Program.Program.Players[Player.Index]?.SelectedBot;
                return BTSPlayer.BTSServerPlayer.SelectedBot;
            }
        }

        public BTSPlayer BPlayer {
            get {
                if (Player.RealPlayer)
                    return Program.Program.Players[Player.Index];
                return BTSPlayer.BTSServerPlayer;
            }
        }

        public BotCommandArgs(CommandArgs baseCommand) : base(baseCommand.Message, baseCommand.Player, baseCommand.Parameters) {

        }
    }

}
