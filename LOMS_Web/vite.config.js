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
