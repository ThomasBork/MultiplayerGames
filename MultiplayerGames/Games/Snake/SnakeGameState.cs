using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Snake
{
    public class SnakeGameState
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public List<SnakeGamePlayerClient> Players { get; set; }
    }
}
