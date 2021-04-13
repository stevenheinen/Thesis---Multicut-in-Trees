echo Building...
dotnet build "MulticutInTrees" -c=Debug
timeout 8 /nobreak
dotnet build "MulticutInTrees" -c=VerboseDebug
timeout 8 /nobreak
dotnet build "MulticutInTrees" -c=Release
timeout 8 /nobreak
dotnet build "MulticutInTrees" -c=Experiment
timeout 8 /nobreak