document.addEventListener('DOMContentLoaded', () => {
    const authButton = document.getElementById("auth-button");
    if (authButton) authButton.addEventListener('click', authClick);
    else console.error("auth-button not found");

    const logOutButton = document.getElementById("log-out-button");
    if (logOutButton) logOutButton.addEventListener('click', logOutClick);

    const profileEditButton = document.getElementById("profile-edit");
    if (profileEditButton) profileEditButton.addEventListener('click', profileEditClick);
});

function authClick() {
    const emailInput = document.querySelector('[name="auth-user-email"]');
    if (!emailInput) throw '[name="auth-user-email"] not found';

    const passwordInput = document.querySelector('[name="auth-user-password"]');
    if (!passwordInput) throw '[name="auth-user-password"] not found';

    const errorDiv = document.getElementById("auth-error");
    if (!errorDiv) throw 'auth-error not found';

    errorDiv.show = err => {
        errorDiv.style.visibility = "visible";
        errorDiv.innerText = err;
    }
    errorDiv.hide = () => {
        errorDiv.style.visibility = "hidden";
        errorDiv.innerText = "";
    }

    const email = emailInput.value.trim();
    const password = passwordInput.value;

    if (email.length === 0) {
        errorDiv.show("Заповніть Email");
        return;
    }
    if (password.length === 0) {
        errorDiv.show("Заповніть пароль");
        return;
    }
    errorDiv.hide();

    console.log(email, password);

    fetch(`/api/auth?input=${email}&password=${password}`, {
        method: 'GET'
    }).then(r => r.json()).then(j => {
        console.log(j);
        if (j.code != 200) {
            errorDiv.show("Відмова. Перевірьте введені дані");
        } else {
            window.location.reload();
        }
    });
}
function logOutClick() {
    fetch('/api/auth', {
        method: 'DELETE'
    }).then(r => location.reload())
}

function profileEditClick(e) {
    const btn = e.target;
    if (btn.classList.contains('bi-check2-square')) {
        btn.classList.remove('bi-check2-square');
        btn.classList.add('bi-pencil-square');
    } else {
        btn.classList.add('bi-check2-square');
        btn.classList.remove('bi-pencil-square');
    }
    for (let elem of document.querySelectorAll('[profile-editable]')) {
        if (elem.hasAttribute('contenteditable')) {
            elem.removeAttribute('contenteditable')
        } else {
            elem.setAttribute('contenteditable', 'true')
        }
    }
    
}