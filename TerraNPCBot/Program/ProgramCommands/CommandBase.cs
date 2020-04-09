using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraNPCBot.Program.ProgramCommands {
    public interface ICommand {
        string GetHelp { get; }
        
        string GetExample { get; }

        void Execute();
    }
}
