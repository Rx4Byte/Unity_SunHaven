using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandExtension.Models
{
    public enum CommandState
    {
        None,
        Activated,
        Deactivated
    }

    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CommandState State { get; set; }
        public Command(string name, string description, CommandState state)
        {
            Name = name;
            Description = description;
            State = state;
        }
    }
}
