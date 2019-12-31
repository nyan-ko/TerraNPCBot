using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace rt.Program {
    public class PluginCommands {

        public static void BotMaster(CommandArgs args) {
            if (args.Parameters.Count > 0) {
                switch (args.Parameters[0]) {
                    case "new":
                        NewBot(args);
                        break;
                    case "start":
                        StartBot(args);
                        break;
                    case "stop":
                        StopBot(args);
                        break;
                    case "record":
                        if (args.Parameters.Count > 1) {
                            switch (args.Parameters[1]) {
                                case "start":
                                    Record(args);
                                    break;
                                case "stop":
                                    StopRecording(args);
                                    break;
                                case "play":
                                    Play(args);
                                    break;
                            }
                        } // Flag101
                        break;
                    case "delete":
                        // Flag102
                        break;
                }
            }
        }

        static void NewBot(CommandArgs args) {
            BTSPlayer bp = Program.Players[args.Player.Index];
            Bot bot = args.Parameters.Count > 0
                ? new Bot("127.0.0.1", args.Player.Index, args.Parameters[0])
                : new Bot("127.0.0.1", args.Player.Index);
            bp._ownedBots.Add(bot);
            bp._selected = bp._ownedBots.Count - 1;

            // something to prevent players going over their limit Flag102
            // Flag101 Flag104

            args.Player.SendMessage($"Created a new bot with name \"{bot._player.Name}\".", Color.Yellow);
        }

        static void StartBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null || !bot.Start()) {
                args.Player.SendErrorMessage("Something went wrong. Retry?");  // more specific error messages later Flag101
            } // Flag104
        }        

        static void StopBot(CommandArgs args) {
            if (Program.Players[args.Player.Index]?.SelectedBot != null) {
                Program.Players[args.Player.Index].SelectedBot.Stop(null);
            } // Flag104
            else {
                // Flag101
            }
        }

        static void Delegation(CommandArgs args) {

        }  // Flag103

        // check to prevent multiple bots recording Flag102
        static void Record(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = true;
                bot._recordedPackets = new List<RecordedPacket>();
            } // Flag104 Flag101
        }

        static void StopRecording(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = false;
            } // Flag104 Flag101
        }

        static void Play(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null && bot._recordedPackets.Count > 0) {
                foreach (var p in bot._recordedPackets) {
                    var packet = ParsedPacketBase.Write(p.packet);
                    if (packet == null) continue;
                    bot._client.AddPackets(packet);
                    Thread.Sleep(p.timeBeforeNextPacket);
                }
            } // Flag101 Flag104
        }
    }
}
