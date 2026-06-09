<<<<<<< HEAD
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// Si tailwindcss est importé via un plugin Vite, assurez-vous qu'il est installé,
// sinon vous pouvez enlever "tailwindcss()" de la liste des plugins.
export default defineConfig({
  plugins: [react()],
  base: "./",
  server: {
    host: "0.0.0.0",
    port: 5000,
    allowedHosts: true,
  },
});
=======
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
>>>>>>> feat/employee-ui-infra-update
