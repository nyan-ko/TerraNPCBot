using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerrariaApi.Server;
using Terraria;
using System.Net;
using System.Net.Sockets;

namespace rt.Program
{
    [ApiVersion(2, 1)]
    public class Program : TerrariaPlugin {

        public static BTSPlayer[] Players = new BTSPlayer[256];

        public Program(Main game) : base(game) {

        }

        public override void Initialize() {

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
