
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
<<<<<<< Updated upstream
  plugins: [react(), tailwindcss()],
  // C'est cette ligne qui change tout pour le déploiement proxy
  base: './', 
})


=======
  plugins: [react()],
  server: {
    host: '0.0.0.0',
    port: 5000,
    allowedHosts: true,
  },
})
>>>>>>> Stashed changes
