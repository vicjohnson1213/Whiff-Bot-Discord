module.exports.addReactions = addReactions;

function addReactions(msg, reactions, current = 0) {
    if (current == reactions.length) {
        return Promise.resolve();
    }

    return msg.react(reactions[current].reaction)
        .then(() => addReactions(msg, reactions, current + 1));
}