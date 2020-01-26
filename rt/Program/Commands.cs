﻿using System;
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
                    case "help":
                        Help(args);
                        break;
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
                        else {
                            args.Player.MultiMsg(Messages.Record, Color.Yellow);
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
                    case "chat":
                        Chat(args);
                        break;
                    case "teleport":
                        Teleport(args);
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

        public static bool NeedsHelp(List<string> args) {
            List<string> s = new List<string>();
            foreach (var x in args) {
                s.Add(x.ToLower());
            }
            return s.Contains("help");
        }
        public static bool NeedsExample(List<string> args) {
            List<string> s = new List<string>();
            foreach (var x in args) {
                s.Add(x.ToLower());
            }
            return s.Contains("example") || s.Contains("examples");
        }

        static void Help(CommandArgs args) {
            if (args.Parameters.Count == 1) {
                args.Player.MultiMsg(Messages.Master1, Color.Yellow);
            }
            else if (args.Parameters.Count == 2) {
                switch (args.Parameters[1]) {
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
        }

        static void Select(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Select, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.MultiMsg(Messages.SelectExample, Color.Orange);
                return;
            }

            if (args.Parameters.Count > 1) {
                BTSPlayer player = Program.Players[args.Player.Index];
                if (player._ownedBots == null || player._ownedBots.Count == 0) {
                    args.Player.SendErrorMessage("You do not have any owned bots.");
                    return;
                }

                int index = -1;
                if (int.TryParse(args.Parameters[1], out int i)) {
                    index = i - 1;
                }
                else {
                    string name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)).ToLower();
                    var bot = player._ownedBots.FirstOrDefault(x => x.Name.ToLower().StartsWith(name) || x.Name.ToLower() == name);
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
            else {
                args.Player.MultiMsg(Messages.Select, Color.Yellow);
            }
        }

        static void NewBot(CommandArgs args) {
            try {
                if (NeedsHelp(args.Parameters)) {
                    args.Player.MultiMsg(Messages.New, Color.Orange);
                    return;
                }
                if (NeedsExample(args.Parameters)) {
                    args.Player.MultiMsg(Messages.NewExample, Color.Orange);
                    return;
                }

                if (!args.Player.HasPermission(Permissions.BotUse)) {
                    args.Player.SendErrorMessage(Messages.NoPermission);
                    return;
                }

                Bot bot;
                BTSPlayer bp = Program.Players[args.Player.Index];                

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
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

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

        private static void Record(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Record, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.SendErrorMessage(Messages.NoExample);
                return;
            }

            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

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

        private static void StopRecording(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Record, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.SendErrorMessage(Messages.NoExample);
                return;
            }

            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = false;
                args.Player.SendSuccessMessage(string.Format(Messages.BotSuccessStopRecording, bot.Name));
            } 
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void Play(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Record, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.SendErrorMessage(Messages.NoExample);
                return;
            }

            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

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

        private static void Copy(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Copy, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.MultiMsg(Messages.CopyExample, Color.Orange);
                return;
            }

            if (!args.Player.HasPermission(Permissions.BotCopy)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

            TSPlayer tstarget;
            if (args.Parameters.Count > 1) {
                if (!args.Player.HasPermission(Permissions.BotCopyOther)) {
                    args.Player.SendErrorMessage("You do not have permission to copy other players.");
                    return;
                }

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
            if (!Program.Players[tstarget.Index]._canBeCopied && !args.Player.HasPermission(Permissions.BotBypassCopy)) {
                args.Player.SendErrorMessage("This player has disabled inventory copying.");
                return;
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
            if (!args.Player.HasPermission(Permissions.BotSave)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }
            StreamWriter.BTSPlayerToStream(Program.Players[args.Player.Index]);
            args.Player.SendSuccessMessage("Successfully saved player data.");
        }

        static void Prune(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotSavePruning)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

            BTSPlayer bp = Program.Players[args.Player.Index];
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

        static void Chat(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Chat, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.MultiMsg(Messages.ChatExample, Color.Orange);
                return;
            }

            if (!args.Player.HasPermission("bot.chat")) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (args.Parameters.Count == 1) {
                    args.Player.SendErrorMessage("Expected message or command as input.");
                    return;
                }
                var message = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                message = message.Trim('"');
                bot.Actions.Chat(message);
            }
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        static void Teleport(CommandArgs args) {
            if (NeedsHelp(args.Parameters)) {
                args.Player.MultiMsg(Messages.Teleport, Color.Orange);
                return;
            }
            if (NeedsExample(args.Parameters)) {
                args.Player.MultiMsg(Messages.TeleportExample, Color.Orange);
                return;
            }

            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
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
                if (!Program.Players[tstarget.Index]._canBeTeleportedTo && !args.Player.HasPermission(Permissions.BotBypassTele)) {
                    args.Player.SendErrorMessage("This player has disabled bot teleportation.");
                    return;
                }

                bot.Actions.Teleport(tstarget.LastNetPosition);
            }
            else {
                args.Player.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
    }
}
