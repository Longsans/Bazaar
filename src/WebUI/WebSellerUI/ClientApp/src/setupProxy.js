const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = `http://localhost:${env.REACT_APP_BFF_PORT}` ;

const context = [
  "/bff",
  "/api"
];

const onError = (err, req, resp, target) => {
    console.error(`${err.message}`);
}

module.exports = function (app) {
  if (env.REACT_APP_DEV) {
    const appProxy = createProxyMiddleware(context, {
      proxyTimeout: 10000,
      target: target,
      // Handle errors to prevent the proxy middleware from crashing when
      // the ASP NET Core webserver is unavailable
      onError: onError,
      secure: false,
      // Uncomment this line to add support for proxying websockets
      //ws: true, 
      headers: {
        Connection: 'Keep-Alive'
      }
    });

    app.use(appProxy);
  }
};
