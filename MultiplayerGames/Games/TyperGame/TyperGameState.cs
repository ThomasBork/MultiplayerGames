using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.TyperGame
{
    public class TyperGameState
    {
        public List<TyperGamePlayerClient> Players { get; set; }

        public string GoalString { get; set; }
    }
}
