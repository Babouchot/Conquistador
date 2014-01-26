//Game.js
//========

exports = module.exports = Game;

var Territory = require('./Territory.js');

var TERRITORIES = 16;
var PLAYER_NUMBER = 4;

function Game(playersArray, tableSocket, io) {
	this.players = playersArray;
	this.table = tableSocket;
	this.io = io;
	this.MAXTURNS = 12;
	this.currentTurn = 0;
	this.playersAnswers = [];

	this.currentQuestion = 0;

	// parse question database file
	this.questionsFile = require('./questions.js');
	this.questions = new this.questionsFile();
	//current question
	this.question = null;

	this.territories = new Array();

	var self = this;
	

	this.table.on('timeout', function (message) {
	
		console.log ('TIMEOUT !!!');
		var questionID = message.id;
		console.log("QuestionId : " + questionID);
		console.log("currentQuestion : " + self.currentQuestion);
		
		if (questionID == self.currentQuestion) {
			console.log("gnééééééééééééé");
			var playersDidNotAnswer = new Array();
			
			for (var i = 0; i < self.players.length; ++i) {
				console.log ("i : " + i);
				playersDidNotAnswer.push(self.players[i].gameID);
				self.players[i].playerSocket.emit('timeout', message);
			}
			
			for (var j = 0; j < self.playersAnswers.length; ++j) {
				console.log(self.playersAnswers[j].id);
				var index = playersDidNotAnswer.indexOf(self.playersAnswers[j].id);
				if (index > -1) {
					console.log("before splice : " + playersDidNotAnswer);
					playersDidNotAnswer.splice(index, 1);
					console.log("after splice : " + playersDidNotAnswer);
				}
			}
			console.log(playersDidNotAnswer);
			
			for (var k = 0; k < playersDidNotAnswer.length; ++k) {
				self.playersAnswers.push({'id' : playersDidNotAnswer[k], 'value' : "void", 'time': -1});
				console.log(playersDidNotAnswer);
			}
			
			checkIfAllAnswers();
		}
	});



	function processAnswers()
	{
		// find winner and attribute territories

		self.playersAnswers.sort(function (answer1, answer2) {
			if (answer1.value == 'void') {
				return 1;
			}
			else if (answer2.value == 'void') {
				return -1;
			}
			
			var answerOffset1 = Math.abs(answer1.value - self.question.answer);
			var answerOffset2 = Math.abs(answer2.value - self.question.answer);
			console.log(" sort : answer1 : "+answerOffset1+" answer2 : "+answerOffset2);
			if (answerOffset1 == answerOffset2) {
				return answer1.time - answer2.time;
			}
			return answerOffset1 - answerOffset2;
		});
		
		/*var orderedPlayers = new Array();
		
		for (var i = 0; i < self.playersAnswers.length; ++i) {
			orderedPlayers.push({'player' : self.playersAnswers[i].id
								, 'answer' : self.playersAnswers[i].value
								, 'time' : self.playersAnswers[i].time});
		}
		console.log("ordered answers : " + orderedPlayers);
		*/
		// send the number of territories each player have to capture
		
		//send message to the table
		self.table.emit('questionAnswered', {'orderedAnswers' : self.playersAnswers});


	}

	// wait for answers
	function checkIfAllAnswers() {
		if (self.playersAnswers.length < self.players.length) {
			console.log('some players did not answer');
		}else{
			console.log('all players answered');
			processAnswers();
		}
	}

	processTerritoriesToCapture = function()
	{
		var orderedPlayers = new Array();
		
		for (var i = 0; i < self.playersAnswers.length; ++i) {
			orderedPlayers.push(self.playersAnswers[i].id);
		}
		console.log("ordered players : " + orderedPlayers);
		
		// send the number of territories each player have to capture
		
		//send message to the table
		self.table.emit('captureTerritories', {'orderedPlayers' : orderedPlayers});
	}
	
	function next(msg)
	{
		// send the number of territories each player have to capture
		processTerritoriesToCapture();
	}

	self.table.on('next', next);
	
	
	//	Phase 1 : Prise des territoires (Attribution des territoires sur question)
	this.phase1 = function () {

		console.log('phase 1 started');
		this.playerCaptureCount = PLAYER_NUMBER;

		var phase1 = this;
	
		function sendNewQuestion()
		{
			phase1.playerCaptureCount = PLAYER_NUMBER;
			self.playersAnswers.length = 0;
			// Pick a question from the database (not like that, a random question)
			self.question = self.questions[0];

			self.question.id = ++self.currentQuestion;

			for (var i = 0; i < self.players.length; ++i) {

				var p = self.players[i];

				console.log("send question");
				p.playerSocket.emit('question', self.question);
			}

			self.table.emit('question', self.question);
		}

		
		
		this.init = function () {
			// give one territory to each player
			// Attributing random territories
			var majChart = [];
			for (var i = 0; i < self.players.length; ++i) {

				var territory = new Territory(Math.floor(Math.random()*3 + i*4));
				self.territories.push(territory);
				changeTerritoryOwner(self.players[i], territory);
				self.table.emit('majPlayerInfo', self.players[i].serialize());
				majChart.push(self.players[i].serialize());
				// self.players[i].playerSocket.emit('majPlayerInfo', self.players[i].serialize());
				// self.io.sockets.emit('majPlayerInfo', self.players[i].serialize());
			}
			io.sockets.emit ('majChart', majChart);

			// get the new territories
			self.table.on('capturedTerritories', function (message) {

				var gameID = message.gameID;
				var territories = message.territories;
				console.log("captured zones : "+territories);
				for (var i = 0; i < territories.length; ++i) {
					console.log("adding captured zone : " + territories[i]);
					var territory = new Territory(territories[i]);
					self.territories.push(territory);
					changeTerritoryOwner(self.players[gameID], territory);

					self.table.emit('majPlayerInfo', self.players[gameID].serialize());

					// self.players[gameID].playerSocket.emit('majPlayerInfo', self.players[gameID].serialize());
					// self.io.sockets.emit('majPlayerInfo', self.players[i].serialize());

				}
				var majChart = [];
				for (var j = 0; j < self.players.length; ++j) {
					majChart.push(self.players[j].serialize());
				}
				io.sockets.emit ('majChart', majChart);

				phase1.playerCaptureCount--;
				console.log("phase1.playerCaptureCount " + phase1.playerCaptureCount);
				//check if all capture have been done
				if(phase1.playerCaptureCount == 0){
					console.log("nb ter " +self.territories.length+" to : " + TERRITORIES); 
					if(self.territories.length >= TERRITORIES){ // check if all territotires have been taken
						//go to next phase
						phase1.nextPhase();
					} else {//if free zones remain
						sendNewQuestion();
					}
				}
			});

			function answerEventGenerator(pla)
			{
				return function (message) {
					var answer = message.answer;
					var time = message.time;

					var questionID = message.id;

					if (questionID != self.currentQuestion) {
						console.log('Timeout answer');
						console.log('questionID : ' + questionID);
						console.log('currentQuestion : ' + self.currentQuestion);
					}
					else {
						console.log("answer");
						self.playersAnswers.push({'id' : pla.gameID, 'value' : answer, 'time': time});
						console.log('id:' + pla.gameID + '/answer:' + answer + '/time:' + time);
						checkIfAllAnswers();
					}
				};
			}

			//Event binding for each player
			for (var i = 0; i < self.players.length; ++i) {

				var p = self.players[i];
				/* -----CANNOT WORK----- ! See : http://blog.jbrantly.com/2010/04/creating-javascript-function-inside.html
				* The reason that this is true is somewhat complex, but in basic terms,
				* the function is only actually created once (instead of once each iteration of the loop)
				* and that one function points to the last known values of the variables it uses.
				
				p.playerSocket.on('answer', function (message) {

					console.log("answer");
					var answer = message.answer;
					var time = message.time;
					var gameID = message.gameID;
					self.playersAnswers.push({'id' : p.gameID, 'answer' : answer, 'time': time});
					console.log('id:' + p.gameID + '/answer:' + answer + '/time:' + time);
					checkIfAllAnswers();
				});
				*/
				
				//----FIX----
				p.playerSocket.on('answer', (answerEventGenerator(p)));
			}

			sendNewQuestion();

		};

		
		this.update = function () {

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
			console.log("phase 1 over, phase 2 not implemented yet");
			self.phase2();
		};
		
		//initialize itself
		this.init();

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
			console.log("phase 2 over, phase 3 not implemented yet");
			self.phase3();
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
		
		//Faudrais peut etre override 'next' plutot
		// change the processAnswers method
		self.processAnswers = function () {
			// find winner and attribute territories

			self.playersAnswers.sort(function (answer1, answer2) {
				if (answer1.value == 'void') {
					return 1;
				}
				else if (answer2.value == 'void') {
					return -1;
				}
				
				if (self.question.type == 'qcm') {
					if (answer1.value == answer1.value) {
						return answer1.time - answer2.time;
					}
					else if (answer1.value == self.question.answer) {
						return -1;
					}
					else if (answer2.value == self.question.answer) {
						return 1;
					}
				}
				else {
					var answerOffset1 = Math.abs(answer1.value - self.question.answer);
					var answerOffset2 = Math.abs(answer2.value - self.question.answer);
					console.log(" sort : answer1 : "+answerOffset1+" answer2 : "+answerOffset2);
					if (answerOffset1 == answerOffset2) {
						return answer1.time - answer2.time;
					}
					return answerOffset1 - answerOffset2;
				}
				return answer1.time - answer2.time;
			});

			
			var orderedPlayers = new Array();
			
			for (var i = 0; i < self.playersAnswers.length; ++i) {
				orderedPlayers.push(self.playersAnswers[i].id);
			}
			console.log("ordered players : " + orderedPlayers);
			
			// send the number of territories each player have to capture

			self.table.emit('captureTerritories', {'orderedPlayers' : orderedPlayers});

		};
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

	
	this.start = function () {
		/*var phase1 = new this.phase1();
		phase1.init();*/
		this.phase1();
		//phase1.update();
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
