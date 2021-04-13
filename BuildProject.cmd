echo Building...
dotnet build "MulticutInTrees" -c=Debug
ping -n 5 127.0.0.1 >NUL
dotnet build "MulticutInTrees" -c=VerboseDebug
ping -n 5 127.0.0.1 >NUL
dotnet build "MulticutInTrees" -c=Release
ping -n 5 127.0.0.1 >NUL
dotnet build "MulticutInTrees" -c=Experiment
ping -n 5 127.0.0.1 >NUL