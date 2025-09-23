using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wish;

namespace CommandExtension
{
    public static class Utility
    {
        // get the player for singleplayer/multiplayer
        public static Player GetPlayerForCommand()
        {
            //Player[] players = UnityEngine.Object.FindObjectsOfType<Player>();
            //foreach (Player player in players)
            //{
            //    if (players.Length == 1 || player.GetComponent<NetworkGamePlayer>().playerName == playerNameForCommands)
            //        return player;
            //}
            return Player.Instance;
        }
    }
}
