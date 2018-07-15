using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Models
{
    public class GameMessage
    {
        public GameMessage()
        {

        }

        public GameMessage(string type, object content)
        {
            this.Type = type;
            this.Content = content;
        }

        public string Type { get; set; }

        public object Content { get; set; }
    }
    public static class GameMessageType
    {
        public const string Action = "ACTION";
        public const string State = "STATE";
    }
}
