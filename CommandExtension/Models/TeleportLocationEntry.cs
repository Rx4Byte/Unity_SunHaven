using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFSW.QC.Utilities;
using UnityEngine;

namespace CommandExtension.Models
{
    public readonly struct TeleportLocationEntry
    {
        public string Key { get; }
        public Vector2 Position { get; }
        public string Destination { get; }

        public TeleportLocationEntry(string key, Vector2 position, string destination)
        {
            Key = key;
            Position = position;
            Destination = destination;
        }

        public bool Matches(string sceneName)
        {
            return sceneName.Equals(Key, StringComparison.OrdinalIgnoreCase);
        }
    }
}
