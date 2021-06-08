REM This file will create txt files with instances created from CNF-SAT instances.
for %%i in (D:\Documents\Universiteit\Thesis\CNF-SAT-instances\*.cnf) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GenerateInstances --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
pause
