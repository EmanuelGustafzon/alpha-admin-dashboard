document.addEventListener("DOMContentLoaded", () => {

    if (!getCookie('cookieConsent'))
        showCookieModal();
})
function showCookieModal() {
    const modal = document.getElementById("cookie-consent")
    if (modal) modal.classList.remove("d-none")

    const consentValue = getCookie('cookieConsent')
    if (!consentValue) return

    try {
        const consent = JSON.parse(consentValue)
        console.log(consent)
        document.getElementById("cookie-functional").checked = consent.functional
    } catch (error) {
        console.error(error)
    }
}
function hideCookieModal() {
    const modal = document.getElementById("cookie-consent")
    if (modal) modal.classList.add("d-none")
}
function getCookie(name) {
    const nameEQ = name + "="
    const cookies = document.cookie.split(";")
    for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.indexOf(nameEQ) === 0) {
            return decodeURIComponent(cookie.substring(nameEQ.length))
        }
    }
    return null;
}
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date()
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000)
        expires = `; expires=${date.toUTCString()}`
    }
    const encodedValue = encodeURIComponent(value || "")
    document.cookie = `${name}=${encodedValue}${expires}; path=/; SameSite=Lax`
}
async function acceptAllCookies() {
    const consent = {
        essential: true,
        functional: true
    }
    setCookie("cookieConsent", JSON.stringify(consent), 90)
    await handleConsent(consent)
    hideCookieModal()
}
async function acceptSelectedCookies() {
    const form = document.getElementById("cookie-consent-form")
    const formData = new FormData(form)
    const consent = {
        essential: true,
        functional: formData.get("functional") === "on"
    }
    setCookie("cookieConsent", JSON.stringify(consent), 90)
    await handleConsent(consent)
    hideCookieModal()
}
async function handleConsent(consent) {
    try {
        const res = await fetch('/cookies/SetCookieConsent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(consent)
        })
        if (!res.ok) {
            console.error("failed setting cookie")
        }
    } catch (error) {
        console.error(error)
    }
}

async function setFunctionalCookie(name, value, days) {
    try {
        const res = await fetch(`/cookies/setFunctionalCookie?name=${name}&value=${value}&days=${days}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
        })
        if (!res.ok) {
            console.error("failed setting cookie")
        }
    } catch (error) {
        console.error(`Failed to set functional cookie :: ${error}`)
    }
}

async function removeCookie(name) {
    try {
        const res = await fetch(`/cookies/deleteCookie?name=${name}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
        })
        if (!res.ok) {
            console.error("failed deleting cookie")
        }
    } catch (error) {
        console.error(`Failed to delete Cookie :: ${error}`)
    }
}

// Theme cookie
const theme = getCookie('ThemeCookie');
document.body.dataset.theme = theme;

const themebox = document.getElementById("theme-checkbox");
if (theme === 'dark') {
    themebox.checked = true;
}

const themeCheckbox = document.getElementById("theme-checkbox");
if (themeCheckbox) {
    themeCheckbox.addEventListener("change", async () => {
        if (themeCheckbox.checked) {
            document.body.dataset.theme = "dark"
            setFunctionalCookie("ThemeCookie", "dark", 90)
        } else {
            document.body.dataset.theme = "light"
            setFunctionalCookie("ThemeCookie", "light", 90)
        }
    })
}
