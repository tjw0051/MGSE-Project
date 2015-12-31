var mongo = require('mongodb');
var monk = require('monk');


var net = require("net");
var buffer = require("buffer");
var stream = require("stream");
var clients = [];
var clientNames = [];
var maxClientsPerLobby = 2;
var playerlist = [];

function Client(socket, name) {
    this.clientSocket = socket;
    this.clientName = name;
}

function Lobby() {
    //this.lobbyClients = [];
    //this.playerNames = [];
    this.clients = [];
}
var lobbies = [];

var db = monk('localhost:27017/PlayerDB');
var collection = db.get('players');

var tcp_server = net.createServer(function(socket) {

    //Connect Client
    var newClient = new Client(socket, "");
    for(var i = 0; i < lobbies.length; i++)
    {
        console.log("i = " + i + " lobbylength = " + lobbies.length);
        if(lobbies[i].clients.length < maxClientsPerLobby)
        {
            console.log("Client added to lobby: " + i);
            lobbies[i].clients.push(newClient);
        }
        else if(i == (lobbies.length - 1))
        {
            console.log("Adding client to new lobby: " + (i+1));
            lobbies.push(new Lobby());
            //lobbies[i + 1].lobbyClients.push(socket);
        }
    }



	// Message received.
	socket.on('data', function(data) {
	  // Convert message to JSON Object.
	  var stringInput = data.toString();
	  var jsonString = DataToJsonString(data);
	  var jsonObject = JSON.parse(jsonString);

	  // Message for server to read.
	  if (jsonObject.type == "ServerMessage")
	  { //console.log('Recieved ServerMessage');
		if(jsonObject.command == "Join") {
			//playerlist.push(jsonObject.message);
            lobbies[FindLobby(socket)].clients.forEach(function (client) {
                if(client.clientSocket == socket) {
                    client.clientName = jsonObject.message;
                }
            });

		    console.log("Player Joined: " + jsonObject.message);
            sendMessage(JSON.stringify({"ServerName" : FindLobby(socket)}), socket);

            //Log Player in Database
            collection.find({ name : jsonObject.message}, {}, function(err, docs) {
                if(docs.length > 0) {
                    console.log("Player found in DB. sending settings...");
                    sendMessage(DbToPlayerState(docs[0]), socket);
                }
                else
                {
                    console.log("new player, inserting to database: " + jsonObject.message);
                    collection.insert({"type":"PlayerState",
                                        "name":jsonObject.message,
                                        "size":49,
                                        "posX":0,
                                        "posY":0,
                                        "velX":0,
                                        "velY":0},
                                        {} );
                }
            });
		}

		if(jsonObject.command == "PlayerList" ) {
      //console.log('Recieved PlayerList');
		  //var replyMessage = JSON.stringify({"type" : "PlayerList", "players" : playerlist});
            sendMessage(JSON.stringify({"type" : "PlayerList", "players" : GetPlayerlist(FindLobby(socket))}), socket);
		}

	  }
      if(jsonObject.type == "PlayerState")
      {
        //console.log('Recieved PlayerState');
          broadcast(data, socket);
          //Update server entry
          collection.update({name : jsonObject.name}, { $set: { size : jsonObject.size,
                                                                  posX : jsonObject.posX,
                                                                  posY : jsonObject.posY,
                                                                  velX : jsonObject.velX,
                                                                  velY : jsonObject.velY}}, function(err) {
              //console.log("error updating");
          });
      }
	  // Message for clients to read.
	  else
    {
      //console.log('Broadcasting unknown message');
		    broadcast(data, socket);
    }

	});
	socket.on('end', function() {
	  clients.splice(clients.indexOf(socket), 1);
	  console.log(' X\tclient disconnected');
	});
	socket.on('close', function() {
	  console.log(' X\tclient lost unexpectedly');
        //Remove player from lobby when they leave.
        var sendersLobby = FindLobby(socket);
        for(var i = 0; i < lobbies[sendersLobby].clients.length; i ++)
        {
            if(lobbies[sendersLobby].clients[i].clientSocket === socket) {
                var disconnectMessage = JSON.stringify({"type" : "RemovePlayer", "name" : lobbies[sendersLobby].clients[i].clientName});
                var fullMessage = JsonStringToData(disconnectMessage);
                console.log(lobbies[sendersLobby].clients[i].clientName);
                console.log(disconnectMessage);
                broadcastString(fullMessage, socket);
                lobbies[sendersLobby].clients.splice(i, 1);
                return;
            }
        }

	});
    socket.on('error', function() {
        console.log(' X\t socket error');
    });

    function sendMessage(message, client) {
        var packet = message;
        if(typeof message == "string")
            packet = JsonStringToData(message);
        var buffer = new Buffer(packet);
        client.write(buffer);
    }

	// Send message to all clients except sender.
	function broadcastString(message, sender)
	{
        var sendersLobby = FindLobby(sender);
        lobbies[sendersLobby].clients.forEach(function (client)
        {
            if(client.clientSocket != sender) {
                //console.log("sending broadcast to client");
                sendMessage(message, client.clientSocket);
            }
        });
        /*
	  clients.forEach(function (client)
	  {
		if(client === sender) return;
		client.write(message);
	  });
	  */
	}
    function broadcast(message, sender)
    {
        var sendersLobby = FindLobby(sender);
        lobbies[sendersLobby].clients.forEach(function (client)
        {
            if(client.clientSocket != sender) {
                //console.log("sending broadcast to client");
                client.clientSocket.write(message);
            }
        });
        /*
         clients.forEach(function (client)
         {
         if(client === sender) return;
         client.write(message);
         });
         */
    }

});
tcp_server.listen(8888, function() {
  console.log("\tserver bound");
    lobbies[0] = new Lobby();

    //Database Tests
    //collection.update({name : 'Elmo'}, { $set: { velX : '2' }}, function(err) {
    //    console.log("error updating");
    //});

});

function FindLobby(sender) {
    for(var i = 0; i < lobbies.length; i++)
    {
        //if(lobbies[i].lobbyClients.contains(sender))
        //for(var x = 0; x < lobbies[i].lobbyClients.length; x++)
        for(var x = 0; x < lobbies[i].clients.length; x++)
        {
            if(lobbies[i].clients[x].clientSocket == sender) {
                //console.log("Found client in lobby: " + i);
                return i;
            }
        }
    }
};


function GetPlayerlist(lobby) {
    var playerNames = [];
    lobbies[lobby].clients.forEach(function (client) {
        playerNames.push(client.clientName)
    });
    return playerNames;
};

// Convert a received byte array to a json string.
function DataToJsonString(data) {
  //console.log('Raw Data: ' + data);
  var stringData = data.toString();

  //console.log('Raw String: ' + stringData);
  var jsonStartIndex = stringData.indexOf('{\"');
  var jsonEndIndex;
  var jsonBracketIncrement = 0;
  for(var i = jsonStartIndex + 1; i < stringData.length; i++)
  {
    if(stringData.charAt(i) == '{')
      jsonBracketIncrement++;
    if(stringData.charAt(i) == '}') {
      if(jsonBracketIncrement == 0) {
        jsonEndIndex = i;
        break;
      }
      else {
        jsonBracketIncrement--;
      }
    }
  }

  //console.log('index: ' + jsonStartIndex + ' LastIndex: ' + jsonEndIndex);
  var result = stringData.slice(jsonStartIndex, jsonEndIndex + 1);
   if(stringData.search('PickupList') > -1) {
  //   console.log('Raw String: ' + stringData);
  //   console.log('index: ' + jsonStartIndex + ' LastIndex: ' + jsonEndIndex);
     console.log('Result: ' + result);
   }
  //console.log('Result: ' + result);
  //console.log('Data = ' + stringData);
  //var messageBytes = utf8.toByteArray(stringData);
  //var size = messageBytes[0];
  //var parsedMessageBytes = messageBytes.slice(4, size + 4);
  //return utf8.parse(parsedMessageBytes);
  return result;
};

// Convert a JSON string to a byte array, prepended with 4 bytes representing the size of the json string.
function JsonStringToData(jsonString) {
  var messageBytes = utf8.toByteArray(jsonString);
  var fullMessage = new Array(4 + messageBytes.length);
  fullMessage[0] = messageBytes.length;
  for(var i = 0; i < messageBytes.length; i++)
  {
	fullMessage[4+i] = messageBytes[i];
  }
  return fullMessage;
};

function DbToPlayerState(dbEntry) {
    var playerState = JSON.stringify({"type" : dbEntry.type,
                                            "name" : dbEntry.name,
                                            "size" : dbEntry.size,
                                            "posX" : dbEntry.posX,
                                            "posY" : dbEntry.posY,
                                            "velX" : dbEntry.velX,
                                            "velY" : dbEntry.velY});
    return playerState;

};

// UTF8 to byte array functions.
// Source: http://stackoverflow.com/questions/1240408/reading-bytes-from-a-javascript-string
// Author: Kadm
// Obtained: 07/12/2015
var utf8 = {}
utf8.toByteArray = function(str) {
  var byteArray = [];
  for (var i = 0; i < str.length; i++)
	if (str.charCodeAt(i) <= 0x7F)
	  byteArray.push(str.charCodeAt(i));
	else {
	  var h = encodeURIComponent(str.charAt(i)).substr(1).split('%');
	  for (var j = 0; j < h.length; j++)
		byteArray.push(parseInt(h[j], 16));
	}
  return byteArray;
};
utf8.parse = function(byteArray) {
  var str = '';
  for (var i = 0; i < byteArray.length; i++)
	str +=  byteArray[i] <= 0x7F?
		byteArray[i] === 0x25 ? "%25" : // %
			String.fromCharCode(byteArray[i]) :
	"%" + byteArray[i].toString(16).toUpperCase();
  return decodeURIComponent(str);
};
