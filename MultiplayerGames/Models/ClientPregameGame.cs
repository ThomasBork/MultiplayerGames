using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Models
{
    public class ClientPregameGame
    {
        public int GameID { get; set; }
        public int GameTypeID { get; set; }
        public List<ClientPregamePlayer> Players { get; set; }
        public string HostID { get; set; }

        public ClientPregameGame()
        {
            this.Players = new List<ClientPregamePlayer>();
        }
    }
}
