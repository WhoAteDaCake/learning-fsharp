// vite.config.js
import { defineConfig } from 'vite'
import dns from 'dns'

dns.setDefaultResultOrder('verbatim')

export default defineConfig({
    define: {
        // By default, Vite doesn't include shims for NodeJS/
        // necessary for segment analytics lib to work
        global: {},
    },
    css: {
        preprocessorOptions: {
            less: {
                javascriptEnabled: true,
            },
        },
    },
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