using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Program {
    public static class Permissions {
        public const string Bot = "bot";
        public const string BotCreate = "bot.create";

        public const string BotRecord = "bot.record";

        public const string BotChat = "bot.chat";

        public const string TogglePlayerFields = "bot.toggle";

        public const string BotCopy = "bot.copy";
        public const string BotCopyOther = "bot.copy.other";

        public const string BotBypassCopy = "bot.bypass.copy"; // bypass BTSPlayer.CanBeCopied
        public const string BotBypassTeleport = "bot.bypass.teleportation"; // bypass BTSPlayer.CanBeTeleportedTo

        public const string BotTeleport = "bot.teleport";

        public const string BotIgnore = "bot.ignore";

        public const string InfoOtherPlayers = "bot.checkOtherInfo";

        public const string BotForeach = "bot.foreach";

        public const string ReloadUserDBEntry = "bot.admin.db.reload";
    }
}
