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

	var self = this;

	//	Phase 1 : Prise des territoires (Attribution des territoires sur question)
	this.phase1 = function () {	
		var playersAnswers = [];
		console.log('phase 1 started');
	
	
		this.init = function () {
			// give one territory to each player
			
			// parse question database file
			var questionsFile = require('./questions.js');
			var questions = new questionsFile();
			console.log(questions);

			// Attributing random territories
			for (var i = 0; i < players.length; ++i) {
				var territory = new Territory(Math.random()*3 + i*4);
				players[i].addTerritory(territory);
				self.table.emit('majPlayerInfo', players[i].serialize());
			}

		};
		
		this.update = function () {
			// Pick a question from the database
			var question;
			//this.table.emit('question', question);
			for (var i = 0; i < self.players.length; ++i) {
				var p = self.players[i];
				p.playerSocket.emit('question', question);
				self.table.emit('question', question);
				p.playerSocket.on('answer', function (answer) {
					playersAnswers.push({'id' : p.gameID, 'answer' : answer});
				});
			}
			
			// wait for answers
			function waitAnswers() {
				if (playersAnswers.length < self.players.length) {
					setTimeout(waitAnswers, 200);
				}
			}
			waitAnswers();
			
			// find winner and attribute territories
			playersAnswers.sort(function (answer1, answer2) {
				var answerOffset1 = Math.abs(answer1.value - question.correctAnswer);
				var answerOffset2 = Math.abs(answer2.value - question.correctAnswer);
				
				if (answerOffset1 == answerOffset2) {
					return answer1.time - answer2.time;
				}
				return answerOffset1 - answerOffset2;
			});
			
			var capturedTerritories = new Array(4);
			capturedTerritories[playersAnswers[0].id] = 2;
			capturedTerritories[playersAnswers[1].id] = 1;
			capturedTerritories[playersAnswers[2].id] = 1;
			capturedTerritories[playersAnswers[3].id] = 0;

			// send the number of territories each player have to capture
			self.table.emit('capturePhase', capturedTerritories);
			
			// create the new assigned territories and assign the correct owner
			self.table.on ('endCapturePhase', function (capturedTerritories) {
				for (var i = 0; i < capturedTerritories.length; ++i) {
					var territory = new Territory(capturedTerritories[i].zone);
					self.territories.push(territory);
					changeTerritoryOwner(players[capturedTerritories[i].owner], territory);
				}
			});
			
			// send the updated list of territories to the table
			self.table.emit ('updateMapState', self.serializeTerritories());
			
			// check if all territories are assigned
			if (self.territories.length >= TERRITORIES) {
				return false;
			}
			return true;
		};
	};



	//	Phase 2 : Déploiement des armées (Placement des pions/tags sur les zones controllées)
	this.phase2 = function () {
		console.log('phase 2 started');
		var phase2 = this;
		self.table.on('endPhase2', function (territories) {

		});
	};

	//	Phase 3 : Conquête du monde
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

	//	Phase 4 : Fin
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
		var phase1 = new this.phase1();
		phase1.init();
		phase1.update();
		//this.currentPhase();
	};

}

Game.prototype.serializeTerritories = function () {
	var serialized = new Array(this.territories.length);
	for (var i = 0; i < this.territories.length; ++i) {
		serialized[territories[i].zone] = territories[i].owner;
	}
	return serialized;
};



function changeTerritoryOwner (newOwner, territory) {
	var oldOwner = territory.owner;
	if (oldOwner !== undefined) {
		oldOwner.removeTerritory(territory);
	}
	newOwner.addTerritory(territory);
	territory.setOwner(newOwner);
}
