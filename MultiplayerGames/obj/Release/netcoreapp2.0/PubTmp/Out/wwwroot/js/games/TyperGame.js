function newTyperGame(pregame, state) {
    var game = new Game(pregame);

    game.id = pregame.gameID;
    game.typeID = pregame.gameTypeID;
    game.players = state.players;
    game.updateYou();
    game.goalString = state.goalString;

    var updatePlayerString = function (jqString, player) {
        var jqTextSpan1 = jqString.find('.done');
        var jqTextSpan2 = jqString.find('.next');
        var jqTextSpan3 = jqString.find('.rest');
        var text1 = game.goalString.substring(0, player.score);
        var text2 = game.goalString.substring(player.score, player.score + 1);
        var text3 = game.goalString.substring(player.score + 1, game.goalString.length);
        jqTextSpan1.html(text1);
        jqTextSpan2.html(text2);
        jqTextSpan3.html(text3);
    };

    var buildGame = function (jqParent) {
        game.jqParent = jqParent;
        jqParent.empty();

        var jqTitle = $('<div class="typer-game-title">');
        jqParent.append(jqTitle);
        jqTitle.html("Quick Type");

        var jqPlayers = $('<div class="typer-game-players">');
        jqParent.append(jqPlayers);
        var jqNames = $('<div class="names">');
        jqPlayers.append(jqNames);
        var jqStrings = $('<div class="strings">');
        jqPlayers.append(jqStrings);

        for (var i = 0; i < game.players.length; i++) {
            var player = game.players[i];
            
            var jqName = $('<div class="name">');
            jqNames.append(jqName);
            var jqNameText = $('<span class="text">');
            jqName.append(jqNameText);
            jqName.attr("attr-id", player.id);
            jqNameText.html(player.name);

            var jqString = $('<div class="string">');
            jqStrings.append(jqString);
            jqString.attr("attr-id", player.id);

            var jqTextSpan1 = $('<span class="done">');
            var jqTextSpan2 = $('<span class="next">');
            var jqTextSpan3 = $('<span class="rest">');
            jqString.append(jqTextSpan1);
            jqString.append(jqTextSpan2);
            jqString.append(jqTextSpan3);
            
            if (connection.connectionId === player.id) {
                jqName.addClass("self");
                jqString.addClass("self");
            }

            updatePlayerString(jqString, player);
        }

        var jqNewGameButton = $('<div class="btn btn-return-to-game-selection">Return to game selection</div>');
        jqParent.append(jqNewGameButton);
        jqNewGameButton.on("click", function () {
            endGame();
            changeToPage("game-selection");
        });
        jqNewGameButton.hide();
    };

    game.initialize = function (jqParent) {
        buildGame(jqParent);
    };

    game.updateState = function (jqParent, state) {
        this.players = state.players;
        buildGame(jqParent);
    };

    game.onKeydown = function (keycode) {
        var targetChar = this.goalString[this.you.score];
        if (targetChar.charCodeAt(0) === keycode) {
            sendGameMessage(String.fromCharCode(keycode));
        }
    };

    game.handleMessage = function (message) {
        switch (message.type) {
            case "PLAYER_SUCCESS":
                var playerID = message.content.playerID;
                var score = message.content.score;
                var player = this.getPlayer(playerID);
                player.score = score;
                var jqPlayerString = this.jqParent.find('.typer-game-players .string[attr-id="' + player.id + '"]');
                updatePlayerString(jqPlayerString, player);
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