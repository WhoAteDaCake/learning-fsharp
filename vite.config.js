// vite.config.js
import { defineConfig } from 'vite'
import dns from 'dns'
import react from '@vitejs/plugin-react'
import history from 'connect-history-api-fallback';
import * as path from 'path'

dns.setDefaultResultOrder('verbatim')

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