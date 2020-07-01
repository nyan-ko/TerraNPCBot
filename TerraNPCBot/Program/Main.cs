using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TShockAPI;
using TShockAPI.Hooks;
using TerrariaApi.Server;
using Terraria;
using System.Net;
using System.Net.Sockets;

namespace TerraNPCBot.Program
{
    [ApiVersion(2, 1)]
    public class PluginMain : TerrariaPlugin {

        #region Info
        public override string Name => "TerraNPCBot";
        public override Version Version => new Version(1, 0);
        public override string Author => "nyan";
        #endregion

        public const string PluginFolderName = "TerraNPCBot";
        public static readonly string PluginFolderLocation = Path.Combine(TShock.SavePath, PluginFolderName);

        public static BTSPlayer[] Players = new BTSPlayer[256];
        public static Bot[] Bots = new Bot[256];
        public static List<Bot> GlobalRunningBots = new List<Bot>();
        public static Database DB { get; private set; }
        public static Config Config { get; private set; }

        public static bool RunThreads { get; private set; }

        public PluginMain(Main game) : base(game) {

        }

        public override void Initialize() {
            RunThreads = true;

            if (!Directory.Exists(PluginFolderLocation)) {
                Directory.CreateDirectory(PluginFolderLocation);
            }
            ServerApi.Hooks.NetGreetPlayer.Register(this, PluginHooks.OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, PluginHooks.OnLeave);
            ServerApi.Hooks.NetGetData.Register(this, PluginHooks.OnGetData);

            TShockAPI.Commands.ChatCommands.Add(new Command(PluginCommands.AddServerCommand, "bot"));
            TShockAPI.Commands.ChatCommands.Add(new Command(PluginCommands.ListCommands, "blist"));
            TShockAPI.Commands.ChatCommands.Add(new Command(PluginCommands.ToggleFields, "toggle"));
            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.ReloadUserDBEntry, PluginCommands.LoadUserDBEntry, "reloaduser"));

            Config = Config.Load();

            DB = new Database(Config.DBConnectionString);

            // Call PluginCommands.CommandThread on a separate thread to handle commands
            ThreadPool.QueueUserWorkItem(PluginCommands.CommandThread);
            // Same thing for packet queue
            ThreadPool.QueueUserWorkItem(PacketBase.PacketSendThread);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                RunThreads = false;

                ServerApi.Hooks.NetGreetPlayer.Deregister(this, PluginHooks.OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, PluginHooks.OnLeave);
                ServerApi.Hooks.NetGetData.Deregister(this, PluginHooks.OnGetData);

                Config.Save();
            }
        }
    }
}
