set "exeLocation=FILL IN IN CMD FILE"
set "repetitions=1"
set "algorithm=GenerateInstances"
set "vertexCoverInstanceDir=FILL IN IN CMD FILE"
set "multicutInstanceDir=FILL IN IN CMD FILE"
set "resultsOutputDir=FILL IN IN CMD FILE"
for /r "%vertexCoverInstanceDir%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
