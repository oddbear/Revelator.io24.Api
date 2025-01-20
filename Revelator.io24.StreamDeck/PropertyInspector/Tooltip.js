// Must add: <div id="tooltip" class="tooltip"></div> and Tooltip.css

document.addEventListener('DOMContentLoaded', function () {
    const div_tooltip = document.getElementById('tooltip');
    const sdpi_ranges = document.querySelectorAll('sdpi-range');

    sdpi_ranges.forEach(sdpi_range => {
        const input_range = sdpi_range.shadowRoot.querySelector('input[type="range"]');

        // Show the tooltip on mouseover
        input_range.addEventListener('mouseover', function () {
            div_tooltip.style.display = 'block';
            updateTooltip();
        });

        // Hide the tooltip on mouseout
        input_range.addEventListener('mouseout', function () {
            div_tooltip.style.display = 'none';
        });

        // Update the tooltip on input change
        input_range.addEventListener('input', updateTooltip);

        // Function to update the tooltip position and content
        function updateTooltip() {
            const rect = input_range.getBoundingClientRect();
            const tooltipWidth = div_tooltip.offsetWidth;
            const rangeWidth = rect.width;
            const rangeValue = input_range.value;
            const max = input_range.max;
            const min = input_range.min;

            // Calculate the position of the tooltip
            const left = rect.left + window.scrollX + (rangeWidth * (rangeValue - min) / (max - min)) - (tooltipWidth / 2);
            const top = rect.top + window.scrollY - div_tooltip.offsetHeight;

            // Update tooltip position and content
            div_tooltip.style.left = `${left}px`;
            div_tooltip.style.top = `${top}px`;
            div_tooltip.textContent = `${rangeValue}`;
        }

        // Initial update to ensure correct positioning
        updateTooltip();

        // https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver/observe#using_attributefilter
        // Sdpi uses Lit Components, and the title attribute is automatically added to the input element on change.
        // See '.title' in: https://github.com/GeekyEggo/sdpi-components/blob/bfc3dc6af045b49c1dafc018e85cc1d438918b29/src/components/range.ts
        // The other problem, is that since this is on change, we get several issues:
        // - We need to remove it initially, but it's added back on 'model binding', without triggering a change event.
        // - We cannot use the 'valuechange' on the sdpi-range, as the value then is not actually added back again.
        // - We can use the 'change' event on the input, but we will not be able to remove the first title.
        // - We can remove the attribute with a setTimeout, so it should be possible to just have a infinite loop that removes it.
        // All in all, the MutationObserver seems to be the best way.
        const observer = new MutationObserver(mutations => {
            mutations
                .filter(mutation => mutation.attributeName === 'title')
                // Does not trigger another mutation when removed, just when it's added back again.
                .forEach(mutation => mutation.target.removeAttribute('title'));
        });

        // Start observing the input element for attribute changes
        observer.observe(input_range, { attributeFilter: ["title"] });

        // Remove the attribute, for the observation to trigger when it's added back again.
        input_range.removeAttribute('title');
    });
});
