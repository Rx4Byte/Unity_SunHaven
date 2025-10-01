using System;

namespace CommandExtension.Models
{
    public enum CommandState
    {
        None,
        Activated,
        Deactivated
    }

    public class Command(string prefixedKey, string description, string usage, CommandState state, Action<string> action)
	{
		public string PrefixedKey { get; } = prefixedKey;
		public string Usage { get; } = usage;
		public string Description { get; } = description;
		public CommandState State { get; set; } = state;
		public Action<string> Action { get; } = action;
    }
}
