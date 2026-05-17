(function () {
    const accordion = document.getElementById('workoutHistoryAccordion');
    if (!accordion) {
        return;
    }

    accordion.addEventListener('show.bs.collapse', async function (event) {
        const panel = event.target.querySelector('.workout-detail-panel');
        if (!panel || panel.dataset.loaded === 'true') {
            return;
        }

        const url = panel.dataset.detailUrl;
        if (!url) {
            return;
        }

        try {
            const response = await fetch(url);
            if (!response.ok) {
                panel.innerHTML = '<p class="text-danger mb-0">Unable to load workout details.</p>';
                return;
            }

            panel.innerHTML = await response.text();
            panel.dataset.loaded = 'true';
        } catch {
            panel.innerHTML = '<p class="text-danger mb-0">Unable to load workout details.</p>';
        }
    });
})();
