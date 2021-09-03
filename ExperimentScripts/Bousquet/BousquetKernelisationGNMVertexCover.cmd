set "exeLocation=P:\Thesis - Multicut in Trees\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe"
set "repetitions=1"
set "algorithm=BousquetKernelisation"
set "vertexCoverInstanceDir=D:\Documents\Universiteit\Thesis\GNMVertexCoverInstances\"
set "multicutInstanceDir=D:\Documents\Universiteit\Thesis\Instances\GNMVertexCover-instances"
set "resultsOutputDir=D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetGNMVertexCover"
for /r "%vertexCoverInstanceDir%" %%i in (*.mis) do (
"%exeLocation%" --algorithm="%algorithm%" --repetitions="%repetitions%" --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="%multicutInstanceDir%" --outputDir="%resultsOutputDir%" -v --instanceFilePath="%%i"
)
del mipMinSolSizeSolver.log
