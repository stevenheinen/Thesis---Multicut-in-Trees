echo Building...
dotnet build "MulticutInTrees" -c=Debug
timeout 5
dotnet build "MulticutInTrees" -c=VerboseDebug
timeout 5
dotnet build "MulticutInTrees" -c=Release
timeout 5
dotnet build "MulticutInTrees" -c=Experiment