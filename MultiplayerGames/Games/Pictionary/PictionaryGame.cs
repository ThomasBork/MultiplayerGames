using MultiplayerGames.Models;
using MultiplayerGames.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MultiplayerGames.Games.Pictionary
{
    public class PictionaryGame : Game
    {
        protected int PixelsWidth { get; set; }
        protected int PixelsHeight { get; set; }
        protected string CurrentWord { get; set; }
        protected PictionaryPlayer CurrentPlayer { get; set; }

        protected string[] WordArray = { "Frodo", "Knuckles", "Batman" }; 

        public PictionaryGame() : base(PictionaryGame.GameTypeInfo.ID)
        {
            this.PixelsWidth = 800;
            this.PixelsHeight = 600;
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
                        ID = 2,
                        Name = "Pictionary",
                        Description = "Draw! Guess!"
                    };
                }
                return _gameTypeInfo;
            }
        }

        protected override void OnStarted()
        {
            this.StartNewRound();
        }

        protected void StartNewRound()
        {
            var currentPlayerIndex = this.Random.Next(this.Players.Count);
            this.CurrentPlayer = (PictionaryPlayer)this.Players[currentPlayerIndex];
            var currentWordIndex = this.Random.Next(this.WordArray.Length);
            this.CurrentWord = this.WordArray[currentWordIndex];
            
            var newRoundMessage = new GameMessage("NewRound", this.CurrentPlayer);
            this.SendMessageToAll(newRoundMessage);
            var newWordMessage = new GameMessage("NewWord", this.CurrentWord);
            this.SendMessage(this.CurrentPlayer, newWordMessage);
        }

        public override Player AddPlayer(WebSocket socket, string id)
        {
            var player = new PictionaryPlayer(socket, id);
            this.Players.Add(player);
            player.Game = this;
            return player;
        }

        public override object GetClientState(Player player)
        {
            return this.GetClientState((PictionaryPlayer)player);
        }

        public PictionaryGameState GetClientState(PictionaryPlayer player)
        {
            var playerList = new List<PictionaryPlayerClient>();
            foreach (PictionaryPlayer p in this.Players)
            {
                playerList.Add(p.GetClientRepresentation());
            }
            var state = new PictionaryGameState
            {
                PixelsHeight = this.PixelsHeight,
                PixelsWidth = this.PixelsWidth,
                CurrentPlayerID = this.CurrentPlayer.ID,
                Players = playerList
            };
            return state;
        }

        public override void HandleMessage(Player player, GameMessage message)
        {
            this.HandleMessage((PictionaryPlayer)player, message);
        }

        public void HandleMessage(PictionaryPlayer player, GameMessage message)
        {
            switch (message.Type)
            {
                case "DrawStroke":
                    var pixels = (PictionaryPixel[])message.Content;
                    DrawStroke(player, pixels);
                    break;
                case "Guess":
                    var guess = (string)message.Content;
                    if(guess==this.CurrentWord)
                    {
                        var winnerMessage = new GameMessage("Winner", new { PlayerID = player.ID, Word = this.CurrentWord });
                        this.SendMessageToAll(winnerMessage);
                    }
                    break;
            }
        }

        public void DrawStroke (PictionaryPlayer player, PictionaryPixel[] pixels)
        {
            SendPixelsToClients(pixels);
        }

        public void SendPixelsToClients(PictionaryPixel[] pixels)
        {
            this.SendMessageToAll(new GameMessage("MOVEMENT", pixels));
        }
    }
}
