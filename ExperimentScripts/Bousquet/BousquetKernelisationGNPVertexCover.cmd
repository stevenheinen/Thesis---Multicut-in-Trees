REM This file will run Bousquet et al.'s algorithm on instances created from Vertex Cover instances generated from the G(n, p) model.
for /r D:\Documents\Universiteit\Thesis\GNPVertexCoverInstances\ %%i in (*.mis) do (
..\..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --algorithm=BousquetKernelisation --treeType=VertexCover --dpType=FromTreeInstance --instanceDir="D:\Documents\Universiteit\Thesis\Instances\GNPVertexCover-instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\BousquetGNPVertexCover" -v --instanceFilePath="%%i"
)
pause
