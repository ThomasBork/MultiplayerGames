using MultiplayerGames.Models;
using MultiplayerGames.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Snake
{
    public class SnakePlayer : Player
    {
        public bool IsAlive { get; set; }

        public IntVector2 Position { get; set; }

        public List<IntVector2> Trail { get; set; }

        public IntVector2 Velocity { get; set; }

        public SnakePlayer(WebSocket socket, string id, IntVector2 position) : base(socket, id)
        {
            this.Trail = new List<IntVector2>();
            this.Position = position;
            this.SetDirection("RIGHT");
            this.IsAlive = true;
        }

        public void SetDirection(string direction)
        {
            switch(direction)
            {
                case "UP":
                    Velocity = new IntVector2(0, -1);
                    break;
                case "DOWN":
                    Velocity = new IntVector2(0, 1);
                    break;
                case "LEFT":
                    Velocity = new IntVector2(-1, 0);
                    break;
                case "RIGHT":
                    Velocity = new IntVector2(1, 0);
                    break;
            }
        }

        public void Move()
        {
            this.Trail.Add(this.Position);
            this.Position = this.Position + this.Velocity;
        }

        public SnakeGamePlayerClient GetClientRepresentation()
        {
            return new SnakeGamePlayerClient { ID = this.ID, Name = this.Name, IsAlive = this.IsAlive, Position = this.Position };
        }
    }

    public class SnakeGamePlayerClient
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsAlive { get; set; }
        public IntVector2 Position { get; set; }
    }
}
