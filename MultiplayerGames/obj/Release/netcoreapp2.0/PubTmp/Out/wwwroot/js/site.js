var connection;
var jqServerFeedback;

var pregame = {
    gameID: "-1",
    gameTypeID: "-1",
    hostID: -1,
    players: []
};

var game = null;

$(document).ready(function () {
    jqServerFeedback = $('#server-feedback');
    initDOM();
    initEvents();
    initConnection();
});

function initDOM() {
    changeToPage("game-selection");
}

function changeToPage(pageID) {
    $('.page').hide();
    $('#' + pageID).show();
}

function initEvents() {
    $(window).on("keydown", function (event) {
        if (game !== null) {
            game.onKeydown(event.which);
        }
    });

    $('.btn-game-type').on("click", function () {
        var jqThis = $(this);
        var gameTypeID = jqThis.attr('attr-id');
        joinOrCreateGame(gameTypeID);
    });

    $('.btn-start-game').on("click", function () {
        startGame();
    });
}

function initConnection() {
    jqServerFeedback.html("Connecting...");
    connection = new WebSocketManager.Connection("ws://" + window.location.host + "/server");

    connection.connectionMethods.onConnected = () => {
        jqServerFeedback.html("Connected.");
    };

    connection.connectionMethods.onDisconnected = () => {
        jqServerFeedback.html("Disconnected.");
    };
    
    connection.clientMethods["onGameJoined"] = (state) => {
        jqServerFeedback.html("Joined game with ID = " + state.gameID + ".");
        initPregame(state.gameID, state.gameTypeID, state.players, state.hostID);
    };

    connection.clientMethods["onPlayerJoined"] = (player, state) => {
        jqServerFeedback.html("Player with ID = " + player.id + " joined the game.");
        
        updatePregame(state.gameID, state.gameTypeID, state.players, state.hostID);
    };

    connection.clientMethods["onGameStarted"] = (state) => {
        jqServerFeedback.html("Game started.");

        loadGame(state);
    };

    connection.clientMethods["receiveGameMessage"] = (message) => {
        if (game !== null) {
            game.handleMessage(message);
        }
    };

    connection.start();

    window.onbeforeunload = function (event) {
        connection.invoke("Disconnect", connection.connectionId);
    };
}

function sendGameMessage(content) {
    if (game !== null) {
        connection.invoke("GameAction", connection.connectionId, game.id + "", content);
    }
}

function joinOrCreateGame(gameTypeID) {
    connection.invoke("JoinOrCreateGame", connection.connectionId, gameTypeID + "");
}

function initPregame(gameID, gameTypeID, players, hostID) {
    pregame.gameID = gameID;
    pregame.gameTypeID = gameTypeID;
    pregame.players = players;
    pregame.hostID = hostID;
    buildPregame(pregame);
    changeToPage("pregame");
}

function buildPregame(pregame) {
    var jqPlayerList = $('#pregame .player-list');
    jqPlayerList.empty();
    for (var i = 0; i < pregame.players.length; i++) {
        var player = pregame.players[i];
        var jqLine = $('<li class="player"></li>');
        if (player.id === pregame.hostID) {
            jqLine.addClass("host");
        }
        jqLine.attr("player-id", player.id);
        jqLine.html(player.name);
        jqPlayerList.append(jqLine);
    }
}

function updatePregame(gameID, gameTypeID, players, hostID) {
    initPregame(gameID, gameTypeID, players, hostID);
}

function startGame() {
    connection.invoke("StartGame", connection.connectionId, "" + pregame.gameID);
}

function loadGame(state) {
    var gameTypeID = pregame.gameTypeID;
    switch (gameTypeID) {
        case 0:
            game = newTyperGame(pregame, state);
            break;
        case 1:
            game = newSnakeGame(pregame, state);
            break;
        default:
            console.log("Unknown game type");
    }
    game.initialize($('#game'));
    changeToPage("game");
}

function endGame() {
    game = null;
}

function Game(pregame) {
    this.id = pregame.gameID;
    this.typeID = pregame.gameTypeID;
    this.players = [];
    this.you = null;
    this.jqParent = null;

    this.updateYou = function () {
        this.you = this.getPlayer(connection.connectionId);
        $('#header .name').html(this.you.name);
    };

    this.getPlayer = function (id) {
        for (var playerIndex in this.players) {
            if (this.players[playerIndex].id === id) {
                return this.players[playerIndex];
            }
        }
    }

    this.initialize = function (jqParent) { };
    this.updateState = function (jqParent, state) { };
    this.onKeydown = function (keycode) { };
    this.handleMessage = function (message) { };
}