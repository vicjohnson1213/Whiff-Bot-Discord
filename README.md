# Whiff Bot for Discord

Just a little bot that brings some useful utilities to Discord.

## Commands

*Notes:*

- All commands are prefixed with an exclamation point (e.g. `!command`).
- Any options or arguments surrounded by `<...>` are required, while any surrounded by `[...]` are optional.

### `groups [size] [channel]`

Randomly splits all users in a voice channel into groups of a specified size.

**Options:**

| Option | Default | Description |
| ------ | ------- | ----------- |
| size   | 2 | The size of the groups you want to form. |
| channel | *General* | The voice channel to read users from. |

### `roll [sides]`

Rolls an *n* sided die

**Options:**

| Option | Default | Description |
| ------ | ------- | ----------- |
| sides | **required** | The number of sides on the die. |

### `straws [channel]`

Draws straws for each user in a voice channel.

**Options:**

| Option | Default | Description |
| ------ | ------- | ----------- |
| channel | *General* | The voice channel to read users from. |
