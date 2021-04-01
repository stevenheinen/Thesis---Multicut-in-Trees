*This program was written between November 2020 and October 2021 by Steven Heinen ([e-mail](<mailto:s.a.heinen@uu.nl>)) within a final thesis project of the Computing Science master program at Utrecht University under supervision of J.M.M. van Rooij ([e-mail](<mailto:j.m.m.vanrooij@uu.nl>)).*

[![pipeline status](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/badges/master/pipeline.svg)](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/-/commits/master)
[![coverage report](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/badges/master/coverage.svg)](https://git.science.uu.nl/s.a.heinen/thesis-multicut-in-trees/-/commits/master)

# Thesis: A Practical Evaluation of Multicut in Trees
This is the project with the code for my Master Thesis about the Multicut in Trees problem.
The README is still a work in progres...

*Last update: 01-04-2021*

## Experiments
*WIP*

### Command line arguments
The program determines which experiment to run using command line arguments. The following can be used:

| Option                                | Explanantion |
|---------------------------------------|--------------|
| `--seed`                              | The seed to use for the random number generator that generates the random numbers for the experiments. Note: when there are multiple repetitions, the seeds used is this argument + the repetition (0-based). For example, with seed 0 and 5 repetitions, the experiments use seed 0, 1, 2, 3 and 4 respectively. |
| `--repetitions`                       | (Default: 1) The number of experiments to run with these settings. Each experiment starts with its own random number generator. Not necessary when running the branching algorithm. |
| `--algorithm`                         | **Required.** The algorithm to use in this experiment. Valid values: GuoNiederMeierBranching, GuoNiedermeierKernelisation, GurobiMIPSolver |
| `--tree`                              | **Required.** The way to generate the input tree in this experiment. Valid values: Prüfer, Caterpillar, VertexCover, CNFSAT, Fixed |
| `--dps`                               | **Required.** The way to generate the input demand pairs in this experiment. Valid values: Random, LengthDistribution, Fixed |
| `--nodes`                             | The number of nodes in this instance. Only necessary for Prüfer or Caterpillar input tree types. |
| `--instanceFile`                      | The path to the file with either the CNF-SAT or Vertex Cover instance, or the fixed tree. Only necessary for CNFSAT, VertexCover or Fixed input tree types. |
| `--dpFile`                            | The path to the file with the endpoints of the demand pairs. Only necessary for fixed input demand pairs. |
| `--demandpairs`                       | The number of demand pairs to generate. Only necessary for Prüfer or Caterpillar input trees. |
| <nobr>`--distanceDistribution`</nobr> | The distance distribution for the demand pairs. Only necessary for Prüfer or Caterpillar input trees and LengthDistribution input demand pair types. The argument should have any number of "values", separated by commas, with a value looking like: (min, max, percentage). For example: "(0, 5, 0.30), (2, 8, 0.25), (9, 350, 0.45)". |
| `--maxSolutionSize`                   | The maximum size the solution is allowed to be. |
| `--outputDir`                         | **Required.** The directory where the output of the experiments should be saved. |
| `--instanceDir`                       | **Required.** The directory where the instances are stored or should be stored. |
| `-v` or `--verbose`                   | Whether the program should run in verbose mode. |
| `--help`                              | Display this help screen. |
| `--version`                           | Display version information. |

### Saving results
*WIP*

## Algorithms
*WIP*

## Instances
*WIP*

### Trees
*WIP*

### Demand pairs
*WIP*

### Saving instances
*WIP*