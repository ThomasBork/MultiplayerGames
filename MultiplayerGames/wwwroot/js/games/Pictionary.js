function newPictionaryGame(pregame, state) {
    var game = new Game(pregame);

    game.id = pregame.gameID;
    game.typeID = pregame.gameTypeID;
    game.players = state.players;
    game.updateYou();
    game.goalString = state.goalString;

    var context;
    game.pixelWidth = state.pixelWidth;
    game.pixelHeight = state.pixelHeight;

    var buildGame = function (jqParent) {
        game.jqParent = jqParent;
        jqParent.empty();

        var jqTitle = $('<div class="pictionary-game-title">');
        jqParent.append(jqTitle);
        jqTitle.html("Pictionary");

        var jqCanvas = $('<canvas class="pictionary-game-canvas">');
        jqParent.append(jqCanvas);
        var canvasWidth = game.pixelWidth ;
        var canvasHeight = game.pixelHeight ;
        jqCanvas.attr("width", canvasWidth);
        jqCanvas.attr("height", canvasHeight);
        context = jqCanvas[0].getContext("2d");
        context.fillStyle = "#2c2c34";
        context.fillRect(0, 0, canvasWidth, canvasHeight);

        var jqNewGameButton = $('<div class="btn btn-return-to-game-selection">Return to game selection</div>');
        jqParent.append(jqNewGameButton);
        jqNewGameButton.on("click", function () {
            endGame();
            changeToPage("game-selection");
        });
        jqNewGameButton.hide();
    };
    
    var updatePlayerPositions = function (players) {
        for (var index in players) {
            game.players[index].position = players[index].position;
        }
    };

    game.initialize = function (jqParent) {
        buildGame(jqParent);
    };

    game.updateState = function (jqParent, state) {
        this.players = state.players;
        buildGame(jqParent);
    };

    game.handleMessage = function (message) {
        switch (message.type) {
            case "MOVEMENT":
                var players = message.content;
                updatePlayerPositions(players);
                drawPlayerPositions();
                break;
            case "PLAYER_WON":
                var playerID = message.content;
                var player = this.getPlayer(playerID);
                jqServerFeedback.html(player.name + " has won the game!");
                $('.btn-return-to-game-selection').show();
                break;
            default:
                console.log("Unknown message type: " + message.type);
        }
    }

    return game;
}