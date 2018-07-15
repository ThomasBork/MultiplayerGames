using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Pictionary
{
    public class PictionaryGameState
    {
        public int PixelsWidth { get; set; }
        public int PixelsHeight { get; set; }
        public string CurrentPlayerID { get; set; }
        public List<PictionaryPlayerClient> Players { get; set; }
    }
}
