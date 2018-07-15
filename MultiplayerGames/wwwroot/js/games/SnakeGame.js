function newSnakeGame(pregame, state) {
    var game = new Game(pregame);

    game.id = pregame.gameID;
    game.typeID = pregame.gameTypeID;
    game.players = state.players;
    game.updateYou();
    game.goalString = state.goalString;

    var context;
    var tileWidth = 16;
    var tileHeight = tileWidth;

    var buildGame = function (jqParent) {
        game.jqParent = jqParent;
        jqParent.empty();

        var jqTitle = $('<div class="snake-game-title">');
        jqParent.append(jqTitle);
        jqTitle.html("Snake");

        var jqCanvas = $('<canvas class="snake-game-canvas">');
        jqParent.append(jqCanvas);
        var canvasWidth = tileWidth * state.mapWidth;
        var canvasHeight = tileHeight * state.mapHeight;
        jqCanvas.attr("width", canvasWidth);
        jqCanvas.attr("height", canvasHeight);
        context = jqCanvas[0].getContext("2d");
        context.fillStyle = "#2c2c34";
        context.fillRect(0, 0, canvasWidth, canvasHeight);

        drawPlayerPositions();

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

    var drawPlayerPositions = function () {
        for (var index in game.players) {
            var player = game.players[index];
            var color = "blue";

            if (connection.connectionId === player.id) {
                color = "red";
            }

            context.fillStyle = color;
            context.fillRect(player.position.x * tileWidth, player.position.y * tileHeight, tileWidth, tileHeight);
        }
    };

    game.initialize = function (jqParent) {
        buildGame(jqParent);
    };

    game.updateState = function (jqParent, state) {
        this.players = state.players;
        buildGame(jqParent);
    };

    game.onKeydown = function (keycode) {
        switch (keycode) {
            case 65:
                sendGameMessage("LEFT");
                break;
            case 87:
                sendGameMessage("UP");
                break;
            case 68:
                sendGameMessage("RIGHT");
                break;
            case 83:
                sendGameMessage("DOWN");
                break;

            case 37:
                sendGameMessage("LEFT");
                break;
            case 38:
                sendGameMessage("UP");
                break;
            case 39:
                sendGameMessage("RIGHT");
                break;
            case 40:
                sendGameMessage("DOWN");
                break;
        }
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