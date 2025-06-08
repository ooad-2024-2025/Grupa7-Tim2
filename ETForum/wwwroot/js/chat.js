"use strict";

document.addEventListener('DOMContentLoaded', function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/livechat").build();
    const sendButton = document.getElementById("sendButton");
    const messageInput = document.getElementById("messageInput");
    const messagesList = document.getElementById("messagesList");
    const messagesWrapper = document.querySelector(".messages-wrapper");

    sendButton.disabled = true;

    function addMessageToList(user, message, time) {
        const li = document.createElement("li");
        const displayTime = time ? new Date(time) : new Date();
        const hours = displayTime.getHours().toString().padStart(2, '0');
        const minutes = displayTime.getMinutes().toString().padStart(2, '0');
        li.textContent = `[${hours}:${minutes}] ${user}: ${message}`;
        messagesList.appendChild(li);
        messagesWrapper.scrollTop = messagesWrapper.scrollHeight;
    }

    connection.on("ReceiveMessage", function (user, message) {
        addMessageToList(user, message);
    });

    connection.start().then(function () {
        sendButton.disabled = false;

        connection.invoke("GetRecentMessages", 100).then(messages => {
            if (messages && messages.length > 0) {
                const fragment = document.createDocumentFragment();
                messages.forEach(msg => {
                    const li = document.createElement("li");
                    const displayTime = msg.vrijeme ? new Date(msg.vrijeme) : new Date();
                    const hours = displayTime.getHours().toString().padStart(2, '0');
                    const minutes = displayTime.getMinutes().toString().padStart(2, '0');
                    li.textContent = `[${hours}:${minutes}] ${msg.userName}: ${msg.poruka}`;
                    fragment.appendChild(li);
                });
                messagesList.appendChild(fragment);
                messagesWrapper.scrollTop = messagesWrapper.scrollHeight;
            }
        }).catch(err => {
            console.error("Greška pri dohvaćanju poruka:", err);
        });
    }).catch(function (err) {
        console.error("Greška pri uspostavljanju SignalR konekcije:", err.toString());
    });

    sendButton.addEventListener("click", function (event) {
        const message = messageInput.value.trim();
        if (message === "") return;

        connection.invoke("SendMessage", message).catch(function (err) {
            console.error(err.toString());
        });

        messageInput.value = "";
        messageInput.focus();
        event.preventDefault();
    });

    messageInput.addEventListener("keydown", function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            sendButton.click();
        }
    });
});
