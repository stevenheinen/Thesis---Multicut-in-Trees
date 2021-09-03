set "exeLocation=P:\Thesis - Multicut in Trees\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe"
set "repetitions=1"
set "algorithm=GenerateInstances"
set "satInstanceDir=D:\Documents\Universiteit\Thesis\3SAT-instances\"
set "multicutInstanceDir=D:\Documents\Universiteit\Thesis\Instances\3SAT-instances"
set "resultsOutputDir=D:\Documents\Universiteit\Thesis\ExperimentResults"
for /r "%satInstanceDir%" %%i in (*.cnf) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
