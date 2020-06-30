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
                    if (CommandsByTag.TryGetValue(args.Parameters[i], out BaseCommand command)) {
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
