# build server app
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
COPY . .
RUN ./build/test.sh && ./build/build.sh

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS=http://+:5000
HEALTHCHECK --interval=1m --timeout=5s \
  CMD curl --fail http://localhost:5000/health/live || exit 1
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "AlfaBot.Host.dll"]