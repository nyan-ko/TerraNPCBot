using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Program {
    public static class Messages {

        public const string ToggleHelp = 
            "Command to toggle certain player fields.\\" +
            "/toggle teleportable - Toggles whether bots can teleport to you.\\" +
            "/toggle copyable - Toggles whether bots can copy you.\\" +
            "/toggle ignore - Toggles whether bots are visible for you.";

        #region Help
        public static string Master =>
            "Command for working with server bots.\\" +
            "For a comprehensive list of all subcommands, type /blist.\\" +
            "-----\\" +
            "Most commands come with their own usage guide and examples, which can be\\" +
            "accessed by passing \"help\" or \"example\" to a command.\\" +
            "Example: /bot new help/example - Get help or examples for the new bot sub-command.\\" +
            "-----\\" +
            $"Commands may also be linked together when separated with a splitter parameter: \"{PluginMain.Config.CommandSplitterCharacter}\".\\" +
            "This will attempt to execute all given commands one after the other.\\" + 
            $"Example: /bot new \"Terraria Bot\" {PluginMain.Config.CommandSplitterCharacter} start {PluginMain.Config.CommandSplitterCharacter} copy - Creates a new bot, starts it, and copies the owner.";
        // too lazy to make the string formatting clean :)

        public const string CommandList1 =
            "/bot new - Creates a new bot.\\" +
            "/bot start - Starts a bot and has them \"join\" the server.\\" +
            "/bot stop - Stops a bot and makes them leave.\\" + 
            "/bot delete - Deletes a bot after confirmation.\\" +
            "Page (1/4)";

        public const string CommandList2 =
            "/bot chat - Displays message through bot and above their head; commands do not work.\\" + 
            "/bot copy - Copies players appearances and inventory and displays on bot.\\" +
            "/bot teleport - Teleports bot to a player or location in the world.\\" +
            "/bot rename - Renames a bot.\\" +
            "Page (2/4)";

        public const string CommandList3 =
            "/bot select - Selects a specified bot.\\" + 
            "/bot list - Lists all your bots.\\" +
            "/bot info - Provides info about yourself or a specific bot.\\" +
            "/bot record - Allows recordings of player actions and playback through a bot.\\" +
            "Page (3/4)";

        public const string CommandList4 =
            "/bot groupbots - Groups a selection of bots for use with /foreach.\\" +
            "/bot foreach - Executes actions for each bot from /groupbots.\\" +
            "Page (4/4)";
        #endregion

        #region Record
        public const string Record =
            "Sub-command for recording and playing back sequence of player actions through a bot.\\" +
            "Usage: /bot record start | stop | play\\" +
            "/bot record start - Starts recording owner's actions.\\" +
            "/bot record stop - Stops recording owner's actions.\\" +
            "/bot record play - Plays a recorded sequence of actions.";
        #endregion

        #region Delete
        public const string Delete =
            "Sub-command for selecting bots given position or name to delete.\\" +
            "Usage: /bot delete [index] | [name] - Number or name are optional.\\" +
            "/bot delete - Currently selected bot will be deleted.\\" +
            "/bot delete [index] - Selects a bot from your owned bots according to its position in the list.\\" +
            "/bot delete [name] - Selects a bot with a match or close match to the given name.\\" +
            "*Bots will only be deleted upon receiving confirmation through /confirm.";


        public const string DeleteExample =
            "Example: /bot delete - Selects currently selected bot for deletion.\\" +
            "Example: /bot delete 2 - Selects the 2nd bot you own for deletion.\\" +
            "Example: /bot delete \"Michael Jackson\" - Selects a bot named Michael Jackson for deletion.";
        
        #endregion

        #region Select
        public const string Select =
            "Sub-command for selecting bots given position or name.\\" +
            "Usage: /bot select <index> | <name>\\" +
            "/bot select <index> - Selects a bot from your owned bots according to its position in the list.\\" +
            "/bot select <name> - Selects a bot with a match or close match to the given name.";

        public const string SelectExample =
            "Example: /bot select 2 - Will select 2nd bot in owned bots.\\" +
            "Example: /bot select Terraria Bot - Will select bot named Terraria Bot.\\" +
            "Example: /bot select Terrar - Will similarly select bot named Terraria Bot.";

        #endregion

        #region New
        public const string New =
            "Sub-command for creating new bots.\\" +
            "Usage: /bot new [name] - Default creates a bot named Michael Jackson.\\" +
            "/bot new [name] - Creates a bot with the given name and selects it.";

        public const string NewExample =
            "Example: /bot new - Will default bot name to \"Michael Jackson\" and world to Nexus.\\" +
            "Example: /bot new \"New Bot\" - Will create bot with name New Bot.";
        
        #endregion

        #region Chat
        public const string Chat =
            "Sub-command for talking through bots.\\" +
            "Usage: /bot chat <message>\\" +
            "/bot chat <message> - Outputs message into chat through bot. Certain commands supported.";

        public const string ChatExample =
            "Example: /bot chat Hello world! - Bot will say \"Hello world!\".\\";
        
        #endregion

        #region Teleport
        public const string Teleport =
            "Sub-command for teleporting bots.\\" +
            "Usage: /bot teleport [target name] | [position] - Default target is bot owner.\\" +
            "/bot teleport [target name] - Teleports bot to online player with a match or close match to the given name*.\\" +
            "/bot teleport [position] - Teleports bot to specified position in x-y coordinates.\\" +
            "*Specificity is recommended for players with similar names (e.g. Cy, CyborgEmperor).";

        public const string TeleportExample =
            "Example: /bot teleport - Will teleport bot to its owner.\\" +
            "Example: /bot teleport Cy - Will teleport bot to Cy, provided he is online.\\" +
            "Example: /bot teleport 4100 600 - Will teleport bot to (4100, 600).";
        
        #endregion

        #region Copy
        public const string Copy =
            "Sub-command for copying player inventories and appearance.\\" +
            "Usage: /bot copy [target name] - Default target is bot owner.\\" +
            "/bot copy - Copies owner.\\" +
            "/bot copy [target name] - Copies an online player with a match or close match to the given name*.\\" +
            "*Specificity is recommended for players with similar names (e.g. Cy, CyborgEmperor).";

        public const string CopyExample =
            "Example: /bot copy - Will copy owner's inventory, vanity, dyes, etc.\\" +
            "Example: /bot copy Cy - Will copy Cy's inventory, vanity, dyes, etc., provided he is online.";
        
        #endregion

        #region Ignore
        public const string Ignore = 
            "Sub-command for ignoring bots.\\" +
            "Usage: /bot ignore\\"+
            "/bot ignore - Toggles visibility of bots for you.";
        #endregion

        #region Group Bots
        public const string GroupBots =
            "Sub-command for grouping a selection of bots together for use with /foreach.\\" +
            "Usage: /bot groupbots [indexes] - Default selects all owned bots.\\" +
            "/bot groupbots - Selects all owned bots.\\" + 
            "/bot groupbots [index1, index2, index3...] - Selects specified bots according to their index.";

        public const string GroupBotsExample =
            "Example: /bot groupbots - Selects all owned bots.\\" + 
            "Example: /bot groupbots 1 3 4 - Selects bots at indexes 1, 3, and 4.";
        #endregion

        #region Foreach
        public const string Foreach =
            "Sub-command for executing an action for each selected bot in a group. Used in conjuction with /groupbots.\\" +
            "Usage: /bot foreach (<list of commands>) - The list of commands must be enclosed in parentheses ().\\" +
            "/bot foreach (<list of commands>) - Executes the list of actions for each bot selected with /groupbots*.\\" +
            "*Only certain actions, such as /stop, /start, /copy, etc. work with foreach.\\";

        public const string ForeachExample =
            "Example: /bot foreach stop - Gets all selected bots to stop.\\" + 
            "Example: /bot foreach copy Cy - Gets all selected bots to copy Cy.";
        #endregion

        #region Rename
        public const string Rename =
            "Sub-command for renaming a bot.\\" +
            "Usage: /bot rename [new name] - By default, bot names will reset to Michael Jackson.\\" +
            "/bot rename - Sets bot name to Michael Jackson.\\" +
            "/bot rename [new name] - Sets bot name to a specified name.";

        public const string RenameExample =
            "Example: /bot rename - Sets bot name to Michael Jackson.\\" +
            "Example: /bot rename \"Terraria One Bot\" - Sets bot name to \"Terraria One Bot\".\\" + 
            "Example: /bot rename Terraria One Bot - Same as the above.";
        #endregion

        public const string NoExample =
            "No examples exist for this command.\\" +
            "Follow usage guide.";

        public const string NoBotsFound = "Could not find specified bot: \"{0}\"";
        public const string MultipleBotsFound = "Multiple bots matched your search criteria: ";
        public const string NoOwnedBots = "You do not have any owned bots.";

        public const string NoPermission = "You do not have permission to use this command.";
        public const string BotErrorGeneric = "Something went wrong: {0}. Report to staff if you see this.";

        public const string BotErrorNotFound = "Could not find selected bot.";
        public const string BotErrorNotRunning = "Selected bot \"{0}\" is not currently running.";
        public const string BotErrorAlreadyRunning = "Selected bot \"{0}\" is already running.";
        public const string BotErrorCouldNotStart = "Selected bot \"{0}\" could not establish a connection. Retry?";

        public const string BotSuccessCreateNew = "Created a new bot with name \"{0}\".";

        public const string BotSuccessStopped = "Stopped selected bot \"{0}\".";
        public const string BotSuccessStarted = "Started selected bot \"{0}\".";

        public const string BotSuccessRecording = "Selected bot \"{0}\" is now recording.";
        public const string BotSuccessStopRecording = "Selected bot \"{0}\" has stopped recording.";

    }
}
