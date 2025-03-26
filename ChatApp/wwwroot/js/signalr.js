var friendConnection = new signalR.HubConnectionBuilder()
    .withUrl("/friendHub")
    .build();

var notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

notificationConnection.on("NotifyFriendRequest", function (pendingRequestsCount) {
    const counterElement = document.getElementById("friendRequestCounter");
    if (counterElement) {
        counterElement.style.display = "inline";
        counterElement.innerText = pendingRequestsCount;
    }
});

friendConnection.on("FriendOnline", function (userId, firstName, lastName) {
    const activeFriendsList = document.getElementById("activeFriendsList");

    if (!document.getElementById(`friend-${userId}`)) {
        const friendElement = document.createElement("li");
        friendElement.id = `friend-${userId}`;
        friendElement.innerHTML = `
                    <div class="friend-container">
                        <div class="friend-info">
                            <span>${firstName} ${lastName}</span>
                            <div class="online-status">
                                <div class="online-dot"></div>
                                <span>Online</span>
                            </div>
                        </div>
                        <button class="message-btn" onclick="window.location.href='/Chat/Chat?friendId=${userId}'">
                            <img src="/images/message-icon.png" alt="Message" class="icon-image"/>
                        </button>
                    </div>
                `;
        activeFriendsList.appendChild(friendElement);
    }
});

friendConnection.on("FriendOffline", function (userId) {
    const friendElement = document.getElementById(`friend-${userId}`);
    if (friendElement) {
        friendElement.remove();
    }
});

Promise.all([
    friendConnection.start(),
    notificationConnection.start()
]).then(function () {
    console.log("SignalR connections established.");
}).catch(function (err) {
    console.error("Error establishing SignalR connections:", err.toString());
});