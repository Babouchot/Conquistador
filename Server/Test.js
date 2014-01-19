// On va utiliser le module express qui permet de faire un serveur 
// web plus complet, capable de servir une page par defaut,
// des pages qui contiennent des css, qui incluent des fichiers 
// javascript etc
var express = require('express');
var Player = require('./Player.js');
var Game = require('./Game.js');
  

var PLAYER_NUMBER = 4;


// Création du serveur, page par defaut, port
var app = express();
app.configure(function(){
  app.use(express.static(__dirname + '/'));
});

app.get('/', function(req, res, next){
  res.render('index.html');
});

var server = app.listen(8080);

console.log('Conquistador server running ou port 8080');

var serverSocket = require('socket.io');
var io = serverSocket.listen(server,  {log : false});


var players = new Array();


io.sockets.on('connection', connectionToServer);

// A device try to connect to the server
function connectionToServer(socket) {

	console.log('connection request');
	players.push(socket);
	
	if(players.length > 1)
	{
		for(var i = 0; i < players.length; ++i)
		{
			players[i].emit('test', { 'id' : i});
		}
	}

}


io.sockets.on ('disconnect', function () {
	console.log("disconnecting");
});


function launchGame () {
	console.log("game is launching");
	if (table !== undefined) {
		table.emit('startGame', {});
	}
	for (var i = 0; i < players.length; ++i) {
		console.log("Player " + players[i].pseudo + players[i].gameID + " startGame");
		players[i].playerSocket.emit('startGame', {'id' : i});
	}
	var game = new Game(players, table);
	console.log("Game launched");

	table.on("startGame", function () {
		game.start();
	});
}