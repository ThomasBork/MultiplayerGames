using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Models
{
    public abstract class Game
    {
        public static int NextID = 0;

        public List<Player> Players { get; set; }

        public Player Host { get; set; }

        public int ID { get; set; }

        public int GameTypeID { get; set; }

        public bool IsStarted { get; set; }

        protected long lastTick = System.Environment.TickCount;

        public Random Random { get; set; }

        public Game (int gameTypeID)
        {
            this.Random = new Random();
            this.GameTypeID = gameTypeID;
            this.Players = new List<Player>();
            this.ID = Game.NextID;
            Game.NextID++;
            this.IsStarted = false;
        }

        public abstract Player AddPlayer(WebSocket socket, string id);

        public void Start()
        {
            this.OnStarted();
            foreach(var player in this.Players)
            {
                var clientState = this.GetClientState(player);
                InvokeClientMethod(player, "onGameStarted", clientState);
            }
            this.IsStarted = true;
            lastTick = System.Environment.TickCount;
        }

        protected virtual void OnStarted() { }

        public abstract void HandleMessage(Player player, GameMessage message);

        public abstract object GetClientState(Player player);
        
        public ClientPregameGame GetClientPregameState()
        {
            var pregame = new ClientPregameGame();
            pregame.GameID = this.ID;
            pregame.GameTypeID = this.GameTypeID;
            pregame.HostID = this.Host.ID;
            pregame.Players = this.Players.Select(p => p.GetClientPregamePlayer()).ToList();
            return pregame;
        }

        public void Update ()
        {
            var currentTick = System.Environment.TickCount;

            var dTime = currentTick - this.lastTick;

            this.Update(dTime);

            this.lastTick = currentTick;
        }

        public virtual void Update (float dTime){ }

        public void InvokeClientMethod(Player player, string methodName, object[] arguments)
        {
            ((GameLobbyHandler)Startup.ServiceProvider.GetService(typeof(GameLobbyHandler)))
                .InvokeClientMethodAsync(player.ID, methodName, arguments)
                .Wait();
        }
        public void InvokeClientMethod(Player player, string methodName, object argument)
        {
            InvokeClientMethod(player, methodName, new object[] { argument });
        }

        public void InvokeClientMethodToAll(string methodName, object[] arguments)
        {
            foreach(var player in this.Players)
            {
                this.InvokeClientMethod(player, methodName, arguments);
            }
        }

        public void InvokeClientMethodToAll(string methodName, object argument)
        {
            InvokeClientMethodToAll(methodName, new object[] { argument });
        }

        public void InvokeClientMethodToAllExcept(Player player, string methodName, object[] arguments)
        {
            foreach (var p in this.Players)
            {
                if(p != player)
                {
                    this.InvokeClientMethod(p, methodName, arguments);
                }
            }
        }

        public void InvokeClientMethodToAllExcept(Player player, string methodName, object argument)
        {
            InvokeClientMethodToAllExcept(player, methodName, new object[] { argument });
        }

        public void SendMessage(Player player, GameMessage message)
        {
            this.InvokeClientMethod(player, "receiveGameMessage", new object[] { message });
        }

        public void SendMessageToAll(GameMessage message)
        {
            foreach(var player in this.Players)
            {
                this.SendMessage(player, message);
            }
        }

        public Player GetPlayer(string playerID)
        {
            return Players.FirstOrDefault(p => p.ID == playerID);
        }

        public void DisconnectPlayer(Player player)
        {
            this.Players.Remove(player);
            //TODO: Notify clients
        }

        public void End()
        {
            GameLobby.Instance.Games.Remove(this);
        }
    }
}
