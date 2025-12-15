# Utilizar una imagen base que incluya Node.js y npm
FROM node:20-alpine AS builder

# Establecer el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copiar los archivos de configuración de dependencias
COPY package*.json ./

# Instalar las dependencias
RUN npm install

# Copiar los archivos de la aplicación al contenedor
COPY . .

# Construir la aplicación
RUN npm run build

# Etapa de producción
FROM nginx:alpine

# Copiar la configuración personalizada de nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copiar los archivos compilados de Angular desde la etapa anterior
# Angular 19 usa la estructura dist/theme-park-ui/browser/
COPY --from=builder /app/dist/theme-park-ui/browser /usr/share/nginx/html/

# El puerto que escucha NGINX
EXPOSE 8080

# Comando para iniciar NGINX
CMD ["nginx", "-g", "daemon off;"]
