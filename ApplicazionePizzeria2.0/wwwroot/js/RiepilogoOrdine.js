document.addEventListener('DOMContentLoaded', () => {
	const ordini = document.querySelectorAll(".mycustomColor");

	ordini.forEach((ordine) => {
		const idOrdineElement = ordine.querySelector(".idordine");
		const idOrdine = idOrdineElement.textContent.replace("Ordine N°: ", "");
		fetch(`/Ordine/TotCostoOrdine/?idOrdine=${idOrdine}`)
			.then(response => response.json())
			.then(data => {
				const prezzoTotalissimo = ordine.querySelector(".pPrezzo_totalissimo");
				prezzoTotalissimo.textContent = `${data} €`;
			});
	});
});