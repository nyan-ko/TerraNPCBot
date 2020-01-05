using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rt.Utils;
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
                                default:
                                    args.Player.MultiMsg(Messages.Record, Color.Yellow);
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
            Bot bot;
            try {
                BTSPlayer bp = Program.Players[args.Player.Index];
                bot = args.Parameters.Count > 1
                    ? new Bot("127.0.0.1", args.Player.Index, args.Parameters[1])
                    : new Bot("127.0.0.1", args.Player.Index);
                bp._ownedBots.Add(bot);
                bp._selected = bp._ownedBots.Count - 1;
            }
            catch (Exception ex) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorGeneric, ex.ToString()));
                return;
            }

            // something to prevent players going over their limit Flag102

            args.Player.SendMessage(string.Format(Messages.BotSuccessCreateNew, bot.Name), Color.Yellow);
        }

        static void StartBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null) {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
            else if (bot.Running) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorAlreadyRunning, bot.Name));
                return;
            }
            else if (!bot.Start()) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorCouldNotStart, bot.Name));
            }
            else {
                args.Player.SendMessage(string.Format(Messages.BotSuccessStarted, bot.Name), Color.Green);
            }
        }        

        static void StopBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot.Running) {
                    bot.Stop(null);

                    args.Player.SendMessage(string.Format(Messages.BotSuccessStopped, bot.Name), Color.Green);
                }
                else {
                    args.Player.SendMessage(string.Format(Messages.BotErrorNotRunning, bot.Name), Color.Red);
                }
            } 
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
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
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        static void StopRecording(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = false;
            } // Flag104 Flag101
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        static void Play(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null && bot._recordedPackets.Count > 0) {
                bot._delayBetweenPackets = new System.Timers.Timer(10);
                bot._delayBetweenPackets.Elapsed += bot.RecordedPacketDelay;
                bot._delayBetweenPackets.AutoReset = true;
                bot._delayBetweenPackets.Start();
            } // Flag101 Flag104
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
    }
}
