using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Terraria.Chat;
using System.Threading.Tasks;
using rt.Utils;
using TShockAPI;

namespace rt.Program {
    public class PluginCommands {

        public static void BotMaster(CommandArgs args) {
            if (args.Parameters.Count > 0) {
                switch (args.Parameters[0]) {
                    case "select":
                        Select(args);
                        break;
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
                        }
                        break;
                    case "delete":
                        DeleteBot(args);
                        break;
                    case "save":
                        if (args.Parameters.Count > 1) {
                            switch (args.Parameters[1]) {
                                case "prune":
                                    Prune(args);
                                    break;
                            }
                        }
                        Save(args);
                        break;
                    case "talk":
                        Talk(args);
                        break;
                    default:
                        args.Player.MultiMsg(Messages.Master, Color.Yellow);
                        break;
                }
            }
            else {
                args.Player.MultiMsg(Messages.Master, Color.Yellow);
            }
        }

        static void Select(CommandArgs args) {
            if (args.Parameters.Count > 1) {
                int index = -1;
                BTSPlayer player = Program.Players[args.Player.Index];
                if (int.TryParse(args.Parameters[1], out int i)) {
                    index = i;
                }
                else {
                    string name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    var bot = player._ownedBots.FirstOrDefault(x => x.Name == name);
                    if (bot != null) {
                        index = player._ownedBots.IndexOf(bot);
                    }
                }
                if (index >= 0 && index < player._ownedBots.Count) {
                    player._selected = index;

                    args.Player.SendSuccessMessage($"Selected bot \"{player.SelectedBot.Name}\".");
                }
                else {
                    args.Player.SendErrorMessage("Could not find specified bot.");
                }
            }
        }

        static void NewBot(CommandArgs args) {
            try {
                Bot bot;
                BTSPlayer bp = Program.Players[args.Player.Index];

                if (!bp.SPlayer.HasPermission("bot.create")) {
                    args.Player.SendErrorMessage(Messages.NoPermission);
                    return;
                }

                string name = "";
                if (args.Parameters.Count > 1) {
                    name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    name = name.Trim('"');
                }
                bot = args.Parameters.Count > 1
                    ? new Bot(Bot.Address, args.Player.Index, 7777, name)
                    : new Bot(Bot.Address, args.Player.Index);

                // Ports for each server Flag102

                if (bp._ownedBots.Count + 1 > bp._botLimit) {
                    args.Player.SendErrorMessage($"You have reached the maximum number of bots you can create: {bp._botLimit}");
                    return;
                }
                bp._ownedBots.Add(bot);
                bp._selected = bp._ownedBots.Count - 1;

                for (int i = 0; i < NetItem.InventorySlots; ++i) {
                    bot._player.InventorySlots[i] = new Terraria.Item() { netID = 0, stack = 0, prefix = 0 };
                }
                for (int i = 0; i < NetItem.ArmorSlots; ++i) {
                    bot._player.ArmorSlots[i] = new Terraria.Item() { netID = 0, stack = 0, prefix = 0 };
                }
                for (int i = 0; i < NetItem.DyeSlots; ++i) {
                    bot._player.DyeSlots[i] = new Terraria.Item() { netID = 0, stack = 0, prefix = 0 };
                }
                for (int i = 0; i < NetItem.MiscEquipSlots; ++i) {
                    bot._player.MiscEquipSlots[i] = new Terraria.Item() { netID = 0, stack = 0, prefix = 0 };
                }
                for (int i = 0; i < NetItem.MiscDyeSlots; ++i) {
                    bot._player.MiscDyeSlots[i] = new Terraria.Item() { netID = 0, stack = 0, prefix = 0 };
                }
                args.Player.SendMessage(string.Format(Messages.BotSuccessCreateNew, bot.Name), Color.Yellow);
            }
            catch (Exception ex) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorGeneric, ex.ToString()));
                return;
            }
        }

        static void StartBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null) {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
            else if (bot.Running) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorAlreadyRunning, bot.Name));
                
            }
            else if (!bot.Start()) {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorCouldNotStart, bot.Name));
            }
            else {
                args.Player.SendMessage(string.Format(Messages.BotSuccessStarted, bot.Name), Color.Green);
                bot._checkJoin.Start();
            }
        }        

        static void StopBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot.Running) {
                    Terraria.NetMessage.SendData((int)PacketTypes.Disconnect, bot.ID);
                    
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

        static void DeleteBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                var player = Program.Players[args.Player.Index];
                try {
                    player._ownedBots.RemoveAt(player._selected);
                    player._ownedBots.TrimExcess();
                    args.Player.SendSuccessMessage("Successfully deleted selected bot.");
                }
                catch (Exception ex) {
                    args.Player.SendErrorMessage(string.Format(Messages.BotErrorGeneric, ex.ToString()));
                    return;
                }
            }
            else {
                args.Player.SendErrorMessage(string.Format(Messages.BotErrorNotFound));
            }
        }

        static void Record(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot._recording) {
                    args.Player.SendErrorMessage($"Selected bot \"{bot.Name}\" is already recording.");
                    return;
                }
                bot._recording = true;
                bot._recordedPackets = new List<RecordedPacket>();

                args.Player.SendSuccessMessage(string.Format(Messages.BotSuccessRecording, bot.Name));
            } 
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        static void StopRecording(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = false;
                args.Player.SendSuccessMessage(string.Format(Messages.BotSuccessStopRecording, bot.Name));
            } 
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        static void Play(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot._recordedPackets.Count == 0) {
                    args.Player.SendErrorMessage("No recorded actions found.");
                    return;
                }

                bot.StartRecordTimer();

                args.Player.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now playing back player actions.");
            } 
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
                if (found.Count == 0 || found == null) {
                    args.Player.SendErrorMessage($"No matches found for \"{namewithspaces}\".");
                    return;
                }                                                                      
                else if (found.Count > 1) {
                    string multiple = string.Join(", ", found);
                    args.Player.SendErrorMessage($"Multiple matches found for \"{namewithspaces}\": {multiple}");
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

            bot.Actions.Copy(target);

            args.Player.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now copying \"{tstarget.Name}\".");
        }

        static void Save(CommandArgs args) {
            StreamWriter.BTSPlayerToStream(Program.Players[args.Player.Index]);
        }

        static void Prune(CommandArgs args) {
            BTSPlayer bp = Program.Players[args.Player.Index];

            if (!bp.SPlayer.HasPermission("bot.save.prune")) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }
            DateTime prune = DateTime.Now;

            if (args.Parameters.Count == 2) {
                prune = prune.AddDays(-7);
                args.Player.SendInfoMessage("Defaulting prune to one week.");
            }
            else {
                int year = 0;
                int month = 0;
                int day = 0;
                int hour = 0;

                List<string> time = args.Parameters.GetRange(2, args.Parameters.Count);
                foreach (string s in time) {
                    switch (s[s.Length - 1]) {
                        case 'h':
                            if (int.TryParse(s.TrimEnd('h'), out int hours)) {
                                hour += hours;
                            }
                            else {
                                args.Player.SendErrorMessage($"Invalid hour format: \"{s}\"");
                            }
                            break;
                        case 'd':
                            if (int.TryParse(s.TrimEnd('d'), out int days)) {
                                day += days;
                            }
                            else {
                                args.Player.SendErrorMessage($"Invalid day format: \"{s}\"");
                            }
                            break;
                        case 'm':
                            if (int.TryParse(s.TrimEnd('m'), out int months)) {
                                month += months;
                            }
                            else {
                                args.Player.SendErrorMessage($"Invalid month format: \"{s}\"");
                            }
                            break;
                        case 'y':
                            if (int.TryParse(s.TrimEnd('y'), out int years)) {
                                year += years;
                            }
                            else {
                                args.Player.SendErrorMessage($"Invalid year format: \"{s}\"");
                            }
                            break;
                    }
                }

                prune = prune.AddHours(hour * -1);
                prune = prune.AddDays(day * -1);
                prune = prune.AddMonths(month * -1);
                prune = prune.AddYears(year * -1);

                args.Player.SendInfoMessage($"Prune set to: {prune.Year}, {prune.Month}, {prune.Day}, {prune.Hour}. All prior saves will be moved to the prune folder.");
            }

            PluginUtils.PruneSaves(prune);
        }

        static void Talk(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (args.Parameters.Count == 1) {
                    args.Player.SendErrorMessage("Expected message or command as input.");
                    return;
                }
                var message = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                message = message.Trim('"');
                bot.Actions.Talk(message);
            }
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
    }
}
