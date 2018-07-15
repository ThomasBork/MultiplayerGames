using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using WebSocketManager;
using WebSocketManager.Common;

namespace MultiplayerGames.Models
{
    public class GameLobbyHandler : WebSocketHandler
    {
        public GameLobbyHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketID = WebSocketConnectionManager.GetId(socket);
        }

        public async Task JoinOrCreateGame(string playerID, string gameTypeID)
        {
            var socket = WebSocketConnectionManager.GetSocketById(playerID);
            GameLobby.Instance.JoinOrCreate(socket, int.Parse(gameTypeID), playerID);
        }

        public async Task JoinGame(string playerID, string gameID)
        {
            var socket = WebSocketConnectionManager.GetSocketById(playerID);
            var game = GameLobby.Instance.GetGame(int.Parse(gameID));
            if (game != null && !game.IsStarted)
            {
                GameLobby.Instance.JoinGame(socket, game, playerID);
            }
        }

        public async Task StartGame(string playerID, string gameID)
        {
            var game = GameLobby.Instance.GetGame(int.Parse(gameID));
            if (game != null && !game.IsStarted)
            {
                if (game.Host.ID == playerID)
                {
                    game.Start();
                }
            }
        }

        public async Task GameAction(string playerID, string gameID, string action)
        {
            var game = GameLobby.Instance.GetGame(int.Parse(gameID));
            if (game != null && game.IsStarted)
            {
                var player = game.GetPlayer(playerID);
                var message = new GameMessage { Type = GameMessageType.Action, Content = action };
                game.HandleMessage(player, message);
            }
        }

        public async Task Disconnect(string playerID)
        {
            var socket = WebSocketConnectionManager.GetSocketById(playerID);
            GameLobby.Instance.RemovePlayerFromGames(playerID);
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketID = WebSocketConnectionManager.GetId(socket);

            await base.OnDisconnected(socket);
        }
    }
}
