const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

connection.on("generalNotifications", (notification) => {
    updateNotificationList(notification)
})

connection.on("adminNotifications", (notification) => {
    updateNotificationList(notification)
});

connection.start().catch(error => console.log(error));

function updateNotificationList(notification) {
    const notificationsList = document.querySelector('#notifications');
    const item = notificationItem(notification.id, notification.icon, notification.message, notification.created, notification.action)
    notificationsList.insertAdjacentHTML('afterbegin', item)
    incramentNotificationCount()
    hasNotificatons(true)
}
function hasNotificatons(memberHasNotification) {
    const redCircle = document.querySelector('#has-notifications');
    if (memberHasNotification) {
        redCircle.classList.add("new-notification")
    } else {
        redCircle.classList.remove("new-notification")
    }
}

let notificationCount = 0;

function setNotificationCountToZero() {
    let count = document.querySelector('#notification-count');
    count.innerText = "0";
    notificationCount = 0;
}

function incramentNotificationCount() {
    let count = document.querySelector('#notification-count');
    notificationCount++;
    count.innerText = notificationCount;
}

function decramentNotificationCount() {
    let count = document.querySelector('#notification-count');
    notificationCount--;
    count.innerText = notificationCount;
    if (notificationCount === 0) hasNotificatons(false);
}

async function getNotifications() {
    const data = await fetchData("/api/notifications")
    setNotificationCountToZero()
    if (data.length != 0) hasNotificatons(true)
    data.forEach(notification => {
        updateNotificationList(notification)
    })
}
async function dismissNotification(id) {
    const result = await sendDataAsQuery(`/api/notifications/dissmiss/${id}`, false);
    if (result === false) return;
    decramentNotificationCount()
    removeNotification(id)
}
function removeNotification(id) {
    const item = document.querySelector(`[data-id="${id}"]`)
    item.remove()
}
function notificationItem(id, image, message, createdAt, action = null) {
    return `
            <div class="notification" data-id="${id}">
                <img class="profile-img" src="${image}"/>
                <div>
                    <p class="message">${message}</p>
                    ${action ? `<a href="${action}">Click here!</a>` : ''}
                    <p class="time" data-created="${new Date(createdAt).toISOString()}"> ${timeAgo(createdAt)} </p>
                </div>
                <button onclick="dismissNotification('${id}')" class="btn btn-clean">X</button>
            </div>
        `;
}
window.addEventListener('DOMContentLoaded', () => {
    getNotifications();
})