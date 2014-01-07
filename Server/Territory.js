// Territory.js
// ============

exports = module.exports = Territory;

function Territory () {
	this.owner;
	this.adjacents = new Array();
}


Territory.prototype.setOwner = function (player) {
	this.owner = player;
};

Territory.prototype.addAdjacent = function (territory) {
	this.adjacents.push (territory);
};