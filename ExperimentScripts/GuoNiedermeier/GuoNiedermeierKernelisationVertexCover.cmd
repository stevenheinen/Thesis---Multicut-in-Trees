REM This file will run the kernelisation algorithm by Guo and Niedermeier on instances created from Vertex Cover instances.
for %%i in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb30-15-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%i"
)
for %%j in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb35-17-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%j"
)
for %%k in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb40-19-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%k"
)
for %%l in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb45-21-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%l"
)
for %%m in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb50-23-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%m"
)
for %%n in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb53-24-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%n"
)
for %%o in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb56-25-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%o"
)
for %%p in (D:\Documents\Universiteit\Thesis\VertexCoverInstances\frb59-26-mis\*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\VertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierVertexCover" -v --instanceFilePath="%%p"
)
del mipMinSolSizeSolver.log
pause
