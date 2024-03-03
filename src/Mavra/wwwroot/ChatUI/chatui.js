var joinButton = document.getElementById("joinButton");
var username = document.getElementById("username");
var sendMessage = document.getElementById("sendMessage");
var sendButton = document.getElementById("sendButton");
var stateLabel = document.getElementById("stateLabel");
var chatArea = document.getElementById("chat-area");
var chatContainer = document.getElementById("chatContainer");
var chatScrollArea = document.getElementById("scroll-chat-area");
var socket;

function updateState() {
    function disable() {
        sendMessage.disabled = true;
        sendButton.disabled = true;
    }
    function enable() {
        sendMessage.disabled = false;
        sendButton.disabled = false;
    }

    joinButton.disabled = true;

    if (!socket) {
        disable();
    }
    else {
        switch (socket.readyState) {
            case WebSocket.CLOSED:
                console.log("Disconnected!");
                disable();
                joinButton.disabled = false;
                break;
            case WebSocket.CLOSING:
                console.log("Disconnecting...");
                disable();
                break;
            case WebSocket.CONNECTING:
                console.log("Connecting...");
                disable();
                break;
            case WebSocket.OPEN:
                console.log("Connected!");
                enable();
                break;
            default:
                disable();
                break;
        }
    }
}

username.onchange = function () {
    username.classList.remove('is-invalid');
};

sendMessage.addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        event.preventDefault();
        sendButton.click();
    }
});

sendButton.onclick = function () {
    if (sendMessage.value.trim() == '') return;
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("socket not connected");
    }
    socket.send(sendMessage.value);
    let scrollToBottom = chatScrollArea.scrollHeight - chatScrollArea.scrollTop == chatScrollArea.clientHeight;
    chatArea.insertAdjacentHTML("beforeend", getOwnMessage(username.value, sendMessage.value));
    sendMessage.value = '';
    if (scrollToBottom)
        chatScrollArea.scrollTo(0, chatScrollArea.scrollHeight);
};

joinButton.onclick = function () {
    joinButton.disabled = true;
    joinButton.innerHTML = 'Connecting...';
    if (username.value.trim() == '') {
        username.classList.add('is-invalid');
        joinButton.disabled = false;
        joinButton.innerHTML = 'Join!';
        return;
    }
    socket = new WebSocket("ws://" + window.location.host + "/chat/" + username.value);
    socket.onopen = function (event) {
        updateState();
        username.classList.add('is-valid');
        console.log("Connected!");
        joinButton.innerHTML = 'Connected!';
        chatContainer.hidden = false;
        chatArea.insertAdjacentHTML("beforeend", getSystemMessage(`Connected!`));
    };
    socket.onclose = function (event) {
        updateState();
        chatArea.insertAdjacentHTML("beforeend", getSystemMessage(`Server closed! Reason:${event.reason}`));
    };
    socket.onerror = updateState;
    socket.onmessage = function (event) {
        console.log(event.data);
        let msgData = JSON.parse(event.data);
        let scrollToBottom= chatScrollArea.scrollHeight - chatScrollArea.scrollTop == chatScrollArea.clientHeight;
        chatArea.insertAdjacentHTML("beforeend",
            msgData.IsSystemMessage ? getSystemMessage(msgData.Message) : getMessage(msgData.Sender, msgData.Message));
        if (scrollToBottom)
            chatScrollArea.scrollTo(0, chatScrollArea.scrollHeight);
    };
};

function getMessage(sender, message,) {
    return `
                    <div class="row my-2">
                        <div class="col text-start">
                            <h6>${sender}</h6>
                            <small class="bg-white d-inline-block flex-wrap m-0 p-1 rounded-1" style="max-width:70%;">${message}</small>
                        </div>
                    </div>
                `;
}
function getOwnMessage(sender, message,) {
    return `
                    <div class="row my-2">
                        <div class="col text-end">
                            <h6>${sender}</h6>
                            <small class="bg-white d-inline-block flex-wrap m-0 p-1 rounded-1" style="max-width:70%;">${message}</small>
                        </div>
                    </div>
                `;
}

function getSystemMessage(message) {
    return '<div class="row my-1"><div class="col text-center"><em class="small text-muted">' + message + '</em></div></div>';
}