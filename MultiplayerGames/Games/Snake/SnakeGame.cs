using MultiplayerGames.Models;
using MultiplayerGames.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Snake
{
    public class SnakeGame : Game
    {
        protected float MoveFrequency { get; set; }
        protected float TimeUntilNextMove { get; set; }

        protected int MapWidth { get; set; }
        protected int MapHeight { get; set; }

        public SnakeGame() : base(SnakeGame.GameTypeInfo.ID)
        {
            this.MoveFrequency = 200;
            this.TimeUntilNextMove = TimeUntilNextMove;
            this.MapHeight = 30;
            this.MapWidth = 30;
        }

        private static GameTypeInfo _gameTypeInfo;

        public static GameTypeInfo GameTypeInfo
        {
            get
            {
                if (_gameTypeInfo == null)
                {
                    _gameTypeInfo = new GameTypeInfo
                    {
                        ID = 1,
                        Name = "Snake",
                        Description = "Live as long as possible!"
                    };
                }
                return _gameTypeInfo;
            }
        }

        public override Player AddPlayer(WebSocket socket, string id)
        {
            var random = new Random();
            var position = new IntVector2(random.Next(MapWidth), random.Next(MapHeight));
            var player = new SnakePlayer(socket, id, position);
            this.Players.Add(player);
            player.Game = this;
            return player;
        }

        public override object GetClientState(Player player)
        {
            return this.GetClientState((SnakePlayer)player);
        }

        public SnakeGameState GetClientState(SnakePlayer player)
        {
            var playerList = new List<SnakeGamePlayerClient>();
            foreach (SnakePlayer p in this.Players)
            {
                playerList.Add(p.GetClientRepresentation());
            }
            var state = new SnakeGameState
            {
                MapHeight = this.MapHeight,
                MapWidth = this.MapWidth,
                Players = playerList
            };
            return state;
        }

        public override void HandleMessage(Player player, GameMessage message)
        {
            this.HandleMessage((SnakePlayer)player, message);
        }

        public void HandleMessage(SnakePlayer player, GameMessage message)
        {
            switch (message.Type)
            {
                case GameMessageType.Action:
                    var direction = (string)message.Content;
                    player.SetDirection(direction);
                    break;
            }
        }

        public override void Update(float dTime)
        {
            this.TimeUntilNextMove -= dTime;
            if(this.TimeUntilNextMove < 0)
            {
                this.TimeUntilNextMove += this.MoveFrequency;
                this.MoveSnakes();
                this.KillCollidingPlayers();
                this.SendMovementToClients();
            }
        }

        public void MoveSnakes ()
        {
            foreach(SnakePlayer player in this.Players)
            {
                if (player.IsAlive)
                {
                    player.Move();
                    if (player.Position.X < 0)
                    {
                        player.Position.X = this.MapWidth - 1;
                    }
                    else if (player.Position.Y < 0)
                    {
                        player.Position.Y = this.MapHeight - 1;
                    }
                    else if (player.Position.X >= MapWidth)
                    {
                        player.Position.X = 0;
                    }
                    else if (player.Position.Y >= MapHeight)
                    {
                        player.Position.Y = 0;
                    }
                }
            }
        }

        public void KillCollidingPlayers()
        {
            foreach (SnakePlayer player in this.Players)
            {
                if (player.IsAlive)
                {
                    // Check collision
                    foreach (SnakePlayer innerPlayer in this.Players)
                    {
                        // Check current positions
                        if(player != innerPlayer)
                        {
                            if (player.Position.Equals(innerPlayer.Position))
                            {
                                player.IsAlive = false;
                                innerPlayer.IsAlive = false;
                                break;
                            }
                        }

                        // Check trails
                        foreach (var position in innerPlayer.Trail)
                        {
                            if (player.Position.Equals(position))
                            {
                                player.IsAlive = false;
                                break;
                            }
                        }

                        // Stop checking if player is dead
                        if(player.IsAlive == false)
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void SendMovementToClients()
        {
            var players = this.Players.Select(p => ((SnakePlayer)p).GetClientRepresentation()).ToArray();
            this.SendMessageToAll(new GameMessage("MOVEMENT", players));
        }
    }
}
