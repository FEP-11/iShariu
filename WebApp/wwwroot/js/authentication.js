const submitBtn = document.getElementById("registerBtn");

submitBtn.addEventListener("click", async e => {
    e.preventDefault()

    const email = document.getElementById("emailForm").value
    const username = document.getElementById("usernameForm").value
    const password = document.getElementById("passwordForm").value
    const confirmPassword = document.getElementById("confirmPasswordForm").value

    await addUser(email, username, password, confirmPassword)
})

// The function of adding user to db
async function addUser(email, username, password, confirmPassword) {
    if (password === confirmPassword && !(await IsExist(username))) {
        const response = await fetch("/api/user", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                username: username,
                email: email,
                password: password,
                role: "user"
            })
        })

        if (response.ok) {
            await LogIn(username, password)
        }
    } else {
        console.error("Password isn't match with Confirm Password")
    }
}

// Authorization function in the account
async function LogIn(username, password) {
    const userForm = new FormData()
    userForm.append("username", username)
    userForm.append("password", password)
    const response = await fetch("/account/signin", {
        method: "POST",
        body: userForm,
        
    })

    if (response.ok) {
        console.log("User logged in successfully.");
    } else {
        console.error(response.error);
    }
}

// The function for finding existing user
async function IsExist(email, username) {
    const response = await fetch("/api/user")
    
    if (response.ok) {
        const users = await response.json()
        return users.some(user => user.username === username || user.email === email)

        return false
    }
}