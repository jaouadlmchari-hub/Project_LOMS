# Étape 1 : Build React
FROM node:20-alpine AS build

WORKDIR /app

# On cible explicitement le dossier LOMS_Web depuis la racine du projet
COPY LOMS_Web/package*.json ./
RUN npm install

# On copie tout le contenu du dossier de l'application web
COPY LOMS_Web/ . 

RUN npm run build

# Étape 2 : Nginx pour servir l'app
FROM nginx:stable-alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]