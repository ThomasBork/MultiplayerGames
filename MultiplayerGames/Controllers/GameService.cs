using MultiplayerGames.Games.Snake;
using MultiplayerGames.Games.TyperGame;
using MultiplayerGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Controllers
{
    public static class GameService
    {
        public static List<GameTypeInfo> GetAllGameTypeInfo ()
        {
            var list = new List<GameTypeInfo>();

            list.Add(TyperGame.GameTypeInfo);
            list.Add(PictionaryGame.GameTypeInfo);

            return list;
        }
    }
}
