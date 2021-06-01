REM This file will use Guo and Niedermeier's kernelisation algorithm to solve instances.
REM Trees are generated using the Prüfer method, and demand pairs are generated uniform randomly.
REM The used numbers of nodes are: 128, 256, 384, 512, 640, 768, 896, 1024.
REM The used numbers of demand pairs are: 128, 256, 384, 512, 640, 768, 896, 1024.
REM There are 10 different instances for each combination of nodes and demand pair.
REM Each instance will be repeated 10 times to compensate for difference in running time because of external factors.
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9472052 --dpSeed=6050219 --experiments=3 --repetitions=1 --algorithm=BousquetKernelisation --treeType=Prufer --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Downloads" -v --nrNodes=256 --nrDPs=256
pause