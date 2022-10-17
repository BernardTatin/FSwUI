dotnet clean -c Release
REM dotnet build -c Release -p:DefineConstants=LOGGER
REM dotnet build -c Release -p:DefineConstants=RECURSEBM -p:DefineConstants=LOGGER
dotnet build -c Release -p:DefineConstants=\"LOGGER;RECURSEBM"
REM dotnet build -c Release
bin\Release\net48\FImage.exe
