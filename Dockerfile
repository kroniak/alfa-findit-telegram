# build ui app
FROM node:10 AS react-build
WORKDIR /app
COPY src/AlfaBot.Client .
RUN npm i && \
    npm run build

# build server app
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
COPY . .
RUN ./build/test.sh && \
    ./build/build.sh

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=react-build /app/build ./wwwroot
ENV ASPNETCORE_URLS=http://+:5000
HEALTHCHECK --interval=10m --timeout=5s \
  CMD curl --fail http://localhost:5000/health/live || exit 1
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "AlfaBot.Host.dll"]