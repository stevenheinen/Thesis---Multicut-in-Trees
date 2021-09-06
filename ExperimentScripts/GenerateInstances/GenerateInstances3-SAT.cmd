set "exeLocation=FILL IN IN CMD FILE"
set "repetitions=1"
set "algorithm=GenerateInstances"
set "satInstanceDir=FILL IN IN CMD FILE"
set "multicutInstanceDir=FILL IN IN CMD FILE"
set "resultsOutputDir=FILL IN IN CMD FILE"
for /r "%satInstanceDir%" %%i in (*.cnf) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
