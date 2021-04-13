echo Building...
dotnet build "MulticutInTrees" -c=Debug
ping -n 10 127.0.0.1
dotnet build "MulticutInTrees" -c=VerboseDebug
ping -n 10 127.0.0.1
dotnet build "MulticutInTrees" -c=Release
ping -n 10 127.0.0.1
dotnet build "MulticutInTrees" -c=Experiment
ping -n 10 127.0.0.1