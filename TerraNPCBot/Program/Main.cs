﻿using System;
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

            TShockAPI.Commands.ChatCommands.Add(new Command(Permissions.Bot, PluginCommands.AddServerCommand, "bot"));

            Config = Config.Load();

            // Magic numbers so no change pls ^_^
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
