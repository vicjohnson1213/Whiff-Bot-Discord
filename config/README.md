# Project Configuration

## auth.js

You should add a file in this config directory named `auth.js` with the following content:

```js
module.exports = {
    token: 'YOUR BOT TOKEN HERE',
    sheets: { /* The google sheets authentication object from the directions. */}
};
```

[Google Sheets authentication directions](https://github.com/theoephraim/node-google-spreadsheet#service-account-recommended-method)
