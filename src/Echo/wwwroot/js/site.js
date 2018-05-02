let echoElement = document.getElementById("echo");
function addMessage(msg) {
    console.log(msg);
    let element = document.createElement('pre');
    element.innerText = msg;
    echoElement.insertBefore(element, echoElement.firstChild);
}

let logger = new signalR.ConsoleLogger(signalR.LogLevel.Information);
let transportType = signalR.TransportType.WebSockets;
let protocol = new signalR.JsonHubProtocol();
let hubRoute = "Echo";
let connection = new signalR.HubConnection(hubRoute, { transport: transportType, logger: logger, protocol: protocol });

connection.on('echo', function (msg) {
    let data = "Date received: " + new Date().toLocaleTimeString();
    data += "\n" + msg.request;
    data += "\n\n" + msg.headers;
    data += "\n" + msg.body;
    addMessage(data);
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