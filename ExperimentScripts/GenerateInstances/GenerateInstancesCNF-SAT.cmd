set "exeLocation=FILL IN IN CMD FILE"
set "algorithm=GenerateInstances"
set "repetitions=1"
set "cnfsatInstanceDir=FILL IN IN CMD FILE"
set "multicutInstanceDir=FILL IN IN CMD FILE"
set "resultsOutputDir=FILL IN IN CMD FILE"
for /r "%cnfsatInstanceDir%" %%i in (*.cnf) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
