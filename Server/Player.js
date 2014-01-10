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

	this.playerSocket.on ('requestQuestionTest',function() {
		this.playerSocket.emit('question', questions[2]);
	});

}


Player.prototype.getID = function () {
	return this.gameID;
};

Player.prototype.getTerritories = function () {
	return this.territories;
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

Player.prototype.play = function () {
	this.playerSocket.emit ('play', {});
//	this.tableSocket.emit ('play');
};


Player.prototype.serialize = function () {
	return {'pseudo': this.pseudo, 'score': this.score, 'territories':this.territories};
};
