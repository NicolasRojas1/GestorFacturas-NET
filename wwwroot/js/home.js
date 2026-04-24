function updateCard(tipo) {
    const header = document.getElementById('cardHeader');
    const card = document.getElementById('portalCard');
    const iconDiv = document.getElementById('headerIcon');

    // 1. Limpieza total de estados anteriores
    card.classList.remove('card-factura--active-agua', 'card-factura--active-gas', 'card-factura--active-luz');
    header.style.color = 'white'; // Reset color texto por si venía de Luz

    // 2. Aplicar nuevos estilos según el tipo
    if (tipo === 'agua') {
        header.style.backgroundColor = 'var(--color-agua)';
        card.classList.add('card-factura--active-agua');
        iconDiv.innerHTML = '<i class="fas fa-faucet fa-4x mb-3 animate__animated animate__bounceIn"></i>';
    }
    else if (tipo === 'gas') {
        header.style.backgroundColor = 'var(--color-gas)';
        card.classList.add('card-factura--active-gas');
        iconDiv.innerHTML = '<i class="fas fa-fire fa-4x mb-3 animate__animated animate__bounceIn"></i>';
    }
    else if (tipo === 'luz') {
        header.style.backgroundColor = 'var(--color-luz)';
        header.style.color = '#212529'; // Texto oscuro para fondo amarillo
        card.classList.add('card-factura--active-luz');
        iconDiv.innerHTML = '<i class="fas fa-lightbulb fa-4x mb-3 animate__animated animate__bounceIn"></i>';
    }
}

function resetCard() {
    const header = document.getElementById('cardHeader');
    const card = document.getElementById('portalCard');
    const iconDiv = document.getElementById('headerIcon');

    header.style.backgroundColor = 'var(--color-neutral)';
    header.style.color = 'white';
    card.classList.remove('card-factura--active-agua', 'card-factura--active-gas', 'card-factura--active-luz');
    iconDiv.innerHTML = '<i class="fas fa-file-invoice-dollar fa-4x mb-3"></i>';
}