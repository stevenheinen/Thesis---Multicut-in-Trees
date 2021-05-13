REM This file will use Guo and Niedermeier's kernelisation algorithm to solve instances.
REM Trees are generated using the Prüfer method, and demand pairs are generated uniform randomly.
REM The used numbers of nodes are: 2048, 3072, 4096, 5120, 6144, 7168, 8192, 9216, 10240.
REM The used numbers of demand pairs are: 2048, 3072, 4096, 5120, 6144, 7168, 8192, 9216, 10240.
REM There are 10 different instances for each combination of nodes and demand pair.
REM This means the seeds for the RNG are: 0, 1, ..., 9.
REM Each instance will be repeated 5 times to compensate for difference in running time because of external factors.
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=2048 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=3072 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=4096 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=5120 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=6144 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=7168 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=8192 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=9216 --nrDPs=10240
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=2048
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=3072
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=4096
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=5120
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=6144
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=7168
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=8192
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=9216
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --seed=0 --experiments=10 --algorithm=GenerateInstances --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --nrNodes=10240 --nrDPs=10240
del mipMinSolSizeSolver.log
pause
