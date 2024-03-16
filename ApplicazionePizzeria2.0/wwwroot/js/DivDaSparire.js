
document.addEventListener("DOMContentLoaded", () => {
    const divAnimazione = document.querySelectorAll(".heartbeat");

    divAnimazione.forEach(divAnimato => {
        setTimeout(() => {
            divAnimato.style.display = "none";
        }, 3000)
    })
});