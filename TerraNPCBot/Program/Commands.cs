using System;
using System.Timers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Terraria.Chat;
using System.Collections.Concurrent;
using TerraNPCBot.Program.Commands;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;
using TShockAPI;

namespace TerraNPCBot.Program {
    public class PluginCommands {

        

        /// <summary>
        /// Dictionary of every command and their identifier.
        /// </summary>
        public static Dictionary<string, BaseCommand> CommandsByTag = new Dictionary<string, BaseCommand> {
            { "help", new Help() },
            { "ignore", new Ignore() },
            { "list", new List() },
            { "info", new Info() },
            { "select", new Select() },
            { "new", new NewBot() },
            { "start", new StartBot() },
            { "stop", new StopBot() },
            { "delete", new DeleteBot() }, { "del", new DeleteBot() },
            { "record", new Record() },
            { "copy", new Copy() },
            { "chat", new Chat() },
            { "teleport", new Teleport() }, { "tp", new Teleport() },
            { "groupbots", new GroupBots() },
            { "foreach", new Foreach() }
        };

        public static void ListCommands(CommandArgs args) {
            args.Player?.SendInfoMessage("Currently available commands for you:");
            args.Player?.SendMultipleMessage(PluginUtils.GetAvailableCommands(args.Player), Color.Yellow);
        }

        public static void ToggleFields(CommandArgs arg) {
            BotCommandArgs args = new BotCommandArgs(arg);
            if (args.Parameters.Count == 0) {
                args.Player?.SendMultipleMessage(Messages.ToggleHelp, Color.Yellow);
            }
            else {
                switch (args.Parameters[0].ToLower()) {
                    case "teleportable":
                        args.BPlayer.CanBeTeleportedTo = !args.BPlayer.CanBeTeleportedTo;
                        args.Player?.SendSuccessMessage($"Bots can {(args.BPlayer.CanBeTeleportedTo ? "now" : "no longer")} teleport to you.");
                        break;
                    case "copyable":
                        args.BPlayer.CanBeCopied = !args.BPlayer.CanBeCopied;
                        args.Player?.SendSuccessMessage($"You can {(args.BPlayer.CanBeCopied ? "now" : "no longer")} be copied by bots.");
                        break;
                    case "ignore":
                        args.BPlayer.ToggleBotVisibility(args.BPlayer.IgnoreBots);
                        args.Player?.SendSuccessMessage($"You are {(args.BPlayer.IgnoreBots ? "now" : "no longer")} ignoring bots.");
                        break;
                    default:
                        args.Player?.SendMultipleMessage(Messages.ToggleHelp, Color.Yellow);
                        break;
                }
            }
        }

        public static void LoadUserDBEntry(CommandArgs args) {
            string nameOrIndex = StringUtils.JoinAndTrimList(args.Parameters);
            List<TSPlayer> targets = TSPlayer.FindByNameOrID(nameOrIndex);

            if (!args.Player.HandleListFromSearches(nameOrIndex, targets))
                return;

            TSPlayer target = targets[0];

            PluginMain.Players[target.Index] = PluginMain.DB.LoadUserEntry(target.Account.ID, target.Index);

            args.Player?.SendSuccessMessage($"Successfully reloaded database entry for {target.Name}.");
        }

        private static BlockingCollection<BotCommandArgs> CommandQueue = new BlockingCollection<BotCommandArgs>();

        internal static void CommandThread(object state) {
            while (PluginMain.RunThreads) {
                if (!CommandQueue.TryTake(out BotCommandArgs args, -1)) {
                    continue;
                }

                if (args.Parameters.Count == 0 || !CommandsByTag.ContainsKey(args.Parameters[0])) {
                    CommandsByTag["help"].Invoke(args);
                    continue;
                }

                for (int i = 0; i < args.Parameters.Count; ++i) {
                    if (CommandsByTag.TryGetValue(args.Parameters[i].ToLower(), out BaseCommand command)) {
                        if (args.FromForeach ? !command.ValidForeachAction : false)
                            continue;
                        command.Invoke(args);
                        args.OffsetToNextSection();
                        i = args.LowerSplitterBound;
                    }
                }
            }
        }

        public static void AddServerCommand(CommandArgs command) => CommandQueue.Add(new BotCommandArgs(command));

        public static void AddInternalCommand(BotCommandArgs command) => CommandQueue.Add(command);
    }
}
