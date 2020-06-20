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
    public class Program : TerrariaPlugin {

        #region Info
        public override string Name => "TerraNPCBot";
        public override Version Version => new Version(1, 0);
        public override string Author => "nyan";
        #endregion

        public const string PluginFolderName = "TerraNPCBot";
        public static readonly string PluginFolderLocation = Path.Combine(TShock.SavePath, PluginFolderName);
        public static readonly string PluginSaveFolderLocation = Path.Combine(PluginFolderLocation, "saves");
        public static readonly string PluginPrunedSaveFolderLocation = Path.Combine(PluginFolderLocation, "prunedsaves");

        public static BTSPlayer[] Players = new BTSPlayer[256];
        public static Bot[] Bots = new Bot[256];
        public static List<Bot> GlobalRunningBots = new List<Bot>();

        public static Dictionary<string, string> ServersByPorts = new Dictionary<string, string> {
            { "7777", "Nexus" },
            { "7770", "Items" },
            { "7771", "Freebuild" },
            { "7779", "Freebuild+" },
            { "7773", "PvP" },
            { "7772", "PvE" },
            { "7774", "CTF" }
        };
        public static Dictionary<string, int> PortsByServers = new Dictionary<string, int> {
            { "nexus", 7777 },
            { "items", 7770 },
            { "freebuild", 7771 }, { "fb", 7771 },
            { "freebuild+", 7779 }, { "fb+", 7779 },
            { "pvp", 7773 },
            { "pve", 7772 },
            { "ctf", 7774 }
        };

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

            Commands.ChatCommands.Add(new Command(Permissions.Bot, PluginCommands.AddCommand, "bot"));
            Commands.ChatCommands.Add(new Command("bot.debug", PluginCommands.Debug, "debug"));

            // Call PluginCommands.CommandThread on a separate thread to handle commands
            //ThreadPool.QueueUserWorkItem(PluginCommands.CommandThread);
            // Same thing for packet queue
            //ThreadPool.QueueUserWorkItem(PacketBase.PacketSendThread);
        }
        protected override void Dispose(bool disposing) {
            if (disposing) {
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, PluginHooks.OnJoin);
                ServerApi.Hooks.ServerLeave.Deregister(this, PluginHooks.OnLeave);
                ServerApi.Hooks.NetGetData.Deregister(this, PluginHooks.OnGetData);
            }
        }
    }
}
