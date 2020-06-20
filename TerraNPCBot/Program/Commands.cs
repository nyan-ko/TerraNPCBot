using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Terraria.Chat;
using System.Collections.Concurrent;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;
using TShockAPI;

namespace TerraNPCBot.Program {
    public class PluginCommands {

        /// <summary>
        /// Dictionary of every command and their identifier.
        /// </summary>
        public static Dictionary<string, Action<BotCommandArgs>> CommandsByTag = new Dictionary<string, Action<BotCommandArgs>> {
            { "help", Help },
            { "ignore", Ignore },
            { "list", List },
            { "info", Info },
            { "select", Select },
            { "new", NewBot },
            { "start", StartBot },
            { "stop", StopBot },
            { "delete", DeleteBot }, { "del", DeleteBot },
            { "record", RecordMaster }, { "rec", RecordMaster },
            { "copy", Copy },
            { "chat", Chat },
            { "teleport", Teleport }, { "tp", Teleport }
        };

        private static BlockingCollection<BotCommandArgs> CommandQueue = new BlockingCollection<BotCommandArgs>();

        internal static void CommandThread(object state) {
            while (true) {
                if (!CommandQueue.TryTake(out BotCommandArgs args)) {
                    continue;
                }

                if (args.Parameters.Count == 0 || !CommandsByTag.ContainsKey(args.Parameters[0])) {
                    CommandsByTag["help"].Invoke(args);
                    continue;
                }

                for (int i = 0; i < args.Parameters.Count; ++i) {
                    if (CommandsByTag.TryGetValue(args.Parameters[i], out Action<BotCommandArgs> command)) {
                        command.Invoke(args);
                        args.OffsetToNextSection();
                        i = args.LowerSplitterBound;
                    }                    
                }
            }
        }

        public static void AddCommand(CommandArgs command) => CommandQueue.Add(new BotCommandArgs(command));

        /// <summary>
        /// Fucking stupid dumbass switch statement command
        /// </summary>
        /// <param name="arg"></param>
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
                        RecordMaster(args);
                        break;
                    case "delete":
                        DeleteBot(args);
                        break;
                    //case "save":
                    //    if (args.Parameters.Count > 1) {
                    //        switch (args.Parameters[1]) {
                    //            case "prune":
                    //                Prune(args);
                    //                break;
                    //        }
                    //    }
                    //    Save(args);
                    //    break;
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

        #region Help Command
        private static void Help(BotCommandArgs args) {
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1) {
                args.Player.MultiMsg(Messages.Master1, Color.Yellow);
            }
            else if (currentSection.Count == 2) {
                switch (currentSection[1]) {
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
            else {
                args.Player.MultiMsg(Messages.Master1, Color.Yellow);
            }
        }

        #endregion

        #region Help
        private static bool NeedsHelp(List<string> args) {
            return args.Exists(x => x.ToLower() == "help");
        }
        private static bool NeedsExample(List<string> args) {
            return args.Exists(x => x.ToLower() == "example");
        }

        private static bool NeedsHelpOrExample(BotCommandArgs args, string helpmsg, string examplemsg ) {
            bool help = NeedsHelp(args.CurrentSection);
            bool example = NeedsExample(args.CurrentSection);

            if (help) {
                args.Player.MultiMsg(helpmsg, Color.Orange);
            }
            else if (example) {
                args.Player.MultiMsg(examplemsg, Color.Orange);
            }

            return help | example;
        }
        #endregion

        #region Ignore
        /// <summary>
        /// Command to ignore bots and their packets.
        /// </summary>
        /// <param name="args"></param>
        private static void Ignore(BotCommandArgs args) {
            if (NeedsHelpOrExample(args, Messages.Ignore, Messages.NoExample))
                return;

            if (args.Player.HasPermission(Permissions.BotIgnore)) {
                args.Player.SendErrorMessage(Messages.NoPermission);
                return;
            }

            args.BPlayer.IgnoreBots = !args.BPlayer.IgnoreBots;
            args.Player.SendInfoMessage($"You are {(args.BPlayer.IgnoreBots ? "now" : "no longer")} ignoring server bots.");
        }
        #endregion

        #region List
        /// <summary>
        /// Lists a player's bots with additional running info.
        /// </summary>
        /// <param name="args"></param>
        private static void List(BotCommandArgs args) {
            if (args.BPlayer.OwnedBots.Count == 0) {
                args.Player.SendInfoMessage("You do not have any bots.");
                return;
            }
            foreach (Bot bot in args.BPlayer.OwnedBots) {
                args.Player.SendInfoMessage($"[{bot.IndexInOwnerBots + 1}] {bot.Name} | Currently running: {bot.Running}");
            }
            args.Player.SendInfoMessage($"Owned bots: ({args.BPlayer.OwnedBots.Count}/{args.BPlayer.BotLimit})");
        }

        #endregion

        #region Info
        /// <summary>
        /// Lists a player or their bot's info.
        /// </summary>
        /// <param name="args"></param>
        private static void Info(BotCommandArgs args) {
            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1) {
                args.Player.MultiMsg(Color.Yellow, $"Bot limit: {player.BotLimit}",
                    $"Currently owned bots: {player.OwnedBots.Count}",
                    $"Currently selected bot: {player.SelectedBot.Name}",
                    $"Autosave on leave: {player.autosave}",
                    $"Allow bot copying: {player.canBeCopied}",
                    $"Allow bot teleport: {player.canBeTeleportedTo}");
            }
            else if (currentSection.Count > 2) {
                if (player.OwnedBots.Count == 0) {
                    args.Player.SendErrorMessage(Messages.NoOwnedBots);
                    return;
                }
                string nameOrIndex = string.Join(" ", currentSection.Skip(1)).Trim('"');
                var bots = player.GetBotFromIndexOrName(nameOrIndex);
                if (bots.Count == 0) {
                    args.Player?.SendErrorMessage(string.Format(Messages.NoBotsFound, nameOrIndex));
                }
                else if (bots.Count > 1) {
                    args.Player?.SendErrorMessage(Messages.MultipleBotsFound);
                    List<string> names = new List<string>();
                    foreach (var bot in player.OwnedBots) {
                        names.Add($"\"{bot.Name}\"");
                    }
                    args.Player?.SendErrorMessage(string.Join(", ", names));
                }
                else {
                    Bot foundBot = bots[0];
                    if (Program.ServersByPorts.TryGetValue(foundBot.Port.ToString(), out string serverName)) {
                        serverName = "none..?";
                    }
                    args.Player.MultiMsg(Color.Yellow, $"Bot name: {foundBot.Name}",
                        $"Bot running: {foundBot.Running}",
                        $"Bot index: {foundBot.ID}",
                        $"Bot index in owned bots: {foundBot.IndexInOwnerBots}",
                        $"Bot position: ({foundBot.TilePosition.X}, {foundBot.TilePosition.Y})",
                        $"Bot server: {serverName}");
                }
            }
        }
        #endregion

        #region Select
        private static void Select(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCreate)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Select, Messages.SelectExample))
                return;

            BTSPlayer player = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count > 1) {
                if (player.OwnedBots == null || player.OwnedBots.Count == 0) {
                    args.Player?.SendErrorMessage(Messages.NoOwnedBots);
                    return;
                }

                string nameOrIndex = string.Join(" ", currentSection.Skip(1)).Trim('"');
                List<Bot> foundbots = player.GetBotFromIndexOrName(nameOrIndex);

                if (foundbots.Count == 0) {
                    args.Player?.SendErrorMessage(string.Format(Messages.NoBotsFound, nameOrIndex));
                }
                else if (foundbots.Count > 1) {
                    args.Player?.SendErrorMessage(Messages.MultipleBotsFound);
                    List<string> names = new List<string>();
                    foreach (Bot bot in foundbots) {
                        names.Add("\"" + bot.Name + "\"");
                    }
                    string stringnames = string.Join(", ", names);
                    args.Player?.SendErrorMessage(stringnames);
                }
                else if (foundbots.Count == 1) {
                    player.Selected = foundbots[0].IndexInOwnerBots;
                    args.Player?.SendSuccessMessage($"Selected bot \"{foundbots[0].Name}\" with index {foundbots[0].IndexInOwnerBots + 1}.");
                }
            }
            else {
                args.Player.MultiMsg(Messages.Select, Color.Yellow);
            } 
        }
        #endregion

        #region New
        /// <summary>
        /// Command to create a new bot.
        /// </summary>
        /// <param name="args"></param>
        private static void NewBot(BotCommandArgs args) {

            // Example command
            //       0     1    2     3        4
            // /bot new Michael F. Jackson 7777/nexus
            // Since port (if it is specified) is always be the last parameter, the range between 'new' and the port will yield the full name.

            if (args.BPlayer.OwnedBots.Count + 1 > args.BPlayer.BotLimit) {
                args.Player?.SendErrorMessage($"You have reached the maximum number of bots you can create: {args.BPlayer.BotLimit}");
                return;
            }

            if (!args.Player.HasPermission(Permissions.BotCreate)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.New, Messages.NewExample))
                return;

            BTSPlayer bp = args.BPlayer;
            List<string> currentSection = args.CurrentSection;
            string name = "Michael Jackson";
            int port = 7777;

            // If the user has specified the name and or port for the bot.
            if (currentSection.Count > 1) {
                // The last parameter which may or may not be the port
                string potentialPort = currentSection[currentSection.Count - 1];
                // Whether the user has specified a port, which is present in the last parameter
                bool hasSpecifiedPort = false;
                if (Program.ServersByPorts.ContainsKey(potentialPort)) {
                    hasSpecifiedPort = true;
                    port = int.Parse(potentialPort);
                }
                else if (Program.PortsByServers.TryGetValue(potentialPort.ToLower(), out port))
                    hasSpecifiedPort = true;
                // Gets the range in the parameter list that contains the user specified name.
                // If there is a port, get the range between 'new' and the port
                // Else, skip 'new' in the current secton to get the range.
                List<string> nameRange = hasSpecifiedPort ? currentSection.GetRange(1, currentSection.Count - 2) : currentSection.Skip(1).ToList();

                // Determine bot name
                string tempName = string.Join(" ", nameRange).Trim('"');
             
                if (tempName.Length > 30) {
                    args.Player?.SendErrorMessage("Specified name exceeds 30 character limit. Defaulting to Michael Jackson.");
                    tempName = "Michael Jackson";
                }
                else if (!PluginUtils.ValidateBotName(ref tempName)) {
                    args.Player.SendErrorMessage("Found and replaced illegal characters in name.");
                }
                name = tempName;

                // Notify user they either did not specify a port or they gave an invalid port.
                if (!hasSpecifiedPort) {
                    args.Player.SendInfoMessage("No specified port detected or an invalid port was given. Defaulting to 7777 (Nexus).");
                }
            }

            Bot bot = new Bot(name, args.Player.Index, bp.OwnedBots.Count, port);

            bp.OwnedBots.Add(bot);
            bp.Selected = bp.OwnedBots.Count - 1;

            args.Player?.SendInfoMessage(string.Format(Messages.BotSuccessCreateNew, bot.Name));
        }
        #endregion

        #region Start
        private static void StartBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCreate)) {
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
        #endregion

        #region Stop
        private static void StopBot(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCreate)) {
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
        #endregion

        #region Delete
        private static void ConfirmedDelete(object obj) {
            BotCommandArgs args = new BotCommandArgs((CommandArgs)obj);
            
            var player = args.BPlayer;
            player.SelectedBot.Shutdown();
            player.OwnedBots.RemoveAt(player.SelectedDelete);
            player.SPlayer?.SendSuccessMessage("Successfully deleted bot.");  
        }

        private static void RefuseDelete(object obj) {
            CommandArgs args = (CommandArgs)obj;

            // Remove /confirm from awaiting responses
            args.Player.AddResponse("confirm", null);
        }

        private static void DeleteBot(BotCommandArgs args) {

            // Example command
            //        0           1
            // /bot delete "Michael Jackson"
            //        0       1       2
            // /bot delete Michael Jackson

            if (!args.Player.HasPermission(Permissions.BotCreate)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Delete, Messages.DeleteExample))
                return;

            if (args.BPlayer.OwnedBots.Count == 0) {
                args.Player?.SendErrorMessage(Messages.NoOwnedBots);
                return;
            }

            var player = Program.Players[args.Player.Index];
            List<string> currentSection = args.CurrentSection;
            if (currentSection.Count == 1 && player.Selected != -1) {
                // Default selected bot
                player.SelectedDelete = player.Selected;
                args.Player?.SendSuccessMessage("Currently selected bot will be deleted upon confirmation: /confirm or /deny.");

                args.Player?.AddResponse("confirm", new Action<object>(ConfirmedDelete));
                args.Player?.AddResponse("deny", new Action<object>(RefuseDelete));
            }
            else if (currentSection.Count > 1) {
                // User specified bot
                string nameOrIndex = string.Join(" ", currentSection.Skip(1)).Trim('"');
                List<Bot> foundbots = player.GetBotFromIndexOrName(nameOrIndex);

                if (foundbots.Count == 0) {
                    args.Player?.SendErrorMessage(string.Format(Messages.NoBotsFound, nameOrIndex));
                }
                else if (foundbots.Count > 1) {
                    args.Player?.SendErrorMessage(Messages.MultipleBotsFound);
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
        #endregion

        #region Record
        private static void RecordMaster(BotCommandArgs args) {

            // Example command:
            //             0              1
            // /bot     record     start/stop/play

            if (!args.Player.HasPermission(Permissions.BotRecord)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Record, Messages.NoExample))
                return;

            if (args.CurrentSection.Count == 2) {
                switch (args.CurrentSection[1]) {
                    case "start":
                        StartRecording(args);
                        break;
                    case "stop":
                        StopRecording(args);
                        break;
                    case "play":
                        PlayRecording(args);
                        break;
                    default:
                        args.Player.MultiMsg(Messages.Record, Color.Yellow);
                        break;
                }
            }
            else {
                args.Player.MultiMsg(Messages.Record, Color.Yellow);
            }
            
        }
        private static void StartRecording(BotCommandArgs args) {
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
            var bot = args.SelectedBot;
            if (bot != null) {
                bot.recording = false;
                args.Player?.SendSuccessMessage(string.Format(Messages.BotSuccessStopRecording, bot.Name));
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }

        private static void PlayRecording(BotCommandArgs args) {
            var bot = args.SelectedBot;
            if (bot != null) {
                if (bot.recordedPackets.Count == 0) {
                    args.Player?.SendErrorMessage("No recorded actions found.");
                    return;
                }

                bot.StartPlaybackTimer();

                args.Player?.SendSuccessMessage($"Selected bot \"{bot.Name}\" is now playing back player actions.");
            } 
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
        #endregion

        #region Copy
        private static void Copy(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotCopy)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Copy, Messages.CopyExample))
                return;

            TSPlayer tstarget;
            List<string> currentSection = args.CurrentSection;

            if (currentSection.Count > 1) {
                if (!args.Player.HasPermission(Permissions.BotCopyOther)) {
                    args.Player?.SendErrorMessage("You do not have permission to copy other players.");
                    return;
                }

                string namewithspaces = string.Join(" ", currentSection).Trim('"');

                var found = TSPlayer.FindByNameOrID(namewithspaces);
                if (found.Count == 0) {
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

            if (!Program.Players[tstarget.Index].canBeCopied
                && !args.Player.HasPermission(Permissions.BotBypassCopy)
                && tstarget.Index != args.Player.Index) {
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
        #endregion

        #region Chat
        private static void Chat(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotChat)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Select, Messages.SelectExample))
                return;

            var bot = args.SelectedBot;
            List<string> currentSection = args.CurrentSection;
            if (bot != null) {
                if (currentSection.Count == 1) {
                    args.Player?.SendErrorMessage("Expected message or command as input.");
                    return;
                }
                var message = string.Join(" ", currentSection.Skip(1)).Trim('"');

                bot.Actions.Chat(message);
            }
            else {
                args.Player?.SendErrorMessage(Messages.BotErrorNotFound);
            }
        }
        #endregion

        #region Teleport
        private static void Teleport(BotCommandArgs args) {
            if (!args.Player.HasPermission(Permissions.BotTeleport)) {
                args.Player?.SendErrorMessage(Messages.NoPermission);
                return;
            }

            if (NeedsHelpOrExample(args, Messages.Teleport, Messages.TeleportExample))
                return;

            var bot = args.SelectedBot;
            List<string> currentSection = args.CurrentSection;
            if (bot != null) {
                TSPlayer tstarget;
                if (currentSection.Count > 1) {

                    if (currentSection.Count == 3 && int.TryParse(args.Parameters[1], out int x) && int.TryParse(args.Parameters[2], out int y)) {
                        Vector2 newPos = new Vector2(x, y);
                        bot.Actions.Teleport(newPos);

                        args.Player?.SendSuccessMessage($"Teleporting bot to ({x}, {y}).");
                        return;
                    }

                    string namewithspaces = string.Join(" ", currentSection.Skip(1)).Trim('"');

                    var found = TSPlayer.FindByNameOrID(namewithspaces);
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
                if (tstarget.Index != args.Player.Index
                    && !Program.Players[tstarget.Index].canBeTeleportedTo
                    && !args.Player.HasPermission(Permissions.BotBypassTeleport)) {
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
        #endregion

        #region Unused

        public static void Debug(CommandArgs args) {
            BotCommandArgs botArgs = new BotCommandArgs(args);
            botArgs.SelectedBot.Actions.UpdateInventory();
        }

        //private static void Save(BotCommandArgs args) {
        //    if (!args.Player.HasPermission(Permissions.BotSave) || !args.Player.HasPermission(Permissions.BotUse)) {
        //        args.Player?.SendErrorMessage(Messages.NoPermission);
        //        return;
        //    }
        //    //FileWriter.BTSPlayerToStream(Program.Players[args.Player.Index]);
        //    //args.Player?.SendSuccessMessage("Successfully saved player data.");
        //}

        //private static void Prune(BotCommandArgs args) {
        //    if (!args.Player.HasPermission(Permissions.BotSavePruning) || !args.Player.HasPermission(Permissions.BotUse)) {
        //        args.Player?.SendErrorMessage(Messages.NoPermission);
        //        return;
        //    }

        //    BTSPlayer bp = Program.Players[args.Player.Index];
        //    DateTime prune = DateTime.Now;

        //    if (args.Parameters.Count == 2) {
        //        prune = prune.AddDays(-7);
        //        args.Player?.SendInfoMessage("Defaulting prune to one week.");
        //    }
        //    else {
        //        int year = 0;
        //        int month = 0;
        //        int day = 0;
        //        int hour = 0;

        //        List<string> time = args.Parameters.GetRange(2, args.Parameters.Count);
        //        foreach (string s in time) {
        //            switch (s[s.Length - 1]) {
        //                case 'h':
        //                    if (int.TryParse(s.TrimEnd('h'), out int hours)) {
        //                        hour += hours;
        //                    }
        //                    else {
        //                        args.Player?.SendErrorMessage($"Invalid hour format: \"{s}\"");
        //                    }
        //                    break;
        //                case 'd':
        //                    if (int.TryParse(s.TrimEnd('d'), out int days)) {
        //                        day += days;
        //                    }
        //                    else {
        //                        args.Player?.SendErrorMessage($"Invalid day format: \"{s}\"");
        //                    }
        //                    break;
        //                case 'm':
        //                    if (int.TryParse(s.TrimEnd('m'), out int months)) {
        //                        month += months;
        //                    }
        //                    else {
        //                        args.Player?.SendErrorMessage($"Invalid month format: \"{s}\"");
        //                    }
        //                    break;
        //                case 'y':
        //                    if (int.TryParse(s.TrimEnd('y'), out int years)) {
        //                        year += years;
        //                    }
        //                    else {
        //                        args.Player?.SendErrorMessage($"Invalid year format: \"{s}\"");
        //                    }
        //                    break;
        //                default:
        //                    args.Player?.SendErrorMessage($"Invalid time format: \"{s}\"");
        //                    break;
        //            }
        //        }

        //        prune = prune.AddHours(hour * -1);
        //        prune = prune.AddDays(day * -1);
        //        prune = prune.AddMonths(month * -1);
        //        prune = prune.AddYears(year * -1);

        //    }

        //    args.Player?.SendInfoMessage($"Prune set to: {prune.Year}, {prune.Month}, {prune.Day}, {prune.Hour}. All prior saves will be moved to the prune folder.");
        //    PluginUtils.PruneSaves(prune); 
        //}
        #endregion
    }
}
