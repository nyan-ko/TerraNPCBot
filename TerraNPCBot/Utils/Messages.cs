using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Utils {
    public static class Messages {
        public static List<string> Record = new List<string> {
            "Sub-command for recording and playing back sequence of player actions through a bot.",
            "Usage: /bot record start/stop/play",
            "/bot record start - Starts recording owner's actions.",
            "/bot record stop - Stops recording owner's actions.",
            "/bot record play - Plays a recorded sequence of actions."
        };

        public static List<string> Master1 = new List<string> {
            "Command for working with server bots.",
            "Usage: /bot new/start/stop/delete/save/chat/teleport/select/copy/record",
            "/bot new - Creates a new bot and automatically selects it.",
            "/bot start - Starts running selected bot and has it join the server.",
            "/bot stop - Stops running selected bot and kicks it from the server.",
            "/bot delete - Deletes the selected bot.",
            "Page (1/3)"
        };
        public static List<string> Master2 = new List<string> {
            "/bot save - Saves player and owned bot data.",
            "/bot chat - Makes selected bot output inputted message; (most) commands work!",
            "/bot teleport - Teleports selected bot to a specified player.",
            "Page (2/3)"
        };
        public static List<string> Master3 = new List<string> {
            "/bot select - Selects a specified bot from owned bots.",
            "/bot copy - Copies a specified player's inventory to display on selected bot.",
            "/bot record - Records and playsback player actions through selected bot.",
            "Page (3/3)"
        };

        public static List<string> Select = new List<string> {
            "Sub-command for selecting bot given position or name.",
            "Usage: /bot select <number>/<name>",
            "/bot select <number> - Selects a bot from your owned bots according to its position in the list.",
            "/bot select <name> - Selects a bot with a match or close match to the given name*.",
            "*Using bot names will only select the first bot found if multiple names match.",
        };
        public static List<string> SelectExample = new List<string> {
            "Example: /bot select 7 - Will select 7th bot in owned bots.",
            "Example: /bot select Terraria Bot - Will select bot named Terraria Bot.",
            "Example: /bot select Terrar - Will similarly select bot named Terraria Bot."
        };

        public static List<string> New = new List<string> {
            "Sub-command for creating new bots.",
            "Usage: /bot new <name> <world> - Name and world are optional.",
            "/bot new <name> - Creates a bot with the given name and selects it.",
            "/bot new <name> <world> - Creates a bot with the given name, sets it to target the given world, and selects it."
        };
        public static List<string> NewExample = new List<string> {
            "Example: /bot new - Will default bot name to \"Michael_Jackson\" and world to Nexus.",
            "Example: /bot new NewBot - Will create bot with name \"NewBot\".",
            "Example: /bot new NewBot fb - Will create bot with name \"NewBot\" that will join Freebuild when started."
        };

        public static List<string> Chat = new List<string> {
            "Sub-command for talking through bots.",
            "Usage: /bot chat <message>",
            "/bot chat <message> - Outputs message into chat through bot. Certain commands supported."
        };
        public static List<string> ChatExample = new List<string> {
            "Example: /bot chat Hello world! - Bot will say \"Hello world!\".",
            "Example: /bot chat /i dirt - Bot will execute \"/i dirt\"."
        };

        public static List<string> Teleport = new List<string> {
            "Sub-command for teleporting bots.",
            "Usage: /bot teleport <target name>/<position> - Default target is bot owner.",
            "/bot teleport <target name> - Teleports bot to online player with a match or close match to the given name*.",
            "/bot teleport <position> - Teleports bot to specified position in x-y coordinates.",
            "*Specificity is recommended for players with similar names (e.g. john, johnny)."
        };
        public static List<string> TeleportExample = new List<string> {
            "Example: /bot teleport - Will teleport bot to its owner (i.e. whoever executed the command).",
            "Example: /bot teleport Cy - Will teleport bot to Cy, provided he is online.",
            "Example: /bot teleport 4100 600 - Will teleport bot to (4100, 600)."
        };

        public static List<string> Copy = new List<string> {
            "Sub-command for copying player inventories and appearance.",
            "Usage: /bot copy <target name> - Default target is bot owner.",
            "/bot copy - Copies owner.",
            "/bot copy <target name> - Copies an online player with a match or close match to the given name*.",
            "*Specificity is recommended for players with similar names (e.g. john, johnny)."
        };
        public static List<string> CopyExample = new List<string> {
            "Example: /bot copy - Will copy owner's inventory, vanity, dyes, etc..",
            "Example: /bot copy Cy - Will copy Cy's inventory, vanity, dyes, etc., provided he is online."
        };

        public static List<string> NoExample = new List<string> {
            "No examples exist for this command.",
            "Follow usage guide."
        };

        public static readonly string NoPermission = "You do not have permission to use this command.";
        public static readonly string BotErrorGeneric = "Something went wrong: {0}. Report to staff if you see this.";

        public static readonly string BotErrorNotFound = "Could not find selected bot.";
        public static readonly string BotErrorNotRunning = "Selected bot \"{0}\" is not currently running.";
        public static readonly string BotErrorAlreadyRunning = "Selected bot \"{0}\" is already running.";
        public static readonly string BotErrorCouldNotStart = "Selected bot \"{0}\" could not establish a connection. Retry?";

        public static readonly string BotSuccessCreateNew = "Created a new bot with name \"{0}\" targeting port {1}.";

        public static readonly string BotSuccessStopped = "Stopped selected bot \"{0}\".";
        public static readonly string BotSuccessStarted = "Started selected bot \"{0}\".";

        public static readonly string BotSuccessRecording = "Selected bot \"{0}\" is now recording.";
        public static readonly string BotSuccessStopRecording = "Selected bot \"{0}\" has stopped recording.";

    }
}
