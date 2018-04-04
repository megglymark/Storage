#FROM microsoft/aspnetcore-build AS build-env
#WORKDIR /app
#
## copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore
#
## copy everything else and build
#COPY . ./
#RUN dotnet publish -c Release -o out
#ENTRYPOINT ["dotnet", "/app/out/Storage.dll"]
#
#FROM microsoft/aspnetcore
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "Storage.dll"]

FROM microsoft/dotnet
WORKDIR /app/src

ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_URLS=http://*:5000
ENV ASPNETCORE_ENVIRONMENT=Development
COPY Storage.csproj .
COPY NuGet.config .
COPY Directory.Build.props .
RUN dotnet restore
#RUN dotnet ef database update --msbuildprojectextensionspath ../out/obj
#ENTRYPOINT [ "dotnet", "ef", "database", "update", "--msbuildprojectextensionspath", "../out/obj", "&&", "dotnet", "watch", "run", "--no-restore" ]
ENTRYPOINT [ "dotnet", "watch", "run", "--no-restore" ]

#ENTRYPOINT [ "dotnet", "watch", "run", "--no-restore" ]
#CMD ["/bin/bash", "-c", "dotnet restore && dotnet ef database update &&  dotnet watch run"]
