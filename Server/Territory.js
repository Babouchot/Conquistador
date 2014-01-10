// Territory.js
// ============

exports = module.exports = Territory;

function Territory (zone) {
	this.owner = undefined;
	this.zone = zone;
}


Territory.prototype.setOwner = function (player) {
	this.owner = player;
};
