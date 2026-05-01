import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    host: true, // <--- C'est CRUCIAL pour que Docker puisse accéder à React
    port: 5174  // <--- Assure-toi que le port correspond à celui dans nginx.conf
  }
})