using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MultiplayerGames.Games.Snake;
using MultiplayerGames.Games.TyperGame;
using WebSocketManager;

namespace MultiplayerGames.Models
{
    public class GameLobby
    {
        private static GameLobby instance;
        private static readonly object padLock = new object();
        public List<Game> Games { get; set; }

        public Timer Timer;

        public static GameLobby Instance
        {
            get
            {
                lock (padLock)
                {
                    return instance ?? (instance = new GameLobby());
                }
            }
        }

        public void Initialize ()
        {
            Games = new List<Game>();
            Timer = new Timer(UpdateGames, null, 0, 1000 / 20);
        }

        private void UpdateGames (object state)
        {
            System.Diagnostics.Debug.WriteLine("Timer run");
            foreach (var game in this.Games)
            {
                if (game.IsStarted)
                {
                    game.Update();
                }
            }
            System.Diagnostics.Debug.WriteLine("Timer done");
        }

        public void JoinGame(WebSocket socket, Game game, string playerID)
        {
            var player = game.AddPlayer(socket, playerID);
            if (game.Players.Count == 1)
            {
                game.Host = player;
            }
            var gameState = game.GetClientPregameState();
            game.InvokeClientMethod(player, "onGameJoined", gameState);
            game.InvokeClientMethodToAllExcept(player, "onPlayerJoined", new object[] {player.GetClientPregamePlayer(), gameState });
        }
        
        public Game AddGame(int gameTypeID)
        {
            var game = NewGame(gameTypeID);
            this.Games.Add(game);
            return game;
        }

        public Game NewGame (int gameTypeID)
        {
            switch (gameTypeID)
            {
                case 0: return new TyperGame();
                case 1: return new PictionaryGame();
                default: throw new Exception("Unknown game type");
            }
        }

        public void JoinOrCreate(WebSocket socket, int gameTypeID, string playerID)
        {
            var gameType = this.GetGameType(gameTypeID);
            var game = this.Games.FirstOrDefault(g => !g.IsStarted && g.GetType() == gameType);
            if(game == null)
            {
                game = this.AddGame(gameTypeID);
            }
            this.JoinGame(socket, game, playerID);
        }

        public Type GetGameType(int gameTypeID)
        {
            switch(gameTypeID)
            {
                case 0: return typeof(TyperGame);
                case 1: return typeof(PictionaryGame);
                default: return null;
            }
        }

        public int GetPlayerCount ()
        {
            return this.Games.Where(g => g.IsStarted).Sum(g => g.Players.Count);
        }

        public Game GetGame(int gameID)
        {
            return Games.FirstOrDefault(g => g.ID == gameID);
        }

        public void RemovePlayerFromGames (string playerID)
        {
            var emptyGames = new List<Game>();
            foreach(var game in this.Games)
            {
                Player playerToRemove = null;
                foreach(var player in game.Players)
                {
                    if(player.ID == playerID)
                    {
                        playerToRemove = player;
                    }
                }
                if (playerToRemove != null)
                {
                    game.DisconnectPlayer(playerToRemove);
                    if (game.Players.Count == 0)
                    {
                        emptyGames.Add(game);
                    }
                }
            }
            foreach(var game in emptyGames)
            {
                this.Games.Remove(game);
            }
        }
    }
}
