using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TShockAPI;
using TerrariaApi.Server;

namespace TerraNPCBot.Program {
    public class PluginHooks {
        public static void OnJoin(GreetPlayerEventArgs args) {
            if (TShock.Players[args.Who] == null)
                return;

            foreach (var bot in PluginMain.GlobalRunningBots) {
                bot.RequestJoinPackets(args.Who);
            }

            var db = PluginMain.DB;
            var id = TShock.Players[args.Who].Account.ID;

            BTSPlayer joiningPlayer = new BTSPlayer(args.Who);
            if (!db.HasUserEntry(id)) {
                db.AddUserEntry(joiningPlayer, id);
            }
            else {
                joiningPlayer = db.LoadUserEntry(id, args.Who);
            }

            PluginMain.Players[args.Who] = joiningPlayer;
        }

        public static void OnLeave(LeaveEventArgs args) {
            if (TShock.Players[args.Who] == null)
                return;

            var db = PluginMain.DB;
            var id = TShock.Players[args.Who].Account.ID;
            var playerLeaving = PluginMain.Players[args.Who];

            if (!db.HasUserEntry(id)) {
                db.AddUserEntry(playerLeaving, id);
            }
            else {
                db.UpdateUserEntry(playerLeaving, id);
            }

            foreach (var bot in playerLeaving.OwnedBots) {
                bot.Shutdown();
            }
            playerLeaving = null;
        } 

        public static void OnGetData(GetDataEventArgs args) {
            Bot p = PluginMain.Players[args.Msg.whoAmI]?.OwnedBots?.FirstOrDefault(x => x.recording);
            if (p != null) {
                using (MemoryStream m = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)) {

                    if (args.Msg.whoAmI != p.Owner) return;

                    if (p.timerBetweenPackets == null) {
                        p.timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p.timerBetweenPackets.Start();

                        p.previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Buffer.BlockCopy(args.Msg.readBuffer, args.Index, p.previousPacket.Buffer, 0, args.Length);
                    }  // first packet recieved
                    else {
                        p.timerBetweenPackets.Stop();
                        p.recordedPackets.Add(new RecordedPacket(p.previousPacket, (uint)p.timerBetweenPackets.ElapsedMilliseconds));

                        p.timerBetweenPackets = new System.Diagnostics.Stopwatch();
                        p.timerBetweenPackets.Start();

                        p.previousPacket = new StreamInfo(new byte[args.Length], (byte)args.MsgID);
                        Buffer.BlockCopy(args.Msg.readBuffer, args.Index, p.previousPacket.Buffer, 0, args.Length);
                    }  // all packets onwards
                }
            }

        }
    }
}
