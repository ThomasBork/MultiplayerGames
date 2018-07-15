using MultiplayerGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.TyperGame
{
    public class TyperGamePlayer : Player
    {
        public TyperGamePlayer(WebSocket socket, string id) : base(socket, id)
        {
            
        }

        public int Score { get; set; }

        public TyperGamePlayerClient GetClientRepresentation()
        {
            return new TyperGamePlayerClient { ID = this.ID, Name = this.Name, Score = this.Score };
        }
    }

    public class TyperGamePlayerClient
    {
        public int Score { get; set; }
        public string Name { get; set; }
        public string ID { get; set; }
    }
}
