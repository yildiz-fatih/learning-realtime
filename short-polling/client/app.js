let allMessages = [];
const baseUrl = "http://localhost:5292/api";
const INTERVAL = 3000; // default polling interval
let failedTries = 0; // counter for consecutive failed attempts
const BACKOFF = 5000; // the backoff multiplier for each failed attempt

// DOM elements
const messagesContainer = document.getElementById("messages");
const chatForm = document.getElementById("chat-form");
const usernameInput = document.getElementById("username-input");
const messageTextInput = document.getElementById("message-text-input");

// event listeners
chatForm.addEventListener("submit", function (event) {
    event.preventDefault();


    sendMessage(usernameInput.value, messageTextInput.value);

    messageTextInput.value = "";
});

async function getMessages() {
    try {
        const res = await fetch(`${baseUrl}/messages`);
        const json = await res.json();

        if (res.status >= 400) {
            throw new Error("Request failed with status " + res.status);
        }

        allMessages = json;
        renderMessages();

        failedTries = 0;

        setTimeout(getMessages, INTERVAL);

    } catch (error) {
        console.log("polling error: ", error);

        failedTries++;

        setTimeout(getMessages, BACKOFF * failedTries);
    }
}

async function sendMessage(username, text) {
    const options = {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ username: username, text: text })
    };

    await fetch(`${baseUrl}/messages`, options);
}

function renderMessages() {
    const html = allMessages.map(({ username, text }) =>
        `<li class="message"><span class="username">${username}</span>: ${text}</li>`
    );
    messagesContainer.innerHTML = html.join("\n");
}

// run on startup
getMessages();