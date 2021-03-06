// Player.js
// ========

exports = module.exports = Player;

function Player (gameID, playerSocket, tableSocket, pseudo) {
	this.gameID = gameID;
	this.pseudo = pseudo;
	this.playerSocket = playerSocket;
	this.tableSocket = tableSocket;
	this.territories = new Array();
	this.score = 0;

	// Temporary
	var questionsFile = require('./questions.js');
	var questions = new questionsFile();
	console.log("player initialized with socket : " + playerSocket.id);
	this.playerSocket.on ('requestQuestionTest',function() {
		playerSocket.emit('question', questions[2]);
	});

}


Player.prototype.getID = function () {
	return this.gameID;
};

Player.prototype.getTerritories = function () {
	var territoriesID = new Array();
	for (var i = 0; i < this.territories.length; ++i) {
		territoriesID.push(this.territories[i].zone);
	}
	return territoriesID;
};


Player.prototype.addTerritory = function (territory) {
	this.territories.push(territory);
};

Player.prototype.removeTerritory = function (territory) {
	var index = this.territories.indexOf(territory);
	if (index > -1) {
		this.territories.splice(index, 1);
	}
};

Player.prototype.serialize = function () {
	// TODO put the score instead of territories.length in 'score' field
	var territories = this.getTerritories();
	return {'gameID' : this.gameID, 'pseudo': this.pseudo, 'score': territories.length, 'territories' : territories};
};
