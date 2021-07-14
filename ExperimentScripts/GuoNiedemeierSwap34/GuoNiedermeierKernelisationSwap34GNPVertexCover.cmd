REM This file will run Guo and Niedermeier's algorithm with reduction rules 3 and 4 swapped on instances created from Vertex Cover instances generated from the G(n, p) model.
for /r D:\Documents\Universiteit\Thesis\GNPVertexCoverInstances\ %%i in (*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisationSwap34 --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\GNPVertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierSwap34GNPVertexCover" -v --instanceFilePath="%%i"
)
pause
