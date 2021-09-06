*This program was written between November 2020 and October 2021 by Steven Heinen ([e-mail](<mailto:s.a.heinen@uu.nl>)) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij ([e-mail](<mailto:j.m.m.vanrooij@uu.nl>)).*

[![pipeline status](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/badges/master/pipeline.svg)](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/-/commits/master)
[![coverage report](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/badges/master/coverage.svg)](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/-/commits/master)

# Thesis: A Practical Evaluation of Multicut in Trees
This is the project with the code for my Master Thesis about the Multicut in Trees problem.

*Last update: 06-09-2021*

## Experiments
This code can be used to compute kernels on Multicut in Trees instances.

### Command line arguments
The program determines which experiment to run using command line arguments. The following can be used:

| Option                                | Explanantion |
|---------------------------------------|--------------|
| <nobr>`--treeSeed`</nobr>             | The seed to use for the random number generator that generates the tree for the experiments. Note: when there are multiple experiments, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively. |
| <nobr>`--dpSeed`</nobr>               | The seed to use for the random number generator that generates the demand pairs for the experiments. Note: when there are multiple experiments, the seed used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seeds 0, 1, 2, 3 and 4 respectively. |
| <nobr>`--experiments`</nobr>          | (Default: 1) The number of experiments to run with these settings. Each experiment starts with its own random number generator, that is seeded as the seed argument plus the number of the current experiment. Only works for generated input. When input is read from a file, this option will be used as a multiplier for the number of repetitions per experiment. |
| <nobr>`--repetitions`</nobr>          | (Default: 1) The number of repetitions to run per experiment. |
| <nobr>`--algorithm`</nobr>            | **Required.** The algorithm to use in this experiment. Valid values: None, GenerateInstances, GurobiMIPSolver, BruteForce, GuoNiederMeierBranching, GuoNiedermeierKernelisation, BousquetKernelisation, ChenKernelisation |
| <nobr>`--treeType`</nobr>             | **Required.** The way to generate the input tree in this experiment. Valid values: None, Prufer, Caterpillar, VertexCover, CNFSAT, Fixed, Degree3Tree |
| <nobr>`--dpType`</nobr>               | **Required.** The way to generate the input demand pairs in this experiment. Valid values: None, Random, LengthDistribution, Fixed, FromTreeInstance, ThroughKnownSolution |
| <nobr>`--nrNodes`</nobr>              | The number of nodes in this instance. Only necessary for Prüfer or Caterpillar input tree types. |
| <nobr>`--instanceFilePath`</nobr>     | (Default: ) The path to the file with either the CNF-SAT or Vertex Cover instance, or the fixed tree. Only necessary for CNFSAT, VertexCover or Fixed input tree types. |
| <nobr>`--dpFile`</nobr>               | (Default: ) The path to the file with the endpoints of the demand pairs. Only necessary for fixed input demand pairs. |
| <nobr>`--nrDPs`</nobr>                | The number of demand pairs to generate. Only necessary for Prüfer or Caterpillar input trees. |
| <nobr>`--distanceDistribution`</nobr> | The distance distribution for the demand pairs. Only necessary for Prüfer or Caterpillar input trees and LengthDistribution input demand pair types. The argument should have any number of "values", separated by commas, with a value looking like: (min, max, percentage). For example: "(0, 5, 0.30), (2, 8, 0.25), (9, 350, 0.45)". |
| <nobr>`--maxSolutionSize`</nobr>      | The maximum size the solution is allowed to be. If this value is 0 or smaller, the optimal possible value will be used. |
| <nobr>`--outputDir`</nobr>            | **Required.** The directory where the output of the experiments should be saved. |
| <nobr>`--instanceDir`</nobr>          | **Required.** The directory where the instances are stored or should be stored. |
| <nobr>`--mipTimeLimit`</nobr>         | (Default: 21600) The maximum time that can be spent by the MIP solver to compute the optimal solution. |
| <nobr>`--overwrite`</nobr>            | Whether to overwrite existing instances if they already exist. |
| <nobr>`-v` or `--verbose`</nobr>      | Whether the program should run in verbose mode. |
| <nobr>`--help`</nobr>                 | Display this help screen. |
| <nobr>`--version`</nobr>              | Display version information. |

### Saving results
When the program is run using the Experiment configuration, the output of an experiment is saved to a csv-file in the location provided in the outputDir command line option.

## Algorithms
Currently, the following algorithms are implemented.
- GurobiMIPSolver: This algorithm can be used to compute the optimal solution for an instance. This algorithm will be used when generating instances (see later), and it requires a valid and licenced version of the Gurobi MIP solver: https://www.gurobi.com/
- GenerateInstances: With this algorithm, instances will be generated, but no kernel is computed. It will use the GurobiMIPSolver algorithm to find the size of the optimal solution.
- BruteForce: A simple O(2^k) algorithm that tries to find a solution of size k by checking every subset of k edges.
- GuoNiedermeierBranching: An O(2^k * n * p) branching algorithm. Here, k is the maximum size of the solution, n the number of nodes, and p the number of demand pairs. Source: https://doi.org/10.1002/net.20081
- GuoNiedermeierKernelisation: A kernelisation algorithm that finds a kernel of size O(k^{3k}), where k is the maximum size of the solution. Source: https://doi.org/10.1002/net.20081
- BousquetKernelistion: A kernelisation algorithm that finds a kernel of size O(k^6), where k is the maximum size of the solution. Source: http://arxiv.org/abs/0902.1047
- ChenKernelistion: A **partial** implementation of a kernelisation algorithm that finds a kernel of size O(k^3), where k is the maximum size of the solution. Source: https://doi.org/10.1016/j.jcss.2012.03.001

## Instances
Instances can be created using the GenerateInstances algorithm described above. If any other algorithm is run, but the instance corresponding to the command line arguments does not exist yet, the instance will first be generated.

### Trees
There are three different types of trees that can be used.
- Prüfer trees: These trees are generated using a uniform randomly generated Prüfer sequence.
- Caterpillars: Caterpillars do not contain an internal node that has three or more neighbours that are also internal nodes. They consist of a backbone and all leaves connected to that backbone. The length of the backbone is determined randomly using a beta distribution with alpha = 2 and beta = 8.
- Degree3 trees: These trees are comparable to a complete binary tree, except that the root node has three neighbours. This tree is made to have many nodes with a degree equal to three.

### Demand pairs
There are also three different types of demand pairs.
- Random demand pairs: Two distinct nodes are picked uniform randomly, and these nodes form a demand pair.
- Demand pairs using a length distribution: A length for a demand path is picked from a length distribution. Then, one possible path of the chosen length is picked in the tree, and the endpoints will form the demand pair. This method uses an all pairs shortest paths algorithm, so it might take a while to run on larger instances.
- Demand pairs through a known solution: k edges are picked uniform randomly. These edges will be the solution. Then, k demand paths are generated, such that they are edge-disjoint and each go through one of the solution edges. Then, the remaining demand pairs are generated by uniform randomly picking two distinct nodes.

### Other instances
Instances can also be generated from Vertex Cover and CNF-SAT. For these, use the FromTreeInstance option for demand pairs.
- Vertex Cover instances should be mis files with the following format:
> A brief description of the ASCII DIMACS graph format:
> There are 0 or more lines starting with c at the top of the file which are comment lines and can be ignored.
> Following the comment lines, there is a line with the form "p edge V E K" which specifies the size of the graph where V and E are the number of vertices and edges respectively. K is the size of the minimum vertex cover for this instance.
> The remaining of the file is a list of lines starting with e which indicate the edges in the graph (e.g. the line "e 1 3" indicates that there is an edge between vertex 1 and vertex 3).
- CNF-SAT files should be cnf files with the following format:
> DIMACS CNF
> The DIMACS CNF format is a textual representation of a formula in conjunctive normal form. A formula in conjunctive normal form is a conjunction (logical and) of a set of clauses. Each clause is a disjunction (logical or) of a set of literals. A literal is a variable or a negation of a variable. > DIMACS CNF uses positive integers to represent variables and their negation to represent the corresponding negated variable. This convention is also used for all textual input and output in Varisat.
> There are several variations and extensions of the DIMACS CNF format. Varisat tries to accept any variation commonly found. Currently no extensions are supported.
> DIMACS CNF is a textual format. Any line that begins with the character c is considered a comment. Some other parsers require comments to start with c and/or support comments only at the beginning of a file. Varisat supports them anywhere in the file.
> A DIMACS file begins with a header line of the form p cnf [variables] [clauses]. Where [variables] and [clauses] are replaced with decimal numbers indicating the number of variables and clauses in the formula.
> Varisat does not require a header line. If it is missing, it will infer the number of clauses and variables. If a header line is present, though, the formula must have the exact number of clauses and may not use variables represented by a number larger than indicated.
> Following the header line are the clauses of the formula. The clauses are encoded as a sequence of decimal numbers separated by spaces and newlines. For each clause the contained literals are listed followed by a 0. Usually each clause is listed on a separate line, using spaces between each of > the literals and the final zero. Sometimes long clauses use multiple lines. Varisat will accept any combination of spaces and newlines as separators, including multiple clauses on the same line.
> As an example the formula (x ∨ y ∨ ¬z) ∧ (¬y ∨ z) could be encoded as this:
> 
> p cnf 3 2<br/>
> 1 2 -3 0<br/>
> -2 3 0<br/>
> The simplified DIMACS CNF format used by the yearly SAT competitions is a subset of the format parsed by Varisat.

### Saving instances
The instanceDir command line option is the location where an instance should be saved when it is generated.
When running an experiment, the program will try to find the correct instance in this directory.
If is does not exist, it will be generated and placed here.