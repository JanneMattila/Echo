﻿console.log(`WebApp.Root: ${WebApp.Root}`);

let selectedElement = null;

function addMessage(msg) {
    console.log(msg);

    const template = document.getElementById("block");

    const block = template.content.cloneNode(true);

    block.querySelectorAll("text")[0].textContent = msg;

    const echoElement = document.getElementById("echo");
    echoElement.insertBefore(block, echoElement.firstChild);
}

let protocol = new signalR.JsonHubProtocol();
let hubRoute = `${WebApp.Root}Echo`;
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
    try {
        let data = "Date received: " + new Date().toLocaleTimeString();
        data += "\n" + msg.request + "\n";
        for (const header in msg.headers) {
            data += header + ": " + msg.headers[header] + "\n";
        }
        data += "\n" + msg.body;
        addMessage(data);
    } catch (e) {
        addMessage(JSON.stringify(e));
        addMessage(JSON.stringify(msg));
    }

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

function copyToClipboard(clickedElement, elementName) {
    let endpoint = document.getElementById(elementName);
    console.log(endpoint.innerText);

    let input = document.createElement("textarea");
    input.innerHTML = endpoint.innerHTML;
    document.body.appendChild(input);
    input.select();
    try {
        document.execCommand("copy");
    } catch (e) {
        console.error(e);
    }
    document.body.removeChild(input);

    clickedElement.innerHTML = "📝";
    setTimeout(function () {
        clickedElement.innerHTML = "📋";
    }, 300);
}

function selectElement(element) {
    selectedElement = element;
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

function clearLog(clickedElement) {
    console.log('clearing log');
    document.getElementById("echo").innerHTML = '';
    updateTitle();

    clickedElement.innerHTML = "⟲";
    setTimeout(function () {
        clickedElement.innerHTML = "⟳";
    }, 300);
}

document.addEventListener('keyup', e => {
    if (e.key === "Escape") {
        hideHelp();
    }
    else if (e.key === "Backspace" || e.key === "Delete") {
        if (selectedElement != null) {
            try {
                selectedElement.remove();

            } catch (err) {
                console.log(err);
            }
        }
        selectedElement = null;
    }
});
