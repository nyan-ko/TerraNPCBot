using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace TerraNPCBot.Data {
    public class BotCommandArgs : CommandArgs {

        /// <summary>
        /// Gets the player who did the command's selected bot.
        /// </summary>
        public Bot SelectedBot {
            get {
                if (Player.RealPlayer)
                    return Program.Program.Players[Player.Index]?.SelectedBot;
                return BTSPlayer.BTSServerPlayer.SelectedBot;
            }
        }

        /// <summary>
        /// Gets the player who did the command as a BTSPlayer.
        /// </summary>
        public BTSPlayer BPlayer {
            get {
                if (Player.RealPlayer)
                    return Program.Program.Players[Player.Index];
                return BTSPlayer.BTSServerPlayer;
            }
        }

        #region Splitters
        private int indexInSplitterList = 0;

        /// <summary>
        /// The index of the lower bound splitter in the current section.
        /// </summary>
        public int LowerSplitterBound => IndicesOfSplitters[indexInSplitterList];

        /// <summary>
        /// The index of the upper bound splitter in the current section.
        /// </summary>
        public int UpperSplitterBound => IndicesOfSplitters[indexInSplitterList + 1];

        /// <summary>
        /// Gets a list of the indexes of the splitters, if any, in the parameter list.
        /// </summary>
        public List<int> IndicesOfSplitters { get; } = new List<int>();

        /// <summary>
        /// Gets the range contained in the current split section (excluding any splitter characters).
        /// </summary>
        public List<string> CurrentSection => Parameters.GetRange(LowerSplitterBound + 1, UpperSplitterBound - LowerSplitterBound - 1);

        /// <summary>
        /// Finds the indices of all splitter ("|") characters in the parameter list.
        /// </summary>
        private void FindSplitters() {
            // Add lower bound for splitter sections
            IndicesOfSplitters.Add(-1);
            for (int i = 0; i < Parameters.Count; i++) {
                if (Parameters[i] == "|")
                    IndicesOfSplitters.Add(i);
            }
            // Add upper bound for splitter sections
            IndicesOfSplitters.Add(Parameters.Count);
        }

        public void OffsetToNextSection() {
            indexInSplitterList++;
            if (indexInSplitterList > IndicesOfSplitters.Count - 1) {
                indexInSplitterList = IndicesOfSplitters.Count - 1;
            }
            OffsetParameters = Parameters.Skip(LowerSplitterBound + 1).ToList();
        }
        #endregion

        /// <summary>
        /// The parameter list with the appropriate offset for linked commands.
        /// </summary>
        public List<string> OffsetParameters { get; private set; }

        public BotCommandArgs(CommandArgs baseCommand) : base(baseCommand.Message, baseCommand.Player, baseCommand.Parameters) {
            OffsetParameters = Parameters;

            FindSplitters();
        }
    }

}
