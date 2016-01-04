# MGSE-Project
Y3 - MGSE Project

###Setup
1. Launch mongod from the install directory C:/Program Files/MongoDB/Server/3.2/bin with the command:

mongod --dbpath "C:/Users/t_j_w/Work/University/Multiplayer Game Software Engineering/MGSE_Project/NodejsServer/data/"

1. From the NodejsServer directory containing the server.js file, run 'npm start'
1. Run client application

###(Optional)
1. use Mongo.exe from Command Prompt to interact with server.
2. The data used by the server is stored in the database 'PlayerDB' and the collection 'players'. To list all players from Mongo.exe, into the command prompt type 'use PlayerDB', then 'db.players.find().pretty()'
