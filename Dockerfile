# build ui app
FROM node:8 AS react-build
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
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "AlfaBot.Host.dll"]