using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Models
{
    public abstract class Player
    {
        public static int NextID = 0;

        public WebSocket WebSocket { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public Game Game { get; set; }

        public Player(WebSocket socket, string id)
        {
            this.WebSocket = socket;
            this.ID = id;
            this.Name = "Guest" + Player.NextID;
            Player.NextID++;
        }

        public ClientPregamePlayer GetClientPregamePlayer ()
        {
            return new ClientPregamePlayer
            {
                ID = this.ID,
                Name = this.Name
            };
        }
    }
}
