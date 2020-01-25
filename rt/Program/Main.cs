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

namespace rt.Program
{
    [ApiVersion(2, 1)]
    public class Program : TerrariaPlugin {

        #region Info
        public override string Name => "TerraNPCBot";
        public override Version Version => new Version(1, 0);
        public override string Author => "nyan";
        #endregion

        public const byte PluginStreamVersion = 1;

        public const int BufferSize = 262144;

        public const string PluginFolderName = "TerraNPCBot";
        public static readonly string PluginFolderLocation = Path.Combine(TShock.SavePath, PluginFolderName);
        public static readonly string PluginSaveFolderLocation = Path.Combine(PluginFolderLocation, "saves");
        public static readonly string PluginPrunedSaveFolderLocation = Path.Combine(PluginFolderLocation, "prunedsaves");

        public static BTSPlayer[] Players = new BTSPlayer[256];

        public Program(Main game) : base(game) {

        }

        public override void Initialize() {
            if (!Directory.Exists(PluginFolderLocation)) {
                Directory.CreateDirectory(PluginFolderLocation);
                Directory.CreateDirectory(PluginSaveFolderLocation);
                Directory.CreateDirectory(PluginPrunedSaveFolderLocation);
            }

            ServerApi.Hooks.ServerJoin.Register(this, PluginHooks.OnJoin);
            ServerApi.Hooks.ServerLeave.Register(this, PluginHooks.OnLeave);
            ServerApi.Hooks.NetGetData.Register(this, PluginHooks.OnGetData);

            Commands.ChatCommands.Add(new Command("bot", PluginCommands.BotMaster, "bot"));

            //Commands.ChatCommands.Add(new Command("", PluginCommands.Start, "startbot"));
            //Commands.ChatCommands.Add(new Command("", PluginCommands.Stop, "stopbot"));

            //Commands.ChatCommands.Add(new Command("", PluginCommands.Delegation, "s"));
            //Commands.ChatCommands.Add(new Command("", PluginCommands.Record, "e"));

        }

    }
}
