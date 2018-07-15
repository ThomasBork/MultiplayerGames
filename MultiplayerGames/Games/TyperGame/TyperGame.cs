using MultiplayerGames.Games.TyperGame;
using MultiplayerGames.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.TyperGame
{
    public class TyperGame : Game
    {
        string GoalString { get; set; }

        public TyperGame() : base(TyperGame.GameTypeInfo.ID)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            this.GoalString = new string(Enumerable.Repeat(chars, 40)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private static GameTypeInfo _gameTypeInfo;

        public static GameTypeInfo GameTypeInfo
        {
            get {
                if(_gameTypeInfo == null)
                {
                    _gameTypeInfo = new GameTypeInfo
                    {
                        ID = 0,
                        Name = "Quick Type",
                        Description = "Type faster than your opponents to win!"
                    };
                }
                return _gameTypeInfo;
            }
        }

        public override Player AddPlayer(WebSocket socket, string id)
        {
            var player = new TyperGamePlayer(socket, id);
            this.Players.Add(player);
            player.Game = this;
            return player;
        }

        public override void HandleMessage(Player player, GameMessage message)
        {
            // Cast player object to a game-specific player type
            this.HandleMessage((TyperGamePlayer)player, message);
        }

        public void HandleMessage (TyperGamePlayer player, GameMessage message)
        {
            if(message.Type == GameMessageType.Action)
            {
                if (this.IsStarted)
                {
                    var goalChar = this.GoalString[player.Score];
                    var playerChar = ((string)message.Content)[0];
                    if (goalChar == playerChar)
                    {
                        player.Score++;
                        var messageContent = new { PlayerID = player.ID, Score = player.Score };
                        this.SendMessageToAll(new GameMessage { Type = "PLAYER_SUCCESS", Content = messageContent });
                        if(player.Score == this.GoalString.Length)
                        {
                            this.SendMessageToAll(new GameMessage { Type = "PLAYER_WON", Content = player.ID });
                            this.End();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Wrong char! Expected {goalChar}, got {playerChar}");
                    }
                }
            }
        }

        public override object GetClientState(Player player)
        {
            // Cast player object to a game-specific player type
            return this.GetClientState((TyperGamePlayer)player);
        }

        public TyperGameState GetClientState(TyperGamePlayer player)
        {
            var playerList = new List<TyperGamePlayerClient>();
            foreach(TyperGamePlayer p in this.Players)
            {
                playerList.Add(p.GetClientRepresentation());
            }
            var state = new TyperGameState
            {
                GoalString = this.GoalString,
                Players = playerList
            };
            return state;
        }
    }
}
