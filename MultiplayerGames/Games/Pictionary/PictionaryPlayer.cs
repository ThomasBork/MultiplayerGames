using MultiplayerGames.Models;
using MultiplayerGames.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Pictionary
{
    public class PictionaryPlayer : Player
    {
        public int Points { get; set; }
        

        public PictionaryPlayer(WebSocket socket, string id) : base(socket, id)
        {
            this.Points = 0;
        }

        public PictionaryPlayerClient GetClientRepresentation()
        {
            return new PictionaryPlayerClient { ID = this.ID, Name = this.Name, Points = this.Points };
        }
    }

    public class PictionaryPlayerClient
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
    }
}
