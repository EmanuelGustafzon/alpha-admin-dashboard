const validateField = (field) => {
    let errorSpan = document.querySelector(`span[data-valmsg-for='${field.name}']`);
    if (!errorSpan) return;
    errorSpan.classList.add("text-error")
    let errorMsg = "";
    let value = field.value.trim();

    if (field.hasAttribute("data-val-required") && value === "") {
        errorMsg = field.getAttribute("data-val-required");
    }

    if (field.hasAttribute("data-val-regex") && value !== "") {
        let pattern = new RegExp(field.getAttribute("data-val-regex-pattern"));
        if (!pattern.test(value)) {
            errorMsg = field.getAttribute("data-val-regex");
        }
    }

    if (errorMsg) {
        field.classList.add("input-validation-error");
        errorSpan.classList.remove("field-validation-valid");
        errorSpan.classList.add("field-validation-error");
        errorSpan.innerText = errorMsg;
    } else {
        field.classList.remove("input-validation-error");
        errorSpan.classList.remove("field-validation-error");
        errorSpan.classList.add("field-validation-valid");
        errorSpan.innerText = "";
    }
};

document.querySelectorAll("form").forEach(form => {
    const fields = form.querySelectorAll("input[data-val='true']");
    fields.forEach(field => {
        field.addEventListener("input", () => validateField(field));
    });
});