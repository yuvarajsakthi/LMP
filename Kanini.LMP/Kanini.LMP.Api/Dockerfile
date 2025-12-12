FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Kanini.LMP.Api/Kanini.LMP.Api.csproj", "Kanini.LMP.Api/"]
COPY ["Kanini.LMP.Application/Kanini.LMP.Application.csproj", "Kanini.LMP.Application/"]
COPY ["Kanini.LMP.Data/Kanini.LMP.Data.csproj", "Kanini.LMP.Data/"]
COPY ["Kanini.LMP.Database/Kanini.LMP.Database.csproj", "Kanini.LMP.Database/"]
RUN dotnet restore "Kanini.LMP.Api/Kanini.LMP.Api.csproj"
COPY . .
WORKDIR "/src/Kanini.LMP.Api"
RUN dotnet build "Kanini.LMP.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kanini.LMP.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG DB_CONNECTION_STRING
ARG JWT_KEY
ARG JWT_ISSUER
ARG JWT_AUDIENCE
ARG SMTP_HOST
ARG SMTP_PORT
ARG SMTP_USERNAME
ARG SMTP_PASSWORD

ENV ConnectionStrings__LMPConnect=${DB_CONNECTION_STRING}
ENV Jwt__Key=${JWT_KEY}
ENV Jwt__Issuer=${JWT_ISSUER}
ENV Jwt__Audience=${JWT_AUDIENCE}
ENV SmtpSettings__Host=${SMTP_HOST}
ENV SmtpSettings__Port=${SMTP_PORT}
ENV SmtpSettings__Username=${SMTP_USERNAME}
ENV SmtpSettings__Password=${SMTP_PASSWORD}

ENTRYPOINT ["dotnet", "Kanini.LMP.Api.dll"]
