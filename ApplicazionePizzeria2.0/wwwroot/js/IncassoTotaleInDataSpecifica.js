const BtnOrdiniTot = document.getElementById("btnOrdiniEvasi");
const btnTotaleIncassatoInData = document.getElementById("btnTotaleIncassatoInData");

const MyFetch = () => {
    fetch("/Ordine/FetchTotOrdniEvasi")
        .then(response => response.json())
        .then(data => {
            pOutput = document.getElementById("outputElement");
            pOutput.innerHTML = `Totale ordini evasi: ${data}`;
        })
}

document.addEventListener("DOMContentLoaded", () => {

    BtnOrdiniTot.addEventListener("click", () => {
        MyFetch();
    })

    btnTotaleIncassatoInData.addEventListener("click", () => {

        const inputDate = document.getElementById("idinputDate").value;

        fetch(`/Ordine/FetchTotIncassatoInData?data=${inputDate}`)
            .then(response => response.json())
            .then(data => {
                pOutput = document.getElementById("outputElement");
                pOutput.innerHTML = `Totale incassato in data: ${data} €`;
            })
    })

})