//Game.js
//========

exports = module.exports = Game;

var TERRITORIES = 16;

function Game(playersArray, tableSocket) {
	this.players = playersArray;
	this.table = tableSocket;
	this.MAXTURNS = 12;
	this.currentTurn = 0;

	this.territories = new Array();

	//	Phase 1 : Attribution des territoires sur question
	this.phase1 = function () {	
		var playersAnswers = [];
		console.log('phase 1 started');
	};
	
	this.phase1.init = function () {
		// give one territory to each player
	};
	
	this.phase1.update = function () {
		// Pick a question from the database
		var question;
		//this.table.emit('question', question);
		for (var i = 0; i < this.players.length; ++i) {
			var p = this.players[i];
			p.playerSocket.emit('question', question);
			p.playerSocket.on('answer', function (answer) {
				playersAnswers.push({'id' : p.gameID, 'answer' : answer});
			});
		}
		
		// wait for answers
		function waitAnswers() {
			if (playersAnswers.length < this.players.length) {
				setTimeout(waitAnswers, 200);
			}
		}
		
		// find winner and attribute territories
		playersAnswer.sort(function (answer1, answer2) {
			var answerOffset1 = Math.abs(answer1.value - question.correctAnswer);
			var answerOffset2 = Math.abs(answer2.value - question.correctAnswer);
			
			if (answerOffset1 == answerOffset2) {
				return answer1.time - answer2.time;
			}
			return answerOffset1 - answerOffset2;
		});
		
		var capturedTerritories = new Array(4);
		for (var i = 0; i < playersAnswer.length; ++i) {
			switch (i) {
			case 0:
				capturedTerritories[playersAnswers[i].id] = 2;
				break;
			case 1:
				capturedTerritories[playersAnswers[i].id] = 1;
				break;
			case 2:
				capturedTerritories[playersAnswers[i].id] = 1;
				break;
			default:
				capturedTerritories[playersAnswers[i].id] = 0;
				break;
			}
		}
		// send the number of territories each player have to capture
		this.table.emit('capturePhase', capturedTerritories);
		
		// create the new assigned territories and assign the correct owner
		this.table.on ('endCapturePhase', function (capturedTerritories) {
			for (var i = 0; i < capturedTerritories.length; ++i) {
				var territory = new Territory(capturedTerritories[i].zone);
				this.territories.push(territory);
				changeTerritoryOwner(players[capturedTerritories[i].owner], territory);
			}
		});
		
		// send the updated list of territories to the table
		this.table.emit ('updateMapState', serializeTerritories());
		
		// check if all territories are assigned
		if (this.territories.length >= TERRITORIES) {
			return false;
		}
		return true;
	};



	//	Phase 2 : Placement des pions (tags) sur les zones controllées
	this.phase2 = function () {
		console.log('phase 2 started');
	};

	//	Phase 3
	this.phase3 =  function () {
		console.log('phase 3 started');
		if (this.currentTurn > this.MAXTURNS) {
			this.currentPhase++;
		}
		for (var i = 0; i < this.players.length; ++i) {
			this.players[i].play();
		}
		this.currentTurn++;
	};

	//	Phase 4
	this.phase4 = function () {
		console.log('phase 4 started');
		this.players.sort(function (player1, player2) {
			return player1.score - player2.score;
		});
		//WTF ?
		this.table.emit('results', players);
		var position = 0;
		for (var i = 0; i < this.players.length; ++i) {
			this.players[i].playerSocket.emit('results', position++);
		}
	};

	this.currentPhase = this.phase1;

	this.gameLoop = function () {
		this.currentPhase();
	};

}

function changeTerritoryOwner (newOwner, territory) {
	var oldOwner = territory.owner;
	if (oldOwner !== undefined) {
		oldOwner.removeTerritory(territory);
	}
	newOwner.addTerritory(territory);
	territory.setOwner(newOwner);
}

Game.prototype.serializeTerritories = function () {
	var serialized = new Array(this.territories.length);
	for (var i = 0; i < this.territories.length; ++i) {
		serialized[territories[i].zone] = territories[i].owner;
	}
	return serialized;
};