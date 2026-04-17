FROM node:18-buster AS setup
WORKDIR app
COPY package.json .
COPY package-lock.json .
RUN npm ci
COPY . .

FROM setup as test
ENTRYPOINT ["npm", "run", "test:ci"]

FROM setup as builder
RUN npm run build:ci

FROM nginx:stable-alpine as final
COPY --from=builder app/build usr/share/nginx/html
EXPOSE 80
