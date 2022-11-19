// vite.config.js
import { defineConfig } from 'vite'
import dns from 'dns'
import react from '@vitejs/plugin-react'
import history from 'connect-history-api-fallback';
import * as path from 'path'

dns.setDefaultResultOrder('verbatim')

// function chartingLibrary() {
//     return {
//         apply: 'serve',
//         configureServer(server) {
//             return () => {
//                 server.middlewares.use(async (req, res, next) => {
//                     if (req.originalUrl?.includes('/charting_library/') && req.originalUrl?.includes('html')) {
//                         res.setHeader('Content-Type', 'text/html');
//                         res.writeHead(200);
//                         res.write(fs.readFileSync(path.join(__dirname, `public/${req.originalUrl}`)));
//                         res.end();
//                     }
//
//                     next();
//                 });
//             };
//         },
//         name: 'charting-library',
//     };
// }
//
// export function speciallSpaFallback() {
//     return {
//         name: 'special-spa-fallback',
//         config() {
//             return {
//                 appType: 'spa'
//             }
//         },
//         configureServer(server) {
//             return () => {
//                 console.log("Test")
//                 server.middlewares.use(history({
//                     index: path.resolve(__dirname, "src/Client/index.html")
//                 }))
//             }
//         }
//     }
// }

export function myPlugin() {
    return {
        name: 'real-spa-fallback',

        configureServer(server) {
            server.middlewares.use((req, res, next) => {
                console.log(req.url)
                const [, firstPart, ...pathParts] = req.url.split('/')
                const lastPart = pathParts[pathParts.length- 1]

                // these a vite internal URLs
                if (firstPart.startsWith('_') || firstPart.startsWith('@')) {
                    return next()
                }

                // URLs that explicitly target files are not rewritten
                if (lastPart && lastPart.includes('.')) {
                    return next()
                }

                // rewrite /foo/bar/baz to /foo/ the downstream middleware 'indexhtml' will take care of the rest
                const match = req.url.match(/^\/([^\/]+)\/?/)
                if (match && match[1]) {
                    req.url = `/${match[1]}/`
                }

                next()
            })
        }
    }
}

export default defineConfig({
    define: {
        // By default, Vite doesn't include shims for NodeJS/
        // necessary for segment analytics lib to work
        global: {},
    },
    appType: 'spa',
    plugins: [react()],
    // plugins: [myPlugin()],
    server: {
        proxy: {
            '/api/**': {
                target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || '5000'),
                changeOrigin: true
            },
            // redirect websocket requests that start with /socket/ to the server on the port 5000
            '/socket/**': {
                target: 'http://localhost:' + (process.env.SERVER_PROXY_PORT || '5000'),
                ws: true
            }
        }
    }
})