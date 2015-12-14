var net = require("net");
var buffer = require("buffer");
var stream = require("stream");
var clients = [];
var clientNames = [];
var maxClientsPerLobby = 2;
var playerlist = [];

function Lobby() {
    this.lobbyClients = [];
    this.playerNames = [];
}
var lobbies = [];

var tcp_server = net.createServer(function(socket)
{
    // Process new clients
    //clients.push(socket);
    //console.log('\tclient connected');
    for(var i = 0; i < lobbies.length; i++)
    {
        console.log("i = " + i + " lobbylength = " + lobbies.length);
        if(lobbies[i].lobbyClients.length < maxClientsPerLobby)
        {
            console.log("Client added to lobby: " + i);
            lobbies[i].lobbyClients.push(socket);
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
	  {
		if(jsonObject.command == "Join") {
			//playerlist.push(jsonObject.message);
            lobbies[FindLobby(socket)].playerNames.push(jsonObject.message);
		    console.log("Player Joined: " + jsonObject.message);
            var reply = JSON.stringify({"ServerName" : FindLobby(socket)});
            var packet = JsonStringToData(reply);
            var buffer = new Buffer(packet);
            socket.write(buffer);
		}

		if(jsonObject.command == "PlayerList" ) {
		  //var replyMessage = JSON.stringify({"type" : "PlayerList", "players" : playerlist});
            var replyMessage = JSON.stringify({"type" : "PlayerList", "players" : lobbies[FindLobby(socket)].playerNames});
		  var fullMessage = JsonStringToData(replyMessage);
		  var buffer = new Buffer(fullMessage);
		  socket.write(buffer);
		}

	  }
	  // Message for clients to read.
	  else
		broadcast(data, socket);

	});
	socket.on('end', function() {
	  clients.splice(clients.indexOf(socket), 1);
	  console.log(' X\tclient disconnected');
	});
	socket.on('close', function() {
	  console.log(' X\tclient lost unexpectedly');
	});
    socket.on('error', function() {
        console.log(' X\t socket error');
    });

	// Send message to all clients except sender.
	function broadcast(message, sender)
	{
        var sendersLobby = FindLobby(sender);
        lobbies[sendersLobby].lobbyClients.forEach(function (client)
        {
            if(client === sender) return;
            client.write(message);
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
});

function FindLobby(sender) {
    for(var i = 0; i < lobbies.length; i++)
    {
        //if(lobbies[i].lobbyClients.contains(sender))
        for(var x = 0; x < lobbies[i].lobbyClients.length; x++)
        {
            if(lobbies[i].lobbyClients[x] == sender) {
                //console.log("Found client in lobby: " + i);
                return i;
            }
        }
    }
};

// Convert a received byte array to a json string.
function DataToJsonString(data) {
  var stringData = data.toString();

  var messageBytes = utf8.toByteArray(stringData);
  var size = messageBytes[0];
  var parsedMessageBytes = messageBytes.slice(4, size + 4);
  return utf8.parse(parsedMessageBytes);
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

// UTF8 to byte array functions.
// Source: http://stackoverflow.com/questions/1240408/reading-bytes-from-a-javascript-string
// User: Kadm
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