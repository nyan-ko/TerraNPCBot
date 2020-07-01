using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TerraNPCBot.Utils;
using TerraNPCBot.Data;

namespace TerraNPCBot.Program.Commands {
    public class Record : BaseCommand {
        public override bool ValidForeachAction => false;

        public override string HelpMessage => Messages.Record;

        public override string ExampleMessage => Messages.NoExample;

        public override string InitialPermission => Permissions.BotRecord;

        protected override void Execute(BotCommandArgs args) {
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
                        args.Player.SendMultipleMessage(Messages.Record, Color.Yellow);
                        break;
                }
            }
            else {
                args.Player.SendMultipleMessage(Messages.Record, Color.Yellow);
            }
        }

        private static void StartRecording(BotCommandArgs args) {
            var bot = args.SelectedBot;
            if (bot != null) {
                if (bot.recording) {
                    args.Player?.SendErrorMessage($"Selected bot \"{bot.Name}\" is already recording.");
                    return;
                }

                foreach (var checkRecording in args.BPlayer.OwnedBots) {
                    if (checkRecording.recording) {
                        args.Player?.SendErrorMessage($"Bot \"{checkRecording.Name}\" is already recording.");
                        return;
                    }
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
    }
}
