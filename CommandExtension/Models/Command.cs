using System;

namespace CommandExtension.Models
{
    public enum CommandState
    {
        None,
        Activated,
        Deactivated
    }

    public class Command(string key, string description, string usage, CommandState state, Action<string> action)
	{
		public string PrefixedKey { get; } = key;
		public string Usage { get; } = usage;
		public string Description { get; } = description;
		public CommandState State { get; set; } = state;
		public Action<string> Action { get; } = action;

		public void Invoke(string inputCommand)
        {
            Action(inputCommand);
        }
    }
}
