let echoElement = document.getElementById("echo");
function addMessage(msg) {
    console.log(msg);
    let element = document.createElement('pre');
    element.innerText = msg;
    echoElement.insertBefore(element, echoElement.firstChild);
}

let protocol = new signalR.JsonHubProtocol();
let hubRoute = "Echo";
let connection = new signalR.HubConnectionBuilder()
    .withUrl(hubRoute)
    .withAutomaticReconnect()
    .withHubProtocol(protocol)
    .build();

const originalTitle = document.title;

let isFocus = true;
let unreadMessages = 0;

function updateTitle() {
    document.title = originalTitle + (isFocus || unreadMessages === 0 ? "" : `: ${unreadMessages}`);
}

window.addEventListener('focus', () => {
    isFocus = true;
    unreadMessages = 0;
    updateTitle();
});

window.addEventListener('blur', () => {
    isFocus = false;
});

connection.on('echo', function (msg) {
    let data = "Date received: " + new Date().toLocaleTimeString();
    data += "\n" + msg.request;
    data += "\n\n" + msg.headers;
    data += "\n" + msg.body;
    addMessage(data);

    if (!isFocus) {
        unreadMessages++;
    }

    updateTitle();
});

connection.onclose(function (e) {
    if (e) {
        addMessage("Connection closed with error: " + e);
    }
    else {
        addMessage("Disconnected");
    }
});

connection.start()
    .then(function () {
        // Connected
    })
    .catch(function (err) {
        addMessage(err);
    });

function copyToClipboard(elementName) {
    var endpoint = document.getElementById(elementName);
    console.log(endpoint.innerText);

    navigator.clipboard.writeText(endpoint.innerText);
}

function showHelp() {
    document.getElementById('helpOpen').style.display = 'none';
    document.getElementById('helpClose').style.display = '';
    document.getElementById('help').style.display = '';
}

function hideHelp() {
    document.getElementById('helpOpen').style.display = '';
    document.getElementById('helpClose').style.display = 'none';
    document.getElementById('help').style.display = 'none';
}

function clearLog() {
    console.log('clearing log');
    echoElement.innerHTML = '';
    updateTitle();
}

document.addEventListener('keyup', (event) => {
    if (event.keyCode === 27 /* Esc */) {
        hideHelp();
    }
});
