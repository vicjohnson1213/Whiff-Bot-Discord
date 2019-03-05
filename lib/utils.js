function bool(value) {
    value = value.toLowerCase ? value.toLowerCase() : value;
    value = value === 'true' ? true : value;
    value = value === 'false' ? false : value;

    return value;
}

module.exports = {
    bool: bool
};
