const path = require("path");

// const mysql = require("mysql");

const myConnection = require("express-myconnection");
const routes = require("./routes/myroutes");
var bodyParser = require("body-parser");
const cookieParser = require("cookie-parser");
var express = require("express");
var app = express();
var cors = require("cors");
const util = require("util");

var server = require("http").createServer(app);
var port = process.env.PORT || 7877;

// Start server
// server.listen(port);
server.listen(port, function () {
  console.log("listening on *:" + port + "  \n --- server is running ...");
});
const io = require("socket.io")(server, {cors: {origin: "*", methods: ["GET", "POST"] }});
// const io = require("socket.io")(server, { origins: "*:*" });

// const connection = require("./db");
const { connectDB, closeDB } = require("./db");
const { debug } = require("console");

// var port = process.env.PORT || 3000,
//   io = require('socket.io')(port),
gameSocket = null;


// global variables for the server
var enemies = [];
var playerSpawnPoints = [];
var clients = []; // Store Client list
var sockets = {};

var rooms = {};
var joinedRooms = {};
var challengeRooms = {};
var users = [];

const makeDb = (connection) => {
  return {
    query(sql, args) {
      return util.promisify(connection.query).call(connection, sql, args);
    },
    close() {
      return util.promisify(connection.end).call(connection);
    },
  };
};

const getRooms = () => {
  var res = [];

  for (var id in rooms) {
    if (
      (joinedRooms.hasOwnProperty(rooms[id].id) && joinedRooms[rooms[id].id]) ||
      (challengeRooms.hasOwnProperty(rooms[id].id) &&
        challengeRooms[rooms[id].id])
    ) {
      continue;
    }

    var element = {};
    element.id = rooms[id].id;
    element.name = rooms[id].name;
    res.push(element);
  }

  console.log("*** Created Rooms");
  console.log(res);

  return res;
};

const joinRoom = (socket, room) => {
  rooms[room.id].sockets.push(socket);
  socket.join(room.id, () => {
    // store the room id in the socket for future use
    socket.roomId = room.id;
    console.log(socket.id, "Joined", room.id);

    if (room.sockets.length == 2) {
      deleteChallengeRoomDB(room);

      joinedRooms[room.id] = true;

      socket.broadcast.emit("show room", { rooms: getRooms() });

      rooms[room.id].sockets.map((s, index) => {
        s.emit("gameTurn", { turn: index + 1, playing: 2 });
      });

      //socket.emit("gameTurn", {turn: 2, playing: 1});
    }
  });
};

const leaveRooms = (socket) => {
  const roomsToDelete = [];
  for (const id in rooms) {
    const room = rooms[id];
    // check to see if the socket is in the current room
    if (room.sockets.includes(socket)) {
      socket.leave(id);
      // remove the socket from the room object
      room.sockets = room.sockets.filter((item) => item !== socket);
    }
    // Prepare to delete any rooms that are now empty
    if (room.sockets.length == 0) {
      roomsToDelete.push(room);
    }
  }

  // Delete all the empty rooms that we found earlier
  for (const room of roomsToDelete) {
    delete rooms[room.id];
  }
};

const deleteRoom = (room) => {
  joinedRooms[room.id] = false;
  challengeRooms[room.id] = false;

  for (const id in rooms) {
    if (room.id == id) {
      delete rooms[id];
      console.log("deleted");
    }
  }
};

const deleteChallengeRoomDB = (room) => {
  const connection = connectDB();
  const db = makeDb(connection);

  db.query("DELETE FROM chess_challenges WHERE roomId = '" + room.id + "'");
};

const getChallenge = async (user) => {
  var res = [];

  const connection = connectDB();
  const db = makeDb(connection);

  const rows = await db.query(
    "SELECT fromUserId, toUserId, status, roomId FROM chess_challenges WHERE fromUserId = '" +
    user.id +
    "' OR toUserId = '" +
    user.id +
    "' ORDER BY timestamp DESC"
  );
  let hasNotifications = 0;
  let flag = {};

  flag[user.id] = true;

  // console.log("Socket : ");
  // console.log(sockets);

  for (let row of rows) {
    //rows.map((row) => {

    console.log("row : ");
    console.log(row);

    let fromName = user.name;
    let toName = user.name;
    let userRow = [];

    flag[row.fromUserId] = true;
    flag[row.toUserId] = true;

    if (row.fromUserId == user.id) {
      userRow = await db.query(
        "SELECT username FROM w_users WHERE id = '" + row.toUserId + "'"
      );

      if (userRow.length == 0) {
        continue;
      }

      toName = userRow[0].username;
      // if the other player was accepted
      if (row.status == 1) {
        if (!sockets.hasOwnProperty(row.toUserId)) continue;

        hasNotifications = 1;
      } else {
      }
    } else {
      // if (!sockets.hasOwnProperty(row.fromUserId)) continue;

      userRow = await db.query(
        "SELECT username FROM w_users WHERE id = '" + row.fromUserId + "'"
      );
      if (userRow.length == 0) {
        continue;
      }
      fromName = userRow[0].username;
      // if a new challenge was received
      if (row.status == 0) {
        hasNotifications = 1;
      }

      console.log("*******notifications : " + hasNotifications);
    }

    res.push({
      fromUserId: row.fromUserId,
      fromUserName: fromName,
      toUserId: row.toUserId,
      toUserName: toName,
      status: row.status,
      roomId: row.roomId,
    });
  }

  for (const user of users) {
    if (flag.hasOwnProperty(user.id) && flag[user.id]) {
      continue;
    }

    res.push({
      fromUserId: "",
      fromUserName: "",
      toUserId: user.id,
      toUserName: user.name,
      status: -1,
      roomId: "",
    });
  }
  //});

  return { notification: hasNotifications, challenges: res };
};
const checkScore = (room, sendMessage = false) => {
  let winner = null;
  for (const client of room.sockets) {
    if (client.score >= NUM_ROUNDS) {
      winner = client;
      break;
    }
  }

  if (winner) {
    if (sendMessage) {
      for (const client of room.sockets) {
        client.emit(
          "gameOver",
          client.id === winner.id ? "You won the game!" : "You lost the game :("
        );
      }
    }

    return true;
  }

  return false;
};

const beginRound = (socket, id) => {
  // This is a hack to make sure this function is only being called once during
  // game play. Basically, the client needs to send us the
  if (id && socket.id !== id) {
    return;
  }

  // Get the room
  const room = rooms[socket.roomId];
  if (!room) {
    return;
  }

  // Make sure to cancel the 20 second lose round timer so we make sure we only
  // have one timer going at any point.
  if (room.timeout) {
    clearTimeout(room.timeout);
  }

  // If we've already found a game winner, we don't need to start a new round.
  if (checkScore(room)) {
    return;
  }

  // the different potential spawning positions on the game map. measured in meters.
  let positions = [
    { x: 8, y: 8 },
    { x: 120, y: 8 },
    { x: 120, y: 120 },
    { x: 8, y: 120 },
  ];
  // Shuffle each position... we're going to use some clever trickery to
  // determine where each player should be spawned. Using lodash for the the shuffle
  // functionality.
  positions = _.shuffle(positions);

  // isIt will represent the new socket that will be considered to be "IT"
  let isIt = null;
  // This is going to be a dictionary that we're going to send to every client.
  // the keys will represent the socket ID and the values will be another dictionary
  // that will represent each player.

  const output = {};

  // We're going to loop through each player in the room.
  for (const client of room.sockets) {
    // here is the trickery. We're just going to get the last object in the positions
    // array to get the position for this player. Now there will be one less choice in
    // in the positions array.
    const position = positions.pop();
    client.x = position.x;
    client.y = position.y;
    // if the player was already it, we don't want to make them it again.
    if (client.isIt) {
      // the player won the round! increment their score.
      client.score = id ? client.score + 1 : client.score;
      client.isIt = false;
    }
    // we're going to use lodash's handy isEmpty check to see if we have an IT socket already.
    // if we don't mark the current player as it! mark the as not it just in case.
    else if (_.isEmpty(isIt)) {
      client.isIt = true;
      isIt = client;
    } else {
      client.isIt = false;
    }

    // this is the sub dictionary that represents the current player.
    output[client.id] = {
      x: client.x,
      y: client.y,
      score: client.score,
      isIt: client.isIt,
    };
  }

  // After all that madness, check if we have a game winner! If we do, then
  // just return out.
  if (checkScore(room, true)) {
    return;
  }

  // Tell all the players to update themselves client side
  for (const client of room.sockets) {
    client.emit("checkifit", output);
  }

  // Start the round over if the player didn't catch anyone. They've lost the round
  // so decrement their score :(. Note that setTimeout is measured in milliseconds hence
  // the multipication by 1000
  room.timeout = setTimeout(() => {
    if (isIt) {
      isIt.score = isIt.score - 1;
    }
    beginRound(socket, null);
  }, 20 * 1000);
};

// Allow express to serve static files in folder structure set by Unity Build
//app.use("/TemplateData",express.static(__dirname + "/TemplateData"));
//app.use("/Release",express.static(__dirname + "/Release"));

// app.use(myConnection(mysql, {
// 	host: 'localhost',
// 	user: 'root',
// 	password: '',
// 	port: 3306,
// 	database: 'annancsh_mygame'

// }));

//app.use(express.static('public'));

app.use(cors());
app.use(bodyParser.json());
app.use(cookieParser());
app.use(bodyParser.urlencoded({ extended: true }));

// app.options("/*", function(req, res, next){
//   res.header('Access-Control-Allow-Origin', '*');
//   res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE,OPTIONS');
//   res.header('Access-Control-Allow-Headers', 'Content-Type, Authorization, Content-Length, X-Requested-With');
//   res.send(200);
// });

app.use("/api", routes);
//app.use(express.urlencoded({extended: false}));

// app.use(express.static(__dirname));
// app.use("/TemplateData",express.static(__dirname + "/TemplateData"));
// app.use("/Build",express.static(__dirname + "/Build"));
// app.get('/',function(req, res)
//         {
//             res.sendFile(__dirname + '/index.html');
//         });

app.get("/", function (req, res) {
  res.send("Welcome to you!");
});



// Redirect response to serve index.html

// Implement socket functionality
gameSocket = io.on("connection", function (socket) {
  socket.on("ready", () => {
    console.log(socket.id, "is ready!");
    const room = rooms[socket.roomId];
    // when we have two players... START THE GAME!
    if (room.sockets.length == 2) {
      // tell each player to start the game.
      for (const client of room.sockets) {
        client.emit("initGame");
      }
    }
  });

  socket.on("deleteRoom", (room) => {
    console.log(rooms);

    joinedRooms[room.id] = false;

    for (const id in rooms) {
      if (room.id == id) {
        delete rooms[id];
        console.log("deleted");
      }
    }
    deleteRoom(room);

    // if the room is a challenge room
    deleteChallengeRoomDB(room);

    socket.broadcast.emit("show room", { rooms: getRooms() });
  });

  socket.on("give up", (room) => {
    for (var sock of rooms[room.id].sockets) {
      //room.name : turn for WHITE and BLACK
      sock.emit("gave up", { turn: room.name });
    }
  });

  socket.on("get room list", (user) => {
    // Emit room
    console.log("Get Room List : ");
    console.log(user);

    // sockets[user.id] = socket;

    // const found = users.some((el) => el.id == user.id);
    // if (!found) users.push({ id: user.id, name: user.name, score: user.score });

    socket.emit("show room", { rooms: getRooms() });

    socket.broadcast.emit("got challenge notifications", { notification: 1 });
  });

  socket.on("get user list", (user) => {
    // Emit room
    console.log("Get User List : " + user);

    socket.emit("show users", { users: users });
  });

  socket.on("startGame", (data, callback) => {
    const room = rooms[socket.roomId];
    if (!room) {
      return;
    }
    const others = [];
    for (const client of room.sockets) {
      client.x = 0;
      client.y = 0;
      client.score = 0;
      if (client === socket) {
        continue;
      }
      others.push({
        id: client.id,
        x: client.x,
        y: client.y,
        score: client.score,
        isIt: false,
      });
    }

    // Tell the client who they are and who everyone else is!
    const ack = {
      me: {
        id: socket.id,
        x: socket.x,
        y: socket.y,
        score: socket.score,
        isIt: false,
      },
      others,
    };

    callback(ack);

    // Start the game in 5 seconds
    setTimeout(() => {
      beginRound(socket, null);
    }, 5000);
  });


  //Made changes here********
  socket.on("createRoom", (roomName) => {
    console.log("create Room");
    const room = {
   //   id: guid(), // generate a unique id for the new room, that way we don't need to deal with duplicates.
      id: roomName.name, // generate a unique id for the new room, that way we don't need to deal with duplicates.
      name: roomName.name,
      sockets: [],
    };
    rooms[room.id] = room;
    console.log(rooms);
    // have the socket join the room they've just created.
   // joinRoom(socket, room);
    socket.emit("createdRoom", room);

    socket.broadcast.emit("show room", { rooms: getRooms() });
  });

  socket.on("joinRoom", (r) => {
    console.log("join room", r);
    if (!rooms.hasOwnProperty(r.id)) {
      deleteChallengeRoomDB(r);
      return;
    }
    const room = rooms[r.id];
    joinRoom(socket, room);
  });

  socket.on("invite a challenge", (req) => {
    console.log("invite a challenge : ");
    const connection = connectDB();
    const db = makeDb(connection);

    db.query(
      "INSERT INTO chess_challenges (fromUserId, toUserId) VALUES ('" +
      req.users[0].id +
      "', " +
      "'" +
      req.users[1].id +
      "')"
    );

    //sockets[users[1].id].emit('received a challenge', {user: users[0]});
    if (sockets.hasOwnProperty(req.users[1].id)) {
      sockets[req.users[1].id].emit("got challenge notifications", {
        notification: 1,
      });
    }

    getChallenge(req.users[0]).then((res) => {
      socket.emit("show challenges", { challenges: res.challenges });
    });
  });

  // Accept the challenge
  socket.on("createChallenge", (req) => {
    console.log("create a challenge room");
    const room = {
      id: guid(), // generate a unique id for the new room, that way we don't need to deal with duplicates.
      name: "challenge room",
      sockets: [],
    };
    rooms[room.id] = room;
    challengeRooms[room.id] = true;

    const connection = connectDB();
    const db = makeDb(connection);

    db.query(
      "UPDATE chess_challenges SET status = '1', roomId = '" +
      room.id +
      "' WHERE fromUserId = '" +
      req.users[0].id +
      "' AND toUserId = '" +
      req.users[1].id +
      "'"
    );
    // have the socket join the room they've just created.
    // joinRoom(socket, room);
    socket.emit("createdRoom", room);

    if (sockets.hasOwnProperty(req.users[0].id)) {
      sockets[req.users[0].id].emit("got challenge notifications", {
        notification: 1,
      });
    }
  });

  // Start the challenge
  socket.on("startChallenge", (r) => {
    console.log("Start the challenge");

    deleteChallengeRoomDB(r);

    if (!rooms.hasOwnProperty(r.id)) {
      return;
    }
    const room = rooms[r.id];
    joinRoom(socket, room);
  });

  // Exit the challenge
  socket.on("exitChallenge", (r) => {
    console.log("Exit the challenge");

    deleteChallengeRoomDB(r);
    deleteRoom(r);
  });

  // Get challenges
  socket.on("get challenges", (user) => {
    console.log("Get challenges");

    getChallenge(user).then((res) => {
      console.log(res);

      socket.emit("got challenge notifications", {
        notification: res.notification,
      });

      socket.emit("show challenges", { challenges: res.challenges });
    });
  });

  socket.on("leaveRoom", () => {
    leaveRooms(socket);
  });

  socket.on("disconnect", () => {
    console.log("user disconnected");
    leaveRooms(socket);
    for (var id in sockets) {
      if (sockets.hasOwnProperty(id) && sockets[id] == socket) {
        delete sockets[id];
      }
    }
  });

  var currentPlayer = {};
  currentPlayer.name = "unknown";

  console.log("socket connected: " + socket.id);

  socket.emit("connected", {});

  socket.on("disconnect", function () {
    console.log("socket disconnected: " + socket.id);
  });

  /////////////////////////////////

  //sockets.push(socket)

  socket.on("click", function (data) {
    console.log(
      "clicked   == " + data.index + (data.turn == 0 ? "White" : "Black")
    );
    rooms[data.roomId].sockets.map((s, index) => {
      if (socket !== s) {
        s.emit("other player turned", data);
      }
    });
    // sockets.map( (s) => {
    // 	if(socket !== s)
    // 	{
    // 		s.emit('other player turned', data);
    // 		console.log(' === connected === ');
    // 	}

    // });
    //socket.broadcast.emit('other player turned', data);
  });

  ////////////////////////////////////////////////////

  socket.on("player linked", function () {
    console.log(" recv: player linked");
  });

  socket.on("player connect", function () {
    console.log(currentPlayer.name + " recv: player connect");
    for (var i = 0; i < clients.length; i++) {
      var playerConnected = {
        name: clients[i].name,
        position: clients[i].position,
        rotation: clients[i].position,
        health: clients[i].health,
      };
      //	in your current game, we need to tell you about the other players.
      socket.emit("other player connected", playerConnected);
      console.log(
        currentPlayer.name +
        " emit: other player connected: " +
        JSON.stringify(playerConnected)
      );
    }
  });

  socket.on("play", function (data) {
    console.log(currentPlayer.name + " recv: play: " + JSON.stringify(data));
    // if this is the first person to join the game init the enemies
    if (clients.length === 0) {
      numberOfEnemies = data.enemySpawnPoints.length;
      enemies = [];
      data.enemySpawnPoints.forEach(function (enemySpawnPoint) {
        var enemy = {
          name: guid(),
          position: enemySpawnPoint.position,
          rotation: enemySpawnPoint.rotation,
          health: 100,
        };
        enemies.push(enemy);
      });
      playerSpawnPoints = [];
      data.playerSpawnPoints.forEach(function (_playerSpawnPoint) {
        var playerSpawnPoint = {
          position: _playerSpawnPoint.position,
          rotation: _playerSpawnPoint.rotation,
        };
        playerSpawnPoints.push(playerSpawnPoint);
      });
    }

    var enemiesResponse = {
      enemies: enemies,
    };
    // we always will send the enemies when the player joins
    console.log(
      currentPlayer.name + " emit: enemies: " + JSON.stringify(enemiesResponse)
    );
    socket.emit("enemies", enemiesResponse);
    var randomSpawnPoint =
      playerSpawnPoints[Math.floor(Math.random() * playerSpawnPoints.length)];
    currentPlayer = {
      name: data.name,
      position: randomSpawnPoint.position,
      rotation: randomSpawnPoint.rotation,
      health: 100,
    };
    clients.push(currentPlayer);
    // in your current game, tell you that you have joined
    console.log(
      currentPlayer.name + " emit: play: " + JSON.stringify(currentPlayer)
    );
    socket.emit("play", currentPlayer);
    // in your current game, we need to tell the other players about you.
    socket.broadcast.emit("other player connected", currentPlayer);
  });

  socket.on("player move", function (data) {
    console.log("recv: move: " + JSON.stringify(data));
    currentPlayer.position = data.position;
    socket.broadcast.emit("player move", currentPlayer);
  });

  socket.on("player turn", function (data) {
    console.log("recv: turn: " + JSON.stringify(data));
    currentPlayer.rotation = data.rotation;
    socket.broadcast.emit("player turn", currentPlayer);
  });

  socket.on("player shoot", function () {
    console.log(currentPlayer.name + " recv: shoot");
    var data = {
      name: currentPlayer.name,
    };
    console.log(currentPlayer.name + " bcst: shoot: " + JSON.stringify(data));
    socket.emit("player shoot", data);
    socket.broadcast.emit("player shoot", data);
  });

  socket.on("health", function (data) {
    console.log(currentPlayer.name + " recv: health: " + JSON.stringify(data));
    // only change the health once, we can do this by checking the originating player
    if (data.from === currentPlayer.name) {
      var indexDamaged = 0;
      if (!data.isEnemy) {
        clients = clients.map(function (client, index) {
          if (client.name === data.name) {
            indexDamaged = index;
            client.health -= data.healthChange;
          }
          return client;
        });
      } else {
        enemies = enemies.map(function (enemy, index) {
          if (enemy.name === data.name) {
            indexDamaged = index;
            enemy.health -= data.healthChange;
          }
          return enemy;
        });
      }

      var response = {
        name: !data.isEnemy
          ? clients[indexDamaged].name
          : enemies[indexDamaged].name,
        health: !data.isEnemy
          ? clients[indexDamaged].health
          : enemies[indexDamaged].health,
      };
      console.log(
        currentPlayer.name + " bcst: health: " + JSON.stringify(response)
      );
      socket.emit("health", response);
      socket.broadcast.emit("health", response);
    }
  });

  socket.on("disconnect", function () {
    console.log(currentPlayer.name + " recv: disconnect " + currentPlayer.name);
    socket.broadcast.emit("other player disconnected", currentPlayer);
    console.log(
      currentPlayer.name +
      " bcst: other player disconnected " +
      JSON.stringify(currentPlayer)
    );
    for (var i = 0; i < clients.length; i++) {
      if (clients[i].name === currentPlayer.name) {
        clients.splice(i, 1);
      }
    }
  });
});

function guid() {
  function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
  }
  return (
    s4() +
    s4() +
    "-" +
    s4() +
    "-" +
    s4() +
    "-" +
    s4() +
    "-" +
    s4() +
    s4() +
    s4()
  );
}
