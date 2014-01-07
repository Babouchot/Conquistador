// On va utiliser le module express qui permet de faire un serveur 
// web plus complet, capable de servir une page par defaut,
// des pages qui contiennent des css, qui incluent des fichiers 
// javascript etc
var express = require('express');
var Player = require('./Player.js');
var Game = require('./Game.js');
  

var PLAYER_NUMBER = 2;


// Création du serveur, page par defait, port
var app = express();
app.configure(function(){
  app.use(express.static(__dirname + '/'));
});

app.get('/', function(req, res, next){
  res.render('index.html');
});

var server = app.listen(8080);

console.log('Server running ou port 8080');

var serverSocket = require('socket.io');
var io = serverSocket.listen(server,  {log : false});


var players = new Array();
var table;


io.sockets.on('connection', connectionToServer);

// A device try to connect to the server
function connectionToServer(socket, type, gameID) {

	socket.emit ('requestIdentity');

	socket.on('identity', function (type, gameID) {

		switch (type) {
		case 'table':
			table = socket;
			console.log("table connected");
			if (players.length > PLAYER_NUMBER) {
				launchGame();
			}
			break;
		case 'player':
			if (players.length < PLAYER_NUMBER) {
				var player = new Player(gameID, socket, table);
				players.push(player);
				console.log("player " + players.length + " connected");
				if (table !== undefined) {
					table.emit('new_player', player);
				}
				if (players.length >= PLAYER_NUMBER) {
					launchGame();
				}
			}
			else if (table !== undefined) {
				launchGame();
			}
			break;
		}

	});
}



function launchGame () {
	console.log("game is launching");
	for (var i = 0; i < players.length; ++i) {
		console.log("Player " + players[i].gameID + i + " start_game");
		players[i].playerSocket.emit('start_game');
	}
	if (table !== undefined) {
		table.emit('start_game');
	}
	var game = new Game(players, table);
	console.log("Game launched");
	game.gameLoop();
}