//Game.js
//========

exports = module.exports = Game;

var Territory = require('./Territory.js');

var TERRITORIES = 16;
var PLAYER_NUMBER = 4;

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
		this.count = PLAYER_NUMBER;

		var phase1 = this;
	
		// parse question database file
		var questionsFile = require('./questions.js');
		var questions = new questionsFile();


		this.init = function () {
			// give one territory to each player
			// Attributing random territories
			for (var i = 0; i < self.players.length; ++i) {
				var territory = new Territory(Math.floor(Math.random()*3 + i*4));
				self.territories.push(territory);
				changeTerritoryOwner(self.players[i], territory);
				self.table.emit('majPlayerInfo', self.players[i].serialize());
			}

			// get the new territories
			self.table.on('capturedTerritories', function (message) {
				var gameID = message.gameID;
				var territories = message.territories;
				for (var i = 0; i < territories.length; ++i) {
					var territory = new Territory(territories[i]);
					self.territories.push(territory);
					changeTerritoryOwner(players[gameID], territory);
					self.table.emit('majPlayerInfo', players[gameID].serialize());
				}
				phase1.count--;
			});

		};
		

		this.update = function () {
			// Pick a question from the database
			var question = questions[0];
			console.log("fuck");
			//this.table.emit('question', question);
			for (var i = 0; i < self.players.length; ++i) {
				var p = self.players[i];
				console.log("send question");
				p.playerSocket.emit('question', question);
				self.table.emit('question', question);
				// console.log('question :' + question.title);
				p.playerSocket.on('answer', function (message) {
					console.log("answer");
					var answer = message.answer;
					var time = message.time;
					var gameID = message.gameID;
					playersAnswers.push({'id' : p.gameID, 'answer' : answer, 'time': time});
					console.log('id:' + p.gameID + '/answer:' + answer + '/time:' + time);
					waitAnswers();
				});
			}
			
			// wait for answers
			function waitAnswers() {
				if (playersAnswers.length < self.players.length) {
					console.log('timeout');

					//setTimeout(waitAnswers, 200);
				}else{
					afterQuesions();
				}
			}
			//waitAnswers();
			///console.log('end timeout');
			
			function afterQuesions()
			{
				// find winner and attribute territories
				playersAnswers.sort(function (answer1, answer2) {
					var answerOffset1 = Math.abs(answer1.value - question.answer);
					var answerOffset2 = Math.abs(answer2.value - question.answer);
					
					if (answerOffset1 == answerOffset2) {
						return answer1.time - answer2.time;
					}
					return answerOffset1 - answerOffset2;
				});
				
				

				var orderedPlayers = new Array();

				for (var i = 0; i < playersAnswers.length; ++i) {
					orderedPlayers.push(playersAnswers[i].id);
				}

				
				// send the number of territories each player have to capture
				phase1.count = PLAYER_NUMBER;
				self.table.emit('captureTerritories', orderedPlayers);

				function waitTerritories () {
					if (phase1.count > 0) {
						setTimeout(waitTerritories, 200);
					}
				}
				waitTerritories();
				}

			// PHASE 2
			// // create the new assigned territories and assign the correct owner
			// self.table.on ('endCapturePhase', function (capturedTerritories) {
			// 	for (var i = 0; i < capturedTerritories.length; ++i) {
			// 		var territory = new Territory(capturedTerritories[i].zone);
			// 		self.territories.push(territory);
			// 		changeTerritoryOwner(players[capturedTerritories[i].owner], territory);
			// 	}
			// });
			
			// // send the updated list of territories to the table
			// self.table.emit ('updateMapState', self.serializeTerritories());
			
			// // check if all territories are assigned
			// if (self.territories.length >= TERRITORIES) {
			// 	return false;
			// }
			// return true;

		}; // Phase 1 update


		this.nextPhase = function () {
			var phase2 = new this.phase2();

			function waitPhase2() {
				if (phase2.count > 0) {
					setTimeout(waitAnswers, 200);
				}
			}
			waitPhase2();
		};

	}; // Phase1



	//	Phase 2 : Déploiement des armées (Placement des pions/tags sur les zones controllées)
	this.phase2 = function () {

		console.log('phase 2 started');
		this.count = PLAYER_NUMBER;
		var phase2 = this;


		self.table.emit('placeCommanders', {});
		self.table.on ('commandersPlaced', function () {
			phase2.nextPhase();
		});

		this.nextPhase = function () {

		};

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
		this.table.emit('results', players.serialize());
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
		// phase1.nextPhase();

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
