using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public string Key { get; }
        public string Usage { get; }
        public string Description { get; }
        public CommandState State { get; set; }
        public Action<string> Action { get; }
        public Command(string key, string description, string usage, CommandState state, Action<string> action)
        {
            Key = key;
            Usage = usage;
            Description = description;
            State = state;
            Action = action;
        }

        public void Invoke(string inputCommand)
        {
            Action(inputCommand);
        }
    }
}
