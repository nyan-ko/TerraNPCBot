using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.Hooks;
using TerrariaApi.Server;
using Terraria;
using System.Net;
using System.Net.Sockets;

namespace TerraNPCBot.Program
{
    [ApiVersion(2, 1)]
    public class Program : TerrariaPlugin {

        #region Info
        public override string Name => "TerraNPCBot";
        public override Version Version => new Version(1, 0);
        public override string Author => "nyan";
        #endregion

        public const byte PluginStreamVersion = 1;
        public const int BufferSize = 262143;

        public const string PluginFolderName = "TerraNPCBot";
        public static readonly string PluginFolderLocation = Path.Combine(TShock.SavePath, PluginFolderName);
        public static readonly string PluginSaveFolderLocation = Path.Combine(PluginFolderLocation, "saves");
        public static readonly string PluginPrunedSaveFolderLocation = Path.Combine(PluginFolderLocation, "prunedsaves");

        public static BTSPlayer[] Players = new BTSPlayer[256];
        public static Bot[] Bots = new Bot[256];
        public static List<Bot> GlobalRunningBots = new List<Bot>();

        public Program(Main game) : base(game) {

        }

        public override void Initialize() {
            if (!Directory.Exists(PluginFolderLocation)) {
                Directory.CreateDirectory(PluginFolderLocation);
                Directory.CreateDirectory(PluginSaveFolderLocation);
                Directory.CreateDirectory(PluginPrunedSaveFolderLocation);
            }

            ServerApi.Hooks.NetGreetPlayer.Register(this, PluginHooks.OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, PluginHooks.OnLeave);
            ServerApi.Hooks.NetGetData.Register(this, PluginHooks.OnGetData);

            Commands.ChatCommands.Add(new Command(Permissions.Bot, PluginCommands.BotMaster, "bot"));
            Commands.ChatCommands.Add(new Command("", PluginCommands.Debug, "debug"));
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                
            }
        }
    }
}
