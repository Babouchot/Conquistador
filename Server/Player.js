// Player.js
// ========

exports = module.exports = Player;

function Player (gameID, playerSocket, tableSocket) {
	this.gameID = gameID;
	this.playerSocket = playerSocket;
	this.tableSocket = tableSocket;
	this.territories = new Array();
	this.score = 0;
}


Player.prototype.getID = function () {
	return this.gameID;
};

Player.prototype.getTerritories = function () {
	return this.territories;
};


Player.prototype.addTerritory = function (territory) {
	this.territories.push(territory);
	territory.setOwner(this);
};

Player.prototype.removeTerritory = function (territory) {
	var index = this.territories.indexOf(territory);
	if (index > -1) {
		this.territories.splice(index, 1);
	}
};

Player.prototype.play = function () {
	this.playerSocket.emit ('play');
	this.tableSocket.emit ('play', this);
};

