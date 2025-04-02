window.getSelectedValues = (selectElement) => {
    const selected = [];
    for (let option of selectElement.options) {
        if (option.selected) {
            selected.push(parseInt(option.value));
        }
    }
    return selected;
};
