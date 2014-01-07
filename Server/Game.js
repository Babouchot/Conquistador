// Game.js
// ========

exports = module.exports = Game;

function Game(playersArray, tableSocket) {
	this.players = playersArray;
	this.table = tableSocket;
	this.currentPhase = 0;
	this.phases = new Array();
	this.MAXTURNS = 12;
	this.currentTurn = 0;

	this.phases.push(this.phase1, this.phase2, this.phase3, this.phase4);

}


// Phase 1
Game.prototype.phase1 = function () {
	console.log('phase 1 started');
	// Pick a question from the database
	var question;
	this.table.emit('question', question);
	
	for (var i = 0; i < this.players.length; ++i) {
		this.players[i].playerSocket.emit('question', question);
	}
	
};

// Phase 2
Game.prototype.phase2 = function () {
	console.log('phase 2 started');
};

// Phase 3
Game.prototype.phase3 =  function () {
	console.log('phase 3 started');
	if (this.currentTurn > this.MAXTURNS) {
		this.currentPhase++;
	}
	for (var i = 0; i < this.players.length; ++i) {
		this.players[i].play();
	}
	this.currentTurn++;
};


//Phase 4
Game.prototype.phase4 = function () {
	console.log('phase 4 started');
	this.players.sort(function (player1, player2) {
		return player1.score - player2.score;
	});
	this.table.emit('results', players);
	var position = 0;
	for (var i = 0; i < this.players.length; ++i) {
		this.players[i].playerSocket.emit('results', position++);
	}
};



Game.prototype.gameLoop = function () {
	this.call(this.phases[this.currentPhase]());
};