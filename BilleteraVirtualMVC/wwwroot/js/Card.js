// Escucha los cambios en los campos del formulario
document.querySelector("form").addEventListener("input", function (e) {
    const target = e.target;
    const name = target.name;
    const value = target.value;
    updateCreditCardField(name, value);
});

// Función para actualizar la tarjeta de crédito simulada
function updateCreditCardField(name, value) {
    const card = document.getElementById("credit-card");

    switch (name) {
        case "bank":
            // Actualiza el banco en la tarjeta de crédito
            // card.querySelector(".credit-card__bank").textContent = value;
            break;
        case "id":
            // Actualiza el número de tarjeta en la tarjeta de crédito
            card.querySelector(".credit-card__number").textContent = formatCardNumber(value);
            break;
        case "issuer":
            // Actualiza el emisor en la tarjeta de crédito
            // card.querySelector(".credit-card__issuer").textContent = value;
            break;
        case "owner":
            // Actualiza el nombre del titular en la tarjeta de crédito
            card.querySelector(".credit-card__name").textContent = value;
            break;
        case "cvv":
            // Actualiza el CVV en la tarjeta de crédito
            // card.querySelector(".credit-card__cvv").textContent = value;
            break;
        case "codedate":
            // Actualiza la fecha de vencimiento en la tarjeta de crédito
            card.querySelector(".credit-card__valid-thru").textContent = value;
            break;
        default:
            break;
    }
}

// Formatea el número de tarjeta para mostrar en la tarjeta de crédito
function formatCardNumber(cardNumber) {
    // Implementa tu lógica de formato aquí
    return cardNumber;
}
