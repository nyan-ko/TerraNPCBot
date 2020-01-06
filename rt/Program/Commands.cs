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
                    case "copy":
                        Copy(args);
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

        static void Copy(CommandArgs args) {
            TSPlayer tstarget;
            if (args.Parameters.Count > 1) {
                string namewithspaces = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                namewithspaces = namewithspaces.Trim('"');

                var found = TShock.Utils.FindPlayer(namewithspaces);
                if (found.Count > 1) {
                    string multiple = string.Join(", ", found);
                    args.Player.SendErrorMessage($"Multiple matches found for \"{namewithspaces}\": {multiple}");
                    return;
                }
                else if (found.Count == 0 || found == null) {
                    args.Player.SendErrorMessage($"No matches found for \"{namewithspaces}\".");
                    return;
                }
                else {
                    tstarget = found[0];
                }
            }
            else {
                tstarget = args.Player;
            }

            var target = tstarget.TPlayer;
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null) {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
                return;
            }

            Terraria.Main.ServerSideCharacter = true;
            Terraria.NetMessage.SendData(7, bot.ID, -1);
            for (int i = 0; i < NetItem.ArmorSlots; i++) {
                var current = target.armor[i];
                bot._client.AddPackets(new Packets.Packet5(bot.ID,
                    (byte)(i + 59),
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));         
            }
            for (int i = 0; i < NetItem.DyeSlots; i++) {
                var current = target.dye[i];
                bot._client.AddPackets(new Packets.Packet5(bot.ID,
                    (byte)(i + 79),
                    (short)current.stack,
                    current.prefix,
                    (short)current.netID));
            }
            Terraria.Main.ServerSideCharacter = false;
            Terraria.NetMessage.SendData(7, bot.ID, -1);

            bot._player.HairType = (byte)target.hair;
            bot._player.HairDye = target.hairDye;
            bot._player.SkinVariant = (byte)target.skinVariant;
            bot._player.HMisc = target.hideMisc;

            bot._player.EyeColor = target.eyeColor;
            bot._player.HairColor = target.hairColor;
            bot._player.PantsColor = target.pantsColor;
            bot._player.ShirtColor = target.shirtColor;
            bot._player.ShoeColor = target.shoeColor;
            bot._player.SkinColor = target.skinColor;
            bot._player.UnderShirtColor = target.underShirtColor;     

            Terraria.BitsByte bit1 = 0;
            for (int i = 0; i < 8; ++i) {
                bit1[i] = target.hideVisual[i];
            }
            bot._player.HVisuals1 = bit1;

            Terraria.BitsByte bit2 = 0;
            for (int i = 8; i < 10; ++i) {
                bit2[i] = target.hideVisual[i];
            }
            bot._player.HVisuals2 = bit2;

            bot._client.AddPackets(new Packets.Packet4(bot._player));

            args.Player.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now copying \"{tstarget.Name}\".");
            //Flag101 Flag104
        }
    }
}
