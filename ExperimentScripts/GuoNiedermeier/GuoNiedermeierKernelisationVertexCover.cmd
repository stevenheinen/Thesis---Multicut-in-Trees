set "exeLocation=P:\Thesis - Multicut in Trees\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe"
set "repetitions=1"
set "algorithm=GuoNiedermeierKernelisation"
set "vertexCoverInstanceDir1=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb30-15-mis\"
set "vertexCoverInstanceDir2=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb35-17-mis\"
set "vertexCoverInstanceDir3=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb40-19-mis\"
set "vertexCoverInstanceDir4=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb45-21-mis\"
set "vertexCoverInstanceDir5=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb50-23-mis\"
set "vertexCoverInstanceDir6=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb53-24-mis\"
set "vertexCoverInstanceDir7=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb56-25-mis\"
set "vertexCoverInstanceDir8=D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb59-26-mis\"
set "multicutInstanceDir=D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances"
set "resultsOutputDir=D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover"
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
