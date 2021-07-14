REM This file will run the kernelisation algorithm by Bousquet et al. on instances created from CNF-SAT instances.
for %%i in (D:\Documents\Universiteit\Thesis\CNF-SAT-instances\*.cnf) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\CNF-SAT-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetCNFSAT" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
pause
