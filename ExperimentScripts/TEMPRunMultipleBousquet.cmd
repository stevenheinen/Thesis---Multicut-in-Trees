for %%k in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb40-19-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%k"
)
for %%i in (D:\Documents\Universiteit\Thesis\CNF-SAT-instances\*.cnf) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\CNF-SAT-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetCNFSAT" -v --instanceFilePath="%%i"
)
for /r D:\Documents\Universiteit\Thesis\3SAT-instances\ %%i in (*.cnf) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=CNFSAT --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\3SAT-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\Bousquet3SAT" -v --instanceFilePath="%%i"
)
for %%l in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb45-21-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%l"
)
for %%m in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb50-23-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%m"
)
for %%n in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb53-24-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%n"
)
for %%o in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb56-25-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%o"
)
for %%p in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb59-26-mis\*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetVertexCover" -v --instanceFilePath="%%p"
)
del mipMinSolSizeSolver.log
pause