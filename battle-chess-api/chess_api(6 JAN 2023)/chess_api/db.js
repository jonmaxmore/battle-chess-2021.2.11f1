var mysql = require('mysql');

exports.connectDB = function() {
    // var db = mysql.createConnection({
    //     host: "sql271.main-hosting.eu",
    //     user: "u649647356_chess",
    //     password: "+l1;@1oD2HG",
    //     database: "u649647356_chess"
    // });
    var db = mysql.createConnection({
        host: "localhost",
        user: "root",
        password: "battleLab@!050322",
        database: "chess"
    });
    
    db.connect(function(err) {
        if (err) {
            console.log("Error: DB Connection.");
            console.log(err);
        } else {
            console.log("Success: DB Connection!");
        }
    });
    
    db.on('error', function onError(err) {

    });

    return db;
};

exports.closeDB = function(db) {
    //db.end();
};
