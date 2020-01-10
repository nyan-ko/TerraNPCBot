using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rt.Utils {
    public static class Messages {
        public static List<string> Record = new List<string> {
            "Sub-command for recording and playback sequence of player actions through a bot.",
            "Usage: /bot record start/stop/play",
            "/bot record start - Starts recording player actions.",
            "/bot record stop - Stops recording player actions.",
            "/bot record play - Plays a recorded sequence of actions."
        };
        public static List<string> Master = new List<string> {
            "Main command for creating and using server bots.",
            "Usage: /bot new/start/stop/select/copy/record/delete",
            "/bot new - Creates a new bot and automatically selects it.",
            "/bot start - Starts running selected bot and has it join the server.",
            "/bot stop - Stops running selected bot and kicks it from the server.",
            "/bot select - Selects a specified bot from your owned bots.",
            "/bot copy - Copies a specified player's inventory to display on selected bot.",
            "/bot record - Records and playsback player actions through selected bot.",
            "/bot delete - Deletes the selected bot."
        };

        public static readonly string BotErrorGeneric = "Something went wrong: {0}. Report to staff if you see this.";

        public static readonly string BotErrorNotFound = "Could not find selected bot.";
        public static readonly string BotErrorNotRunning = "Selected bot \"{0}\" is not currently running.";
        public static readonly string BotErrorAlreadyRunning = "Selected bot \"{0}\" is already running.";
        public static readonly string BotErrorCouldNotStart = "Selected bot \"{0}\" could not establish a connection. Retry?";

        public static readonly string BotSuccessCreateNew = "Created a new bot with name \"{0}\".";

        public static readonly string BotSuccessStopped = "Stopped selected bot \"{0}\".";
        public static readonly string BotSuccessStarted = "Started selected bot \"{0}\".";

        public static readonly string BotSuccessRecording = "Selected bot \"{0}\" is now recording.";
        public static readonly string BotSuccessStopRecording = "Selected bot \"{0}\" has stopped recording.";

    }
}
