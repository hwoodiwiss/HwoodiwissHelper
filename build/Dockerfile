FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
ARG TARGETARCH
RUN dpkg --add-architecture arm64
RUN apt update && apt install -y clang zlib1g-dev zlib1g-dev:arm64 gcc-aarch64-linux-gnu llvm

WORKDIR /source

COPY . .
RUN dotnet publish ./src/HwoodiwissHelper/HwoodiwissHelper.csproj -c Release -a $TARGETARCH --self-contained /p:PublishTrimmed=true /p:PublishAot=true /p:StaticallyLinked=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS run
ARG TARGETARCH

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /source/artifacts/publish/HwoodiwissHelper/release_*/ .
ENTRYPOINT [ "./HwoodiwissHelper" ]