using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using Terraria.Chat;
using System.Collections.Concurrent;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;
using TShockAPI;

namespace TerraNPCBot.Program {
    public class PluginCommands {
        public static void BotMaster(CommandArgs arg) {
            BotCommandArgs args = new BotCommandArgs(arg);
            if (args.Parameters.Count > 0 && args.Player != null) {
                switch (args.Parameters[0]) {
                    case "help":
                        Help(args);
                        break;
                    case "list":
                        List(args);
                        break;
                    case "ignore":
                        Ignore(args);
                        break;
                    case "info":
                        Info(args);
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
        private static bool NeedsHelp(List<string> args) {
            List<string> s = new List<string>();
            foreach (var x in args) {
                s.Add(x.ToLower());
            }
            return s.Contains("help");
        }
        private static bool NeedsExample(List<string> args) {
            List<string> s = new List<string>();
            foreach (var x in args) {
                s.Add(x.ToLower());
            }
            return s.Contains("example") || s.Contains("examples");
        }

        public static bool NeedsHelpOrExample(CommandArgs args, List<string> helpmsg, List<string> examplemsg ) {
            bool help = NeedsHelp(args.Parameters);
            bool example = NeedsExample(args.Parameters);

            if (help) {
                args.Player.MultiMsg(helpmsg, Color.Orange);
            }
            else if (example) {
                args.Player.MultiMsg(examplemsg, Color.Orange);
            }

            return help || example;
        }
        #endregion

        private static void Ignore(BotCommandArgs args) {
            args.BPlayer.IgnoreBots = !args.BPlayer.IgnoreBots;
            args.Player.SendInfoMessage($"You are {(args.BPlayer.IgnoreBots ? "now" : "no longer")} ignoring server bots.");
        }

        private static void List(BotCommandArgs args) {
            if (args.BPlayer.ownedBots.Count == 0) {
                args.Player.SendInfoMessage("You do not have any bots.");
                return;
            }
            foreach (Bot bot in args.BPlayer.ownedBots) {
                args.Player.SendInfoMessage($"[{bot.IndexInOwnerBots + 1}] {bot.Name} | Currently running: {bot.Running}");
            }
            args.Player.SendInfoMessage($"Owned bots: ({args.BPlayer.ownedBots.Count}/{args.BPlayer.botLimit})");
        }

        private static void Info(BotCommandArgs args) {
            if (args.Parameters.Count == 1) {
                BTSPlayer bplayer = args.BPlayer;
                args.Player.MultiMsg(Color.Yellow, $"Bot limit: {bplayer.botLimit}",
                    $"Currently owned bots: {bplayer.ownedBots.Count}",
                    $"Currently selected bot: {bplayer.SelectedBot.Name}",
                    $"Autosave on leave: {bplayer.autosave}",
                    $"Allow bot copying: {bplayer.canBeCopied}",
                    $"Allow bot teleport: {bplayer.canBeTeleportedTo}");
            }
            else if (args.Parameters.Count > 1) {

            }
        }

        private static void Help(BotCommandArgs args) {
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

        private static void Select(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Select, Messages.SelectExample))
                return;

            if (args.Parameters.Count > 1) {
                BTSPlayer player = args.BPlayer;
                if (player.ownedBots == null || player.ownedBots.Count == 0) {
                    args.Player?.SendErrorMessage("You do not have any owned bots.");
                    return;
                }

                string nameOrIndex = string.Join(" ", args.Parameters.Skip(1));
                List<Bot> foundbots = player.GetBotFromIndexOrName(nameOrIndex);

                if (foundbots.Count == 0) {
                    args.Player?.SendErrorMessage("Could not find specified bot.");
                }
                else if (foundbots.Count > 1) {
                    args.Player?.SendErrorMessage("Multiple bots matched your search criteria:");
                    List<string> names = new List<string>();
                    foreach (Bot bot in foundbots) {
                        names.Add("\"" + bot.Name + "\"");
                    }
                    string stringnames = string.Join(", ", names);
                    args.Player?.SendErrorMessage(stringnames);
                }
                else if (foundbots.Count == 1) {
                    player.selected = foundbots[0].IndexInOwnerBots;
                    args.Player?.SendSuccessMessage($"Selected bot \"{foundbots[0].Name}\" with index {foundbots[0].IndexInOwnerBots + 1}.");
                }
            }
            else {
                args.Player.MultiMsg(Messages.Select, Color.Yellow);
            } 
        }

        private static void NewBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.New, Messages.NewExample))
                return;

            BTSPlayer bp = args.BPlayer;
            string name = "Michael Jackson";
            int port = 7777;

            if (args.Parameters.Count > 1) {

                // Names
                string tempName = args.Parameters[1].Trim('"');
                if (tempName.Length > 30) {
                    args.Player?.SendErrorMessage("Specified name exceeds 30 character limit. Defaulting to Michael Jackson.");
                    tempName = "Michael Jackson";
                }
                else if (!PluginUtils.ValidBotName(ref tempName)) {
                    args.Player.SendErrorMessage("Found illegal characters in name, replacing with censored characters.");
                }
                name = tempName;

                // Ports
                if (args.Parameters.Count == 3 && !int.TryParse(args.Parameters[2], out port)) {
                    args.Player?.SendErrorMessage("Specified port is not recognized. Defaulting to 7777.");
                }
            }

            Bot bot = new Bot(args.Player.Index, bp.ownedBots.Count);
            bot.client = new Client(port, bot);
            bot.player = new Player(name);

            // Ports for each server Flag102

            if (bp.ownedBots.Count + 1 > bp.botLimit) {
                args.Player?.SendErrorMessage($"You have reached the maximum number of bots you can create: {bp.botLimit}");
                return;
            }
            bp.ownedBots.Add(bot);
            bp.selected = bp.ownedBots.Count - 1;

            args.Player?.SendInfoMessage(string.Format(Messages.BotSuccessCreateNew, bot.Name));
        }

        private static void StartBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            var bot = args.SelectedBot;
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
            }
        }        

        private static void StopBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            var bot = args.SelectedBot;
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

        private static void ConfirmedDelete(object obj) {
            BotCommandArgs args = new BotCommandArgs((CommandArgs)obj);
            
            var player = args.BPlayer;
            player.SelectedBot.Shutdown();
            player.ownedBots.RemoveAt(player.SelectedDelete);
            player.SPlayer?.SendSuccessMessage("Successfully deleted bot.");  
        }

        private static void RefuseDelete(object obj) {
            BotCommandArgs args = new BotCommandArgs((CommandArgs)obj);

            // Remove /confirm from awaiting responses
            args.Player.AddResponse("confirm", null);
        }

        private static void DeleteBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Delete, Messages.DeleteExample))
                return;

            var player = Program.Players[args.Player.Index];
            if (args.Parameters.Count == 1 && player.selected != -1) {
                player.SelectedDelete = player.selected;
                args.Player?.SendSuccessMessage("Currently selected bot will be deleted upon confirmation: /confirm or /deny.");

                args.Player?.AddResponse("confirm", new Action<object>(ConfirmedDelete));
                args.Player?.AddResponse("deny", new Action<object>(RefuseDelete));
            }
            else if (args.Parameters.Count > 1) {
                string nameOrIndex = string.Join(" ", args.Parameters.Skip(1)).Trim('"');
                if (player.ownedBots == null || player.ownedBots.Count == 0) {
                    args.Player?.SendErrorMessage("You do not have any owned bots.");
                    return;
                }
                List<Bot> foundbots = player.GetBotFromIndexOrName(nameOrIndex);

                if (foundbots.Count == 0) {
                    args.Player?.SendErrorMessage("Could not find specified bot.");
                }
                else if (foundbots.Count > 1) {
                    args.Player?.SendErrorMessage("Multiple bots matched your search criteria:");
                    List<string> names = new List<string>();
                    foreach (Bot bot in foundbots) {
                        names.Add("\"" + bot.Name + "\"");
                    }
                    string stringnames = string.Join(", ", names);
                    args.Player?.SendErrorMessage(stringnames);
                }
                else if (foundbots.Count == 1) {
                    player.SelectedDelete = foundbots[0].IndexInOwnerBots;
                    args.Player?.SendSuccessMessage($"Selecting bot \"{foundbots[0].Name}\" with index {foundbots[0].IndexInOwnerBots} to delete: /confirm or /deny.");

                    args.Player?.AddResponse("confirm", new Action<object>(ConfirmedDelete));
                    args.Player?.AddResponse("deny", new Action<object>(RefuseDelete));
                }
            }
            else {
                args.Player?.MultiMsg(Messages.Delete, Color.Yellow);
            }
        }

        private static void Record(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Record, Messages.NoExample))
                return;

            var bot = args.SelectedBot;
            if (bot != null) {
                if (bot.recording) {
                    args.Player?.SendErrorMessage($"Selected bot \"{bot.Name}\" is already recording.");
                    return;
                }
                bot.recording = true;
                bot.recordedPackets = new List<RecordedPacket>();

                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessRecording, bot.Name));
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void StopRecording(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Record, Messages.NoExample))
                return;

            var bot = args.SelectedBot;
            if (bot != null) {
                bot.recording = false;
                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStopRecording, bot.Name));
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void Play(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotRecord) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Record, Messages.NoExample))
                return;

            var bot = args.SelectedBot;
            if (bot != null) {
                if (bot.recordedPackets.Count == 0) {
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

        private static void Copy(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCopy) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Copy, Messages.CopyExample))
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
            if (!Program.Players[tstarget.Index].canBeCopied && !args.Player.HasPermission(Permissions.BotBypassCopy)) {
                args.Player?.SendErrorMessage("This player has disabled inventory copying.");
                return;
            }

            var target = tstarget.TPlayer;
            var bot = args.SelectedBot;
            if (bot == null) {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
                return;
            }

            bot.Actions.FullCopy(target);

            args.Player?.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now copying \"{tstarget.Name}\".");
        }

        private static void Save(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotSave) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }
            FileWriter.BTSPlayerToStream(Program.Players[args.Player.Index]);
            args.Player?.SendSuccessMessage("Successfully saved player data.");
        }

        private static void Prune(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotSavePruning) || !args.Player.HasPermission(Permissions.BotUse)) {
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
                            args.Player?.SendErrorMessage($"Invalid time format: \"{s}\"");
                            break;
                    }
                }

                prune = prune.AddHours(hour * -1);
                prune = prune.AddDays(day * -1);
                prune = prune.AddMonths(month * -1);
                prune = prune.AddYears(year * -1);

            }

            args.Player?.SendInfoMessage($"Prune set to: {prune.Year}, {prune.Month}, {prune.Day}, {prune.Hour}. All prior saves will be moved to the prune folder.");
            PluginUtils.PruneSaves(prune); 
        }

        private static void Chat(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotChat) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Select, Messages.SelectExample))
                return;

            var bot = args.SelectedBot;
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

        private static void Teleport(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotTeleport) || !args.Player.HasPermission(Permissions.BotUse)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Teleport, Messages.TeleportExample))
                return;

            var bot = args.SelectedBot;
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
                if (tstarget != args.Player && !Program.Players[tstarget.Index].canBeTeleportedTo && !args.Player.HasPermission(Permissions.BotBypassTeleport)) {
                    args.Player?.SendErrorMessage("This player has disabled bot teleportation.");
                    return;
                }

                bot.Actions.Teleport(tstarget.LastNetPosition);
                args.Player?.SendSuccessMessage($"Teleporting bot to {tstarget.Name}.");
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        public static void Debug(CommandArgs args) {
            BotCommandArgs botArgs = new BotCommandArgs(args);
            botArgs.SelectedBot?.Actions.PlayNote(float.Parse(args.Parameters[0]));
        }
    }
}
