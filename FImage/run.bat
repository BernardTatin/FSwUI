set release=Release4Perf
if %release%==Release4Perf (
    set options=-p:DefineConstants=\"LOGGER;RECURSEBM"
) else (
    set options=""
)
dotnet clean -c %release%
REM || goto :ERROR
dotnet build -c %release% %options% || goto :ERROR
bin\%release%\net48\FImage.exe || goto :ERROR
goto :END
:ERROR
echo "Cannot compile and run %release%"
:END
