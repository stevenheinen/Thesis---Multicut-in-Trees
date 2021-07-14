REM This file will run the kernelisation algorithm by Bousquet et al. on instances created from 3-SAT instances.
for /r D:\Documents\Universiteit\Thesis\3SAT-instances\ %%i in (*.cnf) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\3SAT-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\Bousquet3SAT" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
pause
