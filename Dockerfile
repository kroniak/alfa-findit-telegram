FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN ./build/test.sh && \
    ./build/build.sh

# build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 5000/tcp
ENTRYPOINT ["dotnet", "AlfaBot.dll"]