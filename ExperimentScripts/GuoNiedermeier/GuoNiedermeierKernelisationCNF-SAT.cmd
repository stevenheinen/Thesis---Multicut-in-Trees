set "exeLocation=P:\Thesis - Multicut in Trees\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe"
set "algorithm=GuoNiedermeierKernelisation"
set "repetitions=1"
set "cnfsatInstanceDir=D:\Documents\Universiteit\Thesis\CNF-SAT-instances\"
set "multicutInstanceDir=D:\Documents\Universiteit\Thesis\Instances\CNF-SAT-instances"
set "resultsOutputDir=D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCNFSAT"
for /r "%cnfsatInstanceDir%" %%i in (*.cnf) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
