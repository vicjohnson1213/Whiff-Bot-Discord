const _ = require('lodash');

function group(arr, size) {
    const shuffled = _.shuffle(arr);
    return _.chunk(shuffled, size);
}

module.exports = {
    group: group
};