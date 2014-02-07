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
	this.MAXTURNS = 2;
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
		
		// if (questionID == self.currentQuestion) {

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
			
			self.checkIfAllAnswers();
		// }
	});
	
	function nextPhase1(msg)
	{
		// send the number of territories each player have to capture
		processTerritoriesToCapture();
	}

	self.processAnswers = function()
	{
		// find winner and attribute territories
		console.log("process answer phase 1");
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
		//nextPhase1();

	}

	// wait for answers
	self.checkIfAllAnswers = function() {
	
		console.log("check if all answer phase 1");
		if (self.playersAnswers.length < self.players.length) {
			console.log('some players did not answer');
		}else{
			console.log('all players answered');
			self.processAnswers();
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

	self.table.on('nextPhase1', nextPhase1);
	
	
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
			var q = null;
			while (q == null || q.type != 'open') {
				q = self.questions[Math.floor(Math.random()* self.questions.length)];
			}
			self.question = q;
			self.question.id = ++self.currentQuestion;

			for (var i = 0; i < self.players.length; ++i) {

				var p = self.players[i];

				console.log("send question");
				p.playerSocket.emit('question', self.question);
			}

			self.table.emit('question', self.question.title);
			console.log("question emitted");
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
						self.checkIfAllAnswers();
					}
				};
			}

			//Event binding for each player
			for (var i = 0; i < self.players.length; ++i) {

				var p = self.players[i];
				
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
			console.log("phase 1 over");
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
			 console.log("phase 2 over");
			self.phase3();
		};

	};


	//	Phase 3 : Conquête du monde
	this.phase3 =  function () {
		console.log('phase 3 started');
		
		var phase3 = this;
		
		//Faudrais peut etre override 'next' plutot
		// change the processAnswers method
		self.processAnswers = function () {
			// find winner and attribute territories
			
			console.log("process answer phase 3");
			self.playersAnswers.sort(function (answer1, answer2)
			{
				if (answer1.value == 'void') {
					return 1;
				}
				else if (answer2.value == 'void') {
					return -1;
				}
				
				if (self.question.type == 'qcm') {
					if (answer1.value == answer2.value) {
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
			
			//Send the battle results to the tables
			//Emit a tout le monde ?
			self.table.emit('battleResult', {
			'winner' : orderedPlayers[0]
			, 'loser' : orderedPlayers[1]
			, 'winVal' : self.playersAnswers[0].value
			, 'lossVal' : self.playersAnswers[1].value});

		};
		
		
		self.checkIfAllAnswers = function() {
			console.log("check if all answer phase 3");
			if (self.playersAnswers.length < 2) {
				console.log('some players did not answer');
			}else{
				console.log('all players answered');
				self.processAnswers();
			}
		}
		//Réordonner la liste des joueurs par ordre inverse du nombre de zone (sisi, cette phrase à un sens)
		var phase3players = self.players;
		
		//self.table.emit('startPhase3', {});
		
		//number of turns played since beginning of phase
		var turnCount = 0;
		
		var nbMovesInTurn = 0;
		
		function nextPhase3()
		{
			console.log("phase 3 next, turnCount : " + turnCount +" nbMOvesInTurn : "+nbMovesInTurn);
			
			nbMovesInTurn += 1;
			
			if(nbMovesInTurn >= phase3players.length)
			{
				nbMovesInTurn = 0;
				turnCount += 1;
			}
			
			if(turnCount >= self.MAXTURNS)
			{
				console.log("phase 3 over");
				self.phase4();
			}
			else
			{
				console.log('send : play player :' + phase3players[nbMovesInTurn].gameID);
				io.sockets.emit('playerMoveAsked', {'id' : phase3players[nbMovesInTurn].gameID});
			}
		}
		
		self.table.on('startBattle', function(msg)
			{
				console.log('start battle asked');
				var play1 = self.players[msg.player1];
				var play2 = self.players[msg.player2];
				
				//send question to both players (and to the table)
				
				self.playersAnswers.length = 0;
				// Pick a question from the database (not like that, a random question)
				self.question = self.questions[Math.floor(Math.random()* self.questions.length)];

				self.question.id = ++self.currentQuestion;

				play1.playerSocket.emit('question', self.question);
				play2.playerSocket.emit('question', self.question);
				self.table.emit('question', self.question.title);
				
			});

		self.table.on('nextPhase3', nextPhase3);
		
		self.table.on('territoryWon', function(msg)
			{
				
				var zone = msg.zone;
				var owner = msg.owner;
				
				console.log('territory : '+zone+' by : '+owner);
				
				for(var i = 0; i < self.territories.length; ++i)
				{
					if(self.territories[i].zone == zone)
					{
						changeTerritoryOwner(self.players[owner], self.territories[i]);
					}
				}
				var majChart = [];
				for (var j = 0; j < self.players.length; ++j) {
					majChart.push(self.players[j].serialize());
				}
				io.sockets.emit ('majChart', majChart);

			});
		
		//Start first move
		io.sockets.emit('playerMoveAsked', {'id' : phase3players[nbMovesInTurn].gameID});
		
	};


	//	Phase 4 : Fin
	this.phase4 = function () {
		console.log('phase 4 started');
		this.players.sort(function (player1, player2) {
			return player1.territories.length - player2.territories.length;
		});
		var seriPlay = [];
		
		for(var i = 0; i < 4; ++i)
		{
			seriPlay.push(self.players[i].gameID);
		}
		this.table.emit('results', seriPlay);
		var position = 0;
		for (var i = 0; i < this.players.length; ++i) {
			self.players[i].playerSocket.emit('results', position++);
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
