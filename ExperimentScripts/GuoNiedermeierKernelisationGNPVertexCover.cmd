REM This file will create txt files with instances created from Vertex Cover instances generated from the G(n, p) model.
for /r D:\Documents\Universiteit\Thesis\GNPVertexCoverInstances\ %%i in (*.mis) do (
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=GuoNiedermeierKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\GNPVertexCoverMulticutInstances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierGNPVertexCover" -v --instanceFilePath="%%i"
)
pause
