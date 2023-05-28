FROM mcr.microsoft.com/dotnet/sdk:6.0 as builder
WORKDIR /app
COPY Bot/ .
RUN dotnet restore
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef
RUN dotnet publish -c Release -o out
RUN dotnet ef database update

FROM mcr.microsoft.com/dotnet/runtime:6.0 as runner
WORKDIR /app
COPY --from=builder /app/out .

CMD dotnet Bot.dll --usevirtualterminal
