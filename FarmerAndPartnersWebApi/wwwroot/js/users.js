async function GetCompanies() {
    const response = await fetch("/api/companies", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const companies = await response.json();
        let companySelect = document.getElementById("company");
        companies.forEach(company => {
            addCompany(company, companySelect);
        });
    }
}

async function GetUsers() {
    const response = await fetch("/api/users", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const users = await response.json();
        let rows = document.querySelector("tbody");
        users.forEach(user => {
            rows.append(row(user));
        });
    }
}

async function GetUser(id) {
    const response = await fetch("/api/users/" + id, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        const form = document.forms["userForm"];
        form.elements["userId"].value = user.id;
        form.elements["timeStamp"].value = user.timeStamp;
        form.elements["name"].value = user.name;
        form.elements["login"].value = user.login;
        form.elements["password"].value = user.password;
        form.elements["company"].value = user.companyId;
    }
}

async function CreateUser(name, login, password, companyId) {
    const response = await fetch("/api/users", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: name,
            login: login,
            password: password,
            companyId: parseInt(companyId, 10)
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        reset();
        document.querySelector("tbody").append(row(user));
    }
    else {
        const errorData = await response.json();
        clearNameErrors();
        clearLoginErrors();
        clearPasswordErrors();
        clearCompanyErrors();
        if (errorData.errors) {
            if (errorData.errors["Name"]) {
                addNameError(errorData.errors["Name"]);
            }
            if (errorData.errors["Login"]) {
                addLoginError(errorData.errors["Login"]);
            }
            if (errorData.errors["Password"]) {
                addPasswordError(errorData.errors["Password"]);
            }
            if (errorData.errors["CompanyId"]) {
                addCompanyError(errorData.errors["CompanyId"]);
            }
        }
        if (errorData["Login"]) {
            addLoginError(errorData["Login"]);
        }
    }
}

async function EditUser(id, name, timeStamp, login, password, companyId) {
    const response = await fetch("/api/users", {
        method: "PUT",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            id: parseInt(id, 10),
            name: name,
            timeStamp: timeStamp,
            login: login,
            password: password,
            companyId: parseInt(companyId, 10)
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        reset();
        document.querySelector("tr[data-rowid='" + user.id + "']").replaceWith(row(user));
    }
    else {
        const errorData = await response.json();
        clearNameErrors();
        clearLoginErrors();
        clearPasswordErrors();
        clearCompanyErrors();
        if (errorData.errors) {
            if (errorData.errors["Name"]) {
                addNameError(errorData.errors["Name"]);
            }
            if (errorData.errors["Login"]) {
                addLoginError(errorData.errors["Login"]);
            }
            if (errorData.errors["Password"]) {
                addPasswordError(errorData.errors["Password"]);
            }
        }
        if (errorData["Login"]) {
            addLoginError(errorData["Login"]);
        }
    }
}

async function DeleteUser(id) {
    const response = await fetch("/api/users/" + id, {
        method: "DELETE",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        reset();
        document.querySelector("tr[data-rowid='" + user.id + "']").remove();
    }
}

function row(user) {

    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", user.id);

    const idTd = document.createElement("td");
    idTd.append(user.id);
    tr.append(idTd);

    const nameUserTd = document.createElement("td");
    nameUserTd.append(user.name);
    tr.append(nameUserTd);

    const loginTd = document.createElement("td");
    loginTd.append(user.login);
    tr.append(loginTd);

    const passwordTd = document.createElement("td");
    passwordTd.append(user.password);
    tr.append(passwordTd);

    const nameCompanyTd = document.createElement("td");
    nameCompanyTd.append(user.company.name);
    tr.append(nameCompanyTd);

    const linksTd = document.createElement("td");

    const editLink = document.createElement("a");
    editLink.setAttribute("data-id", user.id);
    editLink.setAttribute("style", "cursor:pointer;padding:15px;color:CornflowerBlue;");
    editLink.append("Изменить");
    editLink.addEventListener("click", e => {
        e.preventDefault();
        GetUser(user.id);
        clearNameErrors();
        clearLoginErrors();
        clearPasswordErrors();
        clearCompanyErrors();
    });
    linksTd.append(editLink);

    linksTd.append("|");

    const removeLink = document.createElement("a");
    removeLink.setAttribute("data-id", user.id);
    removeLink.setAttribute("style", "cursor:pointer;padding:15px;color:CornflowerBlue;");
    removeLink.append("Удалить");
    removeLink.addEventListener("click", e => {
        e.preventDefault();
        DeleteUser(user.id);
    });
    linksTd.append(removeLink);

    tr.appendChild(linksTd);

    return tr;
}

function addCompany(company, companySelect) {

    const option = new Option(company.name, company.id);
    companySelect.options[companySelect.options.length] = option;
}

function addNameError(errors) {
    document.getElementById("name").className = "m-2 form-control is-invalid";
    let content = document.getElementById("nameFeedback");
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        content.append(p);
    });
}

function addLoginError(errors) {
    document.getElementById("login").className = "m-2 form-control is-invalid";
    let content = document.getElementById("loginFeedback");
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        content.append(p);
    });
}

function addPasswordError(errors) {
    document.getElementById("password").className = "m-2 form-control is-invalid";
    let content = document.getElementById("passwordFeedback");
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        content.append(p);
    });
}

function addCompanyError(errors) {
    document.getElementById("company").className = "m-2 form-select is-invalid";
    let content = document.getElementById("companyFeedback");
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        content.append(p);
    });
}

function reset() {
    const form = document.forms["userForm"];
    form.elements["userId"].value = 0;
    form.elements["timeStamp"].value = "";
    form.elements["name"].value = "";
    form.elements["login"].value = "";
    form.elements["password"].value = "";
    form.elements["company"].value = document.getElementById("company").options[0].value;
    clearNameErrors();
    clearLoginErrors();
    clearPasswordErrors();
    clearCompanyErrors();
}

function clearNameErrors() {
    const nameErrors = document.getElementById("nameFeedback");
    if (nameErrors.hasChildNodes()) {
        while (nameErrors.firstChild) {
            nameErrors.removeChild(nameErrors.firstChild);
        }
        document.getElementById("name").className = "m-2 form-control";
    }
}

function clearLoginErrors() {
    const loginErrors = document.getElementById("loginFeedback");
    if (loginErrors.hasChildNodes()) {
        while (loginErrors.firstChild) {
            loginErrors.removeChild(loginErrors.firstChild);
        }
        document.getElementById("login").className = "m-2 form-control";
    }   
}

function clearPasswordErrors() {
    const passwordErrors = document.getElementById("passwordFeedback");
    if (passwordErrors.hasChildNodes()) {
        while (passwordErrors.firstChild) {
            passwordErrors.removeChild(passwordErrors.firstChild);
        }
        document.getElementById("password").className = "m-2 form-control";
    }   
}

function clearCompanyErrors() {
    const companyErrors = document.getElementById("companyFeedback");
    if (companyErrors.hasChildNodes()) {
        while (companyErrors.firstChild) {
            companyErrors.removeChild(companyErrors.firstChild);
        }
        document.getElementById("company").className = "m-2 form-select";
    } 
}

document.forms["userForm"].addEventListener("submit", e => {
    e.preventDefault();
    const form = document.forms["userForm"];
    const userId = form.elements["userId"].value;
    const name = form.elements["name"].value;
    const timeStamp = form.elements["timeStamp"].value;
    const login = form.elements["login"].value;
    const password = form.elements["password"].value;
    const companyId = form.elements["company"].value;
    if (userId == 0 && timeStamp == "")
        CreateUser(name, login, password, companyId);
    else
        EditUser(userId, name, timeStamp, login, password, companyId);
});

document.forms["userForm"].addEventListener("submit", e => {
    e.preventDefault();
    reset();
});

GetUsers();
GetCompanies();