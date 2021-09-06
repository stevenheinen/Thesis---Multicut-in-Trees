set "exeLocation=FILL IN IN CMD FILE"
set "repetitions=1"
set "algorithm=GuoNiedermeierKernelisation"
set "vertexCoverInstanceDir1=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir2=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir3=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir4=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir5=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir6=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir7=FILL IN IN CMD FILE"
set "vertexCoverInstanceDir8=FILL IN IN CMD FILE"
set "multicutInstanceDir=FILL IN IN CMD FILE"
set "resultsOutputDir=FILL IN IN CMD FILE"
for /r "%vertexCoverInstanceDir1%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir2%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir3%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir4%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir5%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir6%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir7%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
for /r "%vertexCoverInstanceDir8%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
