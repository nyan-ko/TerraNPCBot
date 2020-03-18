using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Terraria.Chat;
using System.Threading.Tasks;
using TerraNPCBot.Utils;
using TShockAPI;

namespace TerraNPCBot.Program {
    public class PluginCommands {

        public static void BotMaster(CommandArgs args) {
            if (args.Parameters.Count > 0 && args.Player != null) {
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

        #region Help
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

        public static bool NeedsHelpOrExample(List<string> args, TSPlayer plr, List<string> helpmsg, List<string> examplemsg ) {
            bool help = NeedsHelp(args);
            bool example = NeedsExample(args);

            if (help) {
                plr.MultiMsg(helpmsg, Color.Orange);
            }
            else if (example) {
                plr.MultiMsg(examplemsg, Color.Orange);
            }

            return help || example;
        }
        #endregion

        private static void Help(CommandArgs args) {
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

        private static void Select(CommandArgs args) {
            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Select, Messages.SelectExample))
                return;

            if (args.Parameters.Count > 1) {
                BTSPlayer player = Program.Players[args.Player.Index];
                if (player._ownedBots == null || player._ownedBots.Count == 0) {
                    args.Player?.SendErrorMessage("You do not have any owned bots.");
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

                    args.Player?.SendSuccessMessage($"Selected bot \"{player.SelectedBot.Name}\".");
                }
                else {
                    args.Player?.SendErrorMessage("Could not find specified bot.");
                }
            }
            else {
                args.Player.MultiMsg(Messages.Select, Color.Yellow);
            }
        }

        private static void NewBot(CommandArgs args) {
            try {
                if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.New, Messages.NewExample))
                    return;

                if (!args.Player.HasPermission(Permissions.BotUse)) {
                    args.Player?.SendErrorMessage(Messages.NoPermission);
                    return;
                }

                Bot bot;
                BTSPlayer bp = Program.Players[args.Player.Index];
                string name = "Michael Jackson";
                int port = 7777;
                if (args.Parameters.Count > 1) {
                    string tempName = args.Parameters[1].Trim('"');
                    if (tempName.Length > 30)
                        bp.SPlayer?.SendErrorMessage("Specified name exceeds 30 character limit. Defaulting to Michael Jackson.");
                    else
                        name = tempName;
                    if (args.Parameters.Count > 2)
                        if (!int.TryParse(args.Parameters[2], out port))
                            bp.SPlayer?.SendErrorMessage("Invalid port specified. Defaulting to 7777.");
                }

                bot = new Bot(args.Player.Index) { _player = new Player(name) };
                bot._client = new Client(bot, port);


                // Ports for each server Flag102

                if (bp._ownedBots.Count + 1 > bp._botLimit) {
                    args.Player?.SendErrorMessage($"You have reached the maximum number of bots you can create: {bp._botLimit}");
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
                args.Player?.SendInfoMessage(string.Format(Messages.BotSuccessCreateNew, bot.Name, port));
            }
            catch (Exception ex) {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorGeneric, ex.ToString()));
                return;
            }
        }

        private static void StartBot(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null) {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
            else if (bot.Running) {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorAlreadyRunning, bot.Name));
                
            }
            else if (!bot.Start()) {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorCouldNotStart, bot.Name));
            }
            else {
                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStarted, bot.Name));
                //bot._checkJoin.Start(); 
            }
        }        

        private static void StopBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot.Running) {
                    bot.Shutdown();
                    args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStopped, bot.Name));
                }
                else {
                    args.Player?.SendErrorMessage(string.Format(Messages.BotErrorNotRunning, bot.Name));
                }
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void DeleteBot(CommandArgs args) {
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (args.Parameters.Count == 1) {
                    var player = Program.Players[args.Player.Index];
                    player._ownedBots.RemoveAt(player._selected);
                    args.Player?.SendSuccessMessage("Successfully deleted selected bot.");
                }
                else {

                }
            }
            else {
                args.Player?.SendErrorMessage(string.Format(Messages.BotErrorNotFound));
            }
        }

        private static void Record(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Record, Messages.NoExample))
                return;


            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot._recording) {
                    args.Player?.SendErrorMessage($"Selected bot \"{bot.Name}\" is already recording.");
                    return;
                }
                bot._recording = true;
                bot._recordedPackets = new List<RecordedPacket>();

                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessRecording, bot.Name));
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void StopRecording(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Record, Messages.NoExample))
                return;


            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                bot._recording = false;
                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStopRecording, bot.Name));
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void Play(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Record, Messages.NoExample))
                return;


            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (bot._recordedPackets.Count == 0) {
                    args.Player?.SendErrorMessage("No recorded actions found.");
                    return;
                }

                bot.StartRecordTimer();

                args.Player?.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now playing back player actions.");
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void Copy(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCopy)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Copy, Messages.CopyExample))
                return;


            TSPlayer tstarget;
            if (args.Parameters.Count > 1) {
                if (!args.Player.HasPermission(Permissions.BotCopyOther)) {
                    args.Player?.SendErrorMessage("You do not have permission to copy other players.");
                    return;
                }

                string namewithspaces = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                namewithspaces = namewithspaces.Trim('"');

                var found = TShock.Utils.FindPlayer(namewithspaces);
                if (found.Count == 0 || found == null) {
                    args.Player?.SendErrorMessage($"No matches found for \"{namewithspaces}\".");
                    return;
                }                                                                      
                else if (found.Count > 1) {
                    string multiple = string.Join(", ", found);
                    args.Player?.SendErrorMessage($"Multiple matches found for \"{namewithspaces}\": {multiple}");
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
                args.Player?.SendErrorMessage("This player has disabled inventory copying.");
                return;
            }

            var target = tstarget.TPlayer;
            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot == null) {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
                return;
            }

            bot.Actions.Copy(target);

            args.Player?.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now copying \"{tstarget.Name}\".");
        }

        private static void Save(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotSave)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }
            StreamWriter.BTSPlayerToStream(Program.Players[args.Player.Index]);
            args.Player?.SendSuccessMessage("Successfully saved player data.");
        }

        private static void Prune(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotSavePruning)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            BTSPlayer bp = Program.Players[args.Player.Index];
            DateTime prune = DateTime.Now;

            if (args.Parameters.Count == 2) {
                prune = prune.AddDays(-7);
                args.Player?.SendInfoMessage("Defaulting prune to one week.");
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
                                args.Player?.SendErrorMessage($"Invalid hour format: \"{s}\"");
                            }
                            break;
                        case 'd':
                            if (int.TryParse(s.TrimEnd('d'), out int days)) {
                                day += days;
                            }
                            else {
                                args.Player?.SendErrorMessage($"Invalid day format: \"{s}\"");
                            }
                            break;
                        case 'm':
                            if (int.TryParse(s.TrimEnd('m'), out int months)) {
                                month += months;
                            }
                            else {
                                args.Player?.SendErrorMessage($"Invalid month format: \"{s}\"");
                            }
                            break;
                        case 'y':
                            if (int.TryParse(s.TrimEnd('y'), out int years)) {
                                year += years;
                            }
                            else {
                                args.Player?.SendErrorMessage($"Invalid year format: \"{s}\"");
                            }
                            break;
                        default:
                            args.Player?.SendErrorMessage($"Invalid year format: \"{s}\"");
                            break;
                    }
                }

                prune = prune.AddHours(hour * -1);
                prune = prune.AddDays(day * -1);
                prune = prune.AddMonths(month * -1);
                prune = prune.AddYears(year * -1);

                args.Player?.SendInfoMessage($"Prune set to: {prune.Year}, {prune.Month}, {prune.Day}, {prune.Hour}. All prior saves will be moved to the prune folder.");
            }

            PluginUtils.PruneSaves(prune); 
        }

        private static void Chat(CommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotChat)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Select, Messages.SelectExample))
                return;


            var bot = Program.Players[args.Player.Index]?.SelectedBot;
            if (bot != null) {
                if (args.Parameters.Count == 1) {
                    args.Player?.SendErrorMessage("Expected message or command as input.");
                    return;
                }
                var message = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                message = message.Trim('"');
                bot.Actions.Chat(message);
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void Teleport(CommandArgs args) {
            if (NeedsHelpOrExample(args.Parameters, args.Player, Messages.Teleport, Messages.TeleportExample))
                return;

            if (!args.Player.HasPermission(Permissions.BotTeleport)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
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
                        args.Player?.SendErrorMessage($"No matches found for \"{namewithspaces}\".");
                        return;
                    }
                    else if (found.Count > 1) {
                        string multiple = string.Join(", ", found);
                        args.Player?.SendErrorMessage($"Multiple matches found for \"{namewithspaces}\": {multiple}");
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
                    args.Player?.SendErrorMessage("This player has disabled bot teleportation.");
                    return;
                }

                bot.Actions.Teleport(tstarget.LastNetPosition);
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        public static void Debug(CommandArgs args) {
            args.Player.SendMessage(args.Player.Index.ToString(), Color.Orange);
        }
    }
}
