REM This file will use Guo and Niedermeier's kernelisation algorithm to solve instances.
REM Trees are generated using the Caterpillar method, and demand pairs are generated uniform randomly.
REM The used numbers of nodes are: 128, 256, 384, 512, 640, 768, 896, 1024.
REM The used numbers of demand pairs are: 128, 256, 384, 512, 640, 768, 896, 1024.
REM There are 10 different instances for each combination of nodes and demand pair.
REM Each instance will be repeated 10 times to compensate for difference in running time because of external factors.
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9911216 --dpSeed=2074946 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5900853 --dpSeed=3752524 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3068649 --dpSeed=1716587 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3257789 --dpSeed=1639679 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=455177 --dpSeed=2905769 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3573150 --dpSeed=2429967 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6799322 --dpSeed=2683822 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=2251385 --dpSeed=4866684 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=128 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6397247 --dpSeed=5378967 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8737361 --dpSeed=7271963 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8026615 --dpSeed=884212 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7630347 --dpSeed=7325796 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=399579 --dpSeed=1388717 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3614405 --dpSeed=383695 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6842659 --dpSeed=5593872 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9188363 --dpSeed=8443535 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=256 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8206882 --dpSeed=1442274 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4836915 --dpSeed=3273762 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6885152 --dpSeed=1504583 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7879217 --dpSeed=3239704 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8906428 --dpSeed=4476646 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3702955 --dpSeed=8935681 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9986419 --dpSeed=4383408 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3451650 --dpSeed=2262673 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=384 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9261496 --dpSeed=7229106 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4725263 --dpSeed=5578044 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7308209 --dpSeed=3001927 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4030100 --dpSeed=811360 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=1717233 --dpSeed=394273 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=1321306 --dpSeed=3523184 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8623114 --dpSeed=4708068 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5825566 --dpSeed=3546779 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=512 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6226203 --dpSeed=6731335 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8827783 --dpSeed=4680757 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4564483 --dpSeed=5707988 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8470900 --dpSeed=2463039 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4890778 --dpSeed=920050 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9429672 --dpSeed=4930421 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7713246 --dpSeed=3285865 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9996803 --dpSeed=7952509 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=640 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5326551 --dpSeed=4647287 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=695678 --dpSeed=2238021 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5401303 --dpSeed=4506879 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9943456 --dpSeed=8077037 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7042987 --dpSeed=6030174 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=4557526 --dpSeed=3968757 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=8726073 --dpSeed=5735598 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5282480 --dpSeed=3300487 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=768 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3534648 --dpSeed=5752084 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6860889 --dpSeed=5543575 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=455626 --dpSeed=9628043 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7917448 --dpSeed=6953908 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3280931 --dpSeed=9210553 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7866252 --dpSeed=215749 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5712679 --dpSeed=6140248 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=5855903 --dpSeed=4067377 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=896 --nrDPs=1024
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=7308894 --dpSeed=4923700 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=128
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=9687966 --dpSeed=8924564 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=256
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=6621604 --dpSeed=8419940 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=384
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=2845607 --dpSeed=5261673 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=512
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=2896488 --dpSeed=7706468 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=640
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3891876 --dpSeed=8315312 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=768
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=269378 --dpSeed=4805235 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=896
..\MulticutInTrees\bin\Experiment\net5.0\MulticutInTrees.exe --treeSeed=3292975 --dpSeed=4799979 --experiments=100 --repetitions=1 --algorithm=GuoNiedermeierKernelisation --treeType=Caterpillar --dpType=Random --instanceDir="D:\Documents\Universiteit\Thesis\Instances" --outputDir="D:\Documents\Universiteit\Thesis\ExperimentResults\GuoNiedermeierCaterpillarRandomSmall" -v --nrNodes=1024 --nrDPs=1024
pause