var friendConnection = new signalR.HubConnectionBuilder()
    .withUrl("/friendHub")
    .build();

var notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

notificationConnection.on("NotifyFriendRequest", function (pendingRequestsCount, notification, notificationCount) {
    const friendRequestCounterElement = document.getElementById("friendRequestCounter");
    if (friendRequestCounterElement) {
        friendRequestCounterElement.style.display = "inline";
        friendRequestCounterElement.innerText = pendingRequestsCount;
    }

    const notificationCounterElement = document.getElementById("notificationCounter");
    if (notificationCounterElement) {
        notificationCounterElement.style.display = "inline";
        notificationCounterElement.innerText = notificationCount;
    }
    const notificationsList = document.getElementById("notificationsList");
    const noNotificationsMessage = document.getElementById("noNotificationsMessage");

    if (notificationsList) {
        const notificationElement = document.createElement("li");
        notificationElement.className = "list-group-item d-flex justify-content-between align-items-center";
        notificationElement.id = `notification-${notification.id}`;
        notificationElement.innerHTML = `
            <div>
                <span>${notification.content}</span>
                <small class="text-muted d-block">${new Date(notification.date).toLocaleString()}</small>
            </div>
        `;

        notificationsList.appendChild(notificationElement);
        if (noNotificationsMessage) {
            noNotificationsMessage.style.display = "none";
        }
    }
});

friendConnection.on("FriendOnline", function (userId, firstName, lastName) {
    const activeFriendsList = document.getElementById("activeFriendsList");

    if (!document.getElementById(`active-friend-${userId}`)) {
        const friendElement = document.createElement("li");
        friendElement.id = `active-friend-${userId}`;
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
    const friendElement = document.getElementById(`active-friend-${userId}`);
    if (friendElement) {
        friendElement.remove();
    }
});

Promise.all([
    friendConnection.start(),
    notificationConnection.start()
]).then(function () {

}).catch(function (err) {
    console.error(err.toString());
});