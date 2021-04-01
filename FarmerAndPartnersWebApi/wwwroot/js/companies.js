async function GetContractStatuses() {
    const response = await fetch("/api/contractStatuses", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const contractStatuses = await response.json();
        let contractStatusSelect = document.getElementById("contractStatus");
        contractStatuses.forEach(contractStatus => {
            addContractStatus(contractStatus, contractStatusSelect);
        });
    }
}

async function GetCompanies() {
    const response = await fetch("/api/companies", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const companies = await response.json();
        let rows = document.querySelector("tbody");
        companies.forEach(company => {
            rows.append(row(company));
        });
    }
}

async function GetCompany(id) {
    const response = await fetch("/api/companies/" + id, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const company = await response.json();
        const form = document.forms["companyForm"];
        form.elements["companyId"].value = company.id;
        form.elements["timeStamp"].value = company.timeStamp;
        form.elements["name"].value = company.name;
        form.elements["contractStatus"].value = company.contractStatusId;
    }
}

async function CreateCompany(name, contractStatusId) {
    const response = await fetch("/api/companies", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            name: name,
            contractStatusId: parseInt(contractStatusId, 10)
        })
    });
    if (response.ok === true) {
        const company = await response.json();
        reset();
        document.querySelector("tbody").append(row(company));
    }
    else {
        const errorData = await response.json();
        clearNameErrors();
        clearContractStatusErrors();
        if (errorData.errors) {
            if (errorData.errors["Name"]) {
                addNameError(errorData.errors["Name"]);
            }
            if (errorData.errors["ContractStatusId"]) {
                addContractStatusError(errorData.errors["ContractStatusId"]);
            }
        }
        if (errorData["Name"]) {
            addNameError(errorData["Name"]);
        }
    }
}

async function EditCompany(id, name, timeStamp, contractStatusId) {
    const response = await fetch("/api/companies", {
        method: "PUT",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            id: parseInt(id, 10),
            name: name,
            timeStamp: timeStamp,
            contractStatusId: parseInt(contractStatusId, 10)
        })
    });
    if (response.ok === true) {
        const company = await response.json();
        reset();
        document.querySelector("tr[data-rowid='" + company.id + "']").replaceWith(row(company));
    }
    else {
        const errorData = await response.json();
        clearNameErrors();
        clearContractStatusErrors();
        if (errorData.errors) {
            if (errorData.errors["Name"]) {
                addNameError(errorData.errors["Name"]);
            }
        }
        if (errorData["Name"]) {
            addNameError(errorData["Name"]);
        }
    }
}

async function DeleteCompany(id) {
    const response = await fetch("/api/companies/" + id, {
        method: "DELETE",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const company = await response.json();
        reset();
        document.querySelector("tr[data-rowid='" + company.id + "']").remove();
    }
}

function row(company) {

    const tr = document.createElement("tr");
    tr.setAttribute("data-rowid", company.id);

    const idTd = document.createElement("td");
    idTd.append(company.id);
    tr.append(idTd);

    const nameCompanyTd = document.createElement("td");
    nameCompanyTd.append(company.name);
    tr.append(nameCompanyTd);

    const nameContractStatusTd = document.createElement("td");
    nameContractStatusTd.append(company.contractStatus.definition);
    tr.append(nameContractStatusTd);

    const linksTd = document.createElement("td");

    const editLink = document.createElement("a");
    editLink.setAttribute("data-id", company.id);
    editLink.setAttribute("style", "cursor:pointer;padding:15px;color:CornflowerBlue;");
    editLink.append("Изменить");
    editLink.addEventListener("click", e => {
        e.preventDefault();
        GetCompany(company.id);
        clearNameErrors();
        clearContractStatusErrors();
    });
    linksTd.append(editLink);

    linksTd.append("|");

    const removeLink = document.createElement("a");
    removeLink.setAttribute("data-id", company.id);
    removeLink.setAttribute("style", "cursor:pointer;padding:15px;color:CornflowerBlue;");
    removeLink.append("Удалить");
    removeLink.addEventListener("click", e => {
        e.preventDefault();
        DeleteCompany(company.id);
    });
    linksTd.append(removeLink);

    tr.appendChild(linksTd);

    return tr;
}

function addContractStatus(contractStatus, contractStatusSelect) {

    const option = new Option(contractStatus.definition, contractStatus.id);
    contractStatusSelect.options[contractStatusSelect.options.length] = option;
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

function addContractStatusError(errors) {
    document.getElementById("contractStatus").className = "m-2 form-select is-invalid";
    let content = document.getElementById("contractStatusFeedback");
    errors.forEach(error => {
        const p = document.createElement("p");
        p.append(error);
        content.append(p);
    });
}

function reset() {
    const form = document.forms["companyForm"];
    form.elements["companyId"].value = 0;
    form.elements["timeStamp"].value = "";
    form.elements["name"].value = "";
    form.elements["contractStatus"].value = document.getElementById("contractStatus").options[0].value;
    clearNameErrors();
    clearContractStatusErrors();
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

function clearContractStatusErrors() {
    const contractStatusErrors = document.getElementById("contractStatusFeedback");
    if (contractStatusErrors.hasChildNodes()) {
        while (contractStatusErrors.firstChild) {
            contractStatusErrors.removeChild(contractStatusErrors.firstChild);
        }
        document.getElementById("contractStatus").className = "m-2 form-select";
    }  
}

document.forms["companyForm"].addEventListener("submit", e => {
    e.preventDefault();
    const form = document.forms["companyForm"];
    const companyId = form.elements["companyId"].value;
    const name = form.elements["name"].value;
    const timeStamp = form.elements["timeStamp"].value;
    const contractStatusId = form.elements["contractStatus"].value;
    if (companyId == 0 && timeStamp == "")
        CreateCompany(name, contractStatusId);
    else
        EditCompany(companyId, name, timeStamp, contractStatusId);
});

document.forms["companyForm"].addEventListener("reset", e => {
    e.preventDefault();
    reset();
});

GetCompanies();
GetContractStatuses();