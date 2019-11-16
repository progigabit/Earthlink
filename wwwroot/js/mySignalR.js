/*
"use strict";



(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/myHub").build();
    connection.start().then(function () { console.log("connected"); });


    connection.on("ClientFunc", function (msg) {

        var encodedMsg = " says " + msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("mytext").innerHTML = msg;
        //document.getElementById("extractButton").appendChild(li);
        if (msg.includes("audio"))
            document.getElementById("mytext").style.color = "#d00";
    });

    connection.on("artistName", function (msg) {

        
       
    });



    document.getElementById("extractButton").addEventListener("click", function (event) {

        var message = document.getElementById("mytext").value;
        connection.invoke("artistName").catch (function (err) {
            return console.error(err.toString());
        connection.invoke("ServerFunc", "server").catch(function (err) {
            return console.error(err.toString());
        });

    });





})();