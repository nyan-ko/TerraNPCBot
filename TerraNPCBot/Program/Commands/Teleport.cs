﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TerraNPCBot.Utils;
using Microsoft.Xna.Framework;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Teleport : BaseCommand {
        public override bool ValidForeachAction => true;

        public override string HelpMessage => Messages.Teleport;

        public override string ExampleMessage => Messages.TeleportExample;

        public override string InitialPermission => Permissions.BotTeleport;

        protected override void Execute(BotCommandArgs args) {
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

                    string nameOrIndex = StringUtils.JoinAndTrimList(currentSection.Skip(1));

                    var found = TSPlayer.FindByNameOrID(nameOrIndex);
                    if (!args.Player.HandleListFromSearches(nameOrIndex, found))
                        return;
                    tstarget = found[0];

                }
                else {
                    tstarget = args.Player;
                }
                if (tstarget.Index != args.Player.Index
                    && !PluginMain.Players[tstarget.Index].CanBeTeleportedTo
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
    }
}
