from functools import reduce
import numpy as np
from numpy.lib.shape_base import apply_along_axis
import pandas as pd
import matplotlib.pyplot as plot
from pandas.core.frame import DataFrame
import seaborn as sns
import csv
import os

def read_csv_files_in_folder(folder: str, algorithm: str, nodes: set, dps: set, remaining_nodes: dict, remaining_dps: dict, remaining_k: dict, original_k: dict, reduction_rule_names: list, ticks: dict, operations: dict, throughKnownSolution: bool, ks: list) -> tuple:
    first_file = True
    titles = {}
    short_titles = {}
    current_nodes = -1
    for file in os.listdir(folder):
        if not file.endswith(".csv"):
            continue
        name_indices = []
        applicability_indices = []
        modifying_indices = []
        tree_counter_indices = []
        dp_counter_indices = []
        key_counter_indices = []
        value_counter_indices = []
        with open(os.path.join(folder, file)) as csv_file:
            csv_reader = csv.reader(csv_file, delimiter=',')
            line_count = 0
            for row in csv_reader:
                line_count += 1
                if line_count == 1:
                    i = 1
                    name_template = "RR" + str(i) + "Name"
                    template = "RR" + str(i) + "TicksApplicability"
                    while row.__contains__(template):
                        name_indices.append(row.index(name_template))
                        index = row.index(template)
                        applicability_indices.append(index)
                        modifying_indices.append(index + 1)
                        tree_counter_indices.append(index - 4)
                        dp_counter_indices.append(index - 3)
                        key_counter_indices.append(index - 2)
                        value_counter_indices.append(index - 1)
                        i += 1
                        name_template = "RR" + str(i) + "Name"
                        template = "RR" + str(i) + "TicksApplicability"
                    continue
                else:
                    if row[9].lower() != "true":
                        raise Exception("There is an instance that was marked as not solvable! In the \"solvable\" field it has value " + row[9])
                    if line_count == 2:
                        if first_file:
                            first_file = False
                            for i in range(len(name_indices)):
                                reduction_rule_names.append(row[name_indices[i]])
                        current_nodes = int(row[0])
                        tree_type = row[2]
                        if tree_type == "Prufer":
                            tree_type = "Prüfer"
                        title = "tree with " + row[0] + " nodes generated with the " + tree_type + " method, demand pairs generated using the " + row[3] + " method, and the " + algorithm + " algorithm."
                        if algorithm == "Guo and Niedermeier":
                            short_title = "GuoNiedermeier" + row[2] + row[0] + "nodes" + row[3] + "DPs"
                        elif algorithm == "Bousquet et al.":
                            short_title = "Bousquet" + row[2] + row[0] + "nodes" + row[3] + "DPs"
                        else:
                            raise Exception("Unknown algorithm!")
                        if throughKnownSolution:
                            sol_size = next(k for k in ks if k >= int(row[8]))
                            title += " The optimal solution size is at most " + str(sol_size) + "."
                            short_title += "(k=" + str(sol_size) + ")"
                    nodes.add(int(row[0]))
                    dps.add(int(row[1]))
                    key = (int(row[0]), int(row[1]))                   
                    if not remaining_nodes.__contains__(key):
                        remaining_nodes[key] = []
                    if not remaining_dps.__contains__(key):
                        remaining_dps[key] = []
                    if not remaining_k.__contains__(key):
                        remaining_k[key] = []
                    if not original_k.__contains__(key):
                        original_k[key] = []
                    if not ticks.__contains__(key):
                        ticks[key] = dict()
                        for i in range(len(applicability_indices)):
                            ticks[key][i] = []
                    if not operations.__contains__(key):
                        operations[key] = dict()
                        for i in range(len(applicability_indices)):
                            operations[key][i] = []
                    remaining_nodes[key].append(int(row[10]))
                    remaining_dps[key].append(int(row[11]))
                    remaining_k[key].append(int(row[12]))
                    original_k[key].append(int(row[8]))
                    for i in range(len(applicability_indices)):
                        applicability_ticks = int(row[applicability_indices[i]])
                        modifying_ticks = int(row[modifying_indices[i]])
                        ticks[key][i].append(applicability_ticks + modifying_ticks)
                        tree_ops = int(row[tree_counter_indices[i]])
                        dp_ops = int(row[dp_counter_indices[i]])
                        key_ops = int(row[key_counter_indices[i]])
                        value_ops = int(row[value_counter_indices[i]])
                        operations[key][i].append(tree_ops + dp_ops + key_ops + value_ops)
            titles[current_nodes] = []
            titles[current_nodes].append(title)
            short_titles[current_nodes] = []
            short_titles[current_nodes].append(short_title)
    return (titles, short_titles)

def draw_kernel_size_plots(titles: dict(), short_titles: dict(), nodes: set, dps: set, remaining_nodes: dict, remaining_dps: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_experiments = int(len(remaining_nodes[(nodes[0], dps[0])]) / different_ks)
    for k in range(different_ks):
        for node in nodes:
            fig, ax = plot.subplots(figsize=(10, 6))
            ax.set_title("Remaining number of nodes and demand pairs in the kernel for a " + titles[node][k] + " Average over " + str(number_of_experiments) + " experiments.", wrap=True)
            ax.set_xticklabels(dps)
            node_data = []
            dp_data = []
            cols = []
            for dp in dps:
                for _ in range(number_of_experiments):
                    cols.append(dp)
                for i, dp_result in enumerate(remaining_dps[(node, dp)][k * number_of_experiments : (k + 1) * number_of_experiments]):
                    dp_data.append(dp_result)
                    if dp_result == 0:
                        node_data.append(0)
                    else:
                        node_data.append(remaining_nodes[(node, dp)][k * number_of_experiments : (k + 1) * number_of_experiments][i])
            x_axis_name = "Number of demand pairs in the original instance"
            y_axis_name = "Number of nodes and demand pairs in the kernel"
            legend_name = "Legend"
            remaining_nodes_var_name = "Remaining nodes"
            remaining_dps_var_name = "Remaining demand pairs"
            df = pd.DataFrame({remaining_nodes_var_name: node_data, remaining_dps_var_name: dp_data, x_axis_name: cols})
            df_plot = df.melt(id_vars=x_axis_name, value_vars=[remaining_nodes_var_name, remaining_dps_var_name], var_name=legend_name, value_name=y_axis_name)
            sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
            title = "remainingNodesDPs" + short_titles[node][k]
            fig.savefig(title + ".png")
            fig.savefig(title + ".pdf")
            plot.close(fig)

def draw_time_plots(titles: dict(), short_titles: dict(), nodes: set, dps: set, reduction_rules_names: list, ticks: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    number_of_experiments = int(len(ticks[(nodes[0], dps[0])][0]) / different_ks)
    for k in range(different_ks):
        for node in nodes:
            fig, ax = plot.subplots(figsize=(22, 10))
            ax.set_title("Number of ticks per reduction rule for a " + titles[node][k] + " Average over " + str(number_of_experiments) + " experiments.", wrap=True)
            plot.ticklabel_format(axis="y", style="plain")
            ax.set_xticklabels(dps)
            ticks_data = []
            cols = []
            for dp in dps:
                for _ in range(number_of_experiments):
                    cols.append(dp)
            for i in range(number_of_reduction_rules):
                ticks_data.append([])
                for dp in dps:
                    ticks_data[i].append(ticks[(node, dp)][i][k * number_of_experiments : (k + 1) * number_of_experiments])
                ticks_data[i] = [item for sublist in ticks_data[i] for item in sublist]
            x_axis_name = "Number of demand pairs in the original instance"
            y_axis_name = "Number of ticks"
            legend_name = "Legend"
            legend = []
            data_frame_dict = {x_axis_name: cols}
            for i in range(number_of_reduction_rules):
                name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
                legend.append(name)
                data_frame_dict[name] = ticks_data[i]
            df = pd.DataFrame(data_frame_dict)
            df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
            sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
            title = "ticks" + short_titles[node][k]
            fig.savefig(title + ".png")
            fig.savefig(title + ".pdf")
            plot.close(fig)

def draw_operations_plots(titles: dict(), short_titles: dict(), nodes: set, dps: set, reduction_rules_names: list, operations: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    number_of_experiments = int(len(operations[(nodes[0], dps[0])][0]) / different_ks)
    for k in range(different_ks):
        for node in nodes:
            fig, ax = plot.subplots(figsize=(22, 10))
            ax.set_title("Number of operations per reduction rule for a " + titles[node][k] + " Average over " + str(number_of_experiments) + " experiments.", wrap=True)
            plot.ticklabel_format(axis="y", style="plain")
            ax.set_xticklabels(dps)
            operations_data = []
            cols = []
            for dp in dps:
                for _ in range(number_of_experiments):
                    cols.append(dp)
            for i in range(number_of_reduction_rules):
                operations_data.append([])
                for dp in dps:
                    operations_data[i].append(operations[(node, dp)][i][k * number_of_experiments : (k + 1) * number_of_experiments])
                operations_data[i] = [item for sublist in operations_data[i] for item in sublist]
            x_axis_name = "Number of demand pairs in the original instance"
            y_axis_name = "Number of operations"
            legend_name = "Legend"
            legend = []
            data_frame_dict = {x_axis_name: cols}
            for i in range(number_of_reduction_rules):
                name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
                legend.append(name)
                data_frame_dict[name] = operations_data[i]
            df = pd.DataFrame(data_frame_dict)
            df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
            sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
            title = "operations" + short_titles[node][k]
            fig.savefig(title + ".png", wrap=True)
            fig.savefig(title + ".pdf", wrap=True)
            plot.close(fig)

def make_plots(folder: str, algorithm: str, throughKnownSolution: bool) -> None:
    nodes = set()
    dps = set()
    remaining_nodes = {}
    remaining_dps = {}
    remaining_k = {}
    original_k = {}
    reduction_rules_names = []
    ticks = {}
    operations = {}
    titles, short_titles = read_csv_files_in_folder(folder, algorithm, nodes, dps, remaining_nodes, remaining_dps, remaining_k, original_k, reduction_rules_names, ticks, operations, throughKnownSolution, [5, 10, 20, 50, 100])
    if throughKnownSolution:
        draw_kernel_size_plots(titles, short_titles, nodes, dps, remaining_nodes, remaining_dps, 5)
        draw_kernel_size_plots(titles, short_titles, nodes, dps, reduction_rules_names, ticks, 5)
        draw_kernel_size_plots(titles, short_titles, nodes, dps, reduction_rules_names, operations, 5)
    else:
        draw_kernel_size_plots(titles, short_titles, nodes, dps, remaining_nodes, remaining_dps, 1)
        draw_kernel_size_plots(titles, short_titles, nodes, dps, reduction_rules_names, ticks, 1)
        draw_kernel_size_plots(titles, short_titles, nodes, dps, reduction_rules_names, operations, 1)


guo_niedermeier_algorithm_name = "Guo and Niedermeier"
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarLengthDistributionSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarLengthDistributionLarge", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarRandomLarge", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarRandomSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarThroughKnownSolutionLarge", guo_niedermeier_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarThroughKnownSolutionSmall", guo_niedermeier_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeLengthDistributionSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeLengthDistributionLarge", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeRandomLarge", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeRandomSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeThroughKnownSolutionLarge", guo_niedermeier_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeThroughKnownSolutionSmall", guo_niedermeier_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierGNPVertexCover", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferLengthDistributionSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferLengthDistributionLarge", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferRandomLarge", guo_niedermeier_algorithm_name, False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferRandomSmall", guo_niedermeier_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferThroughKnownSolutionLarge", guo_niedermeier_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferThroughKnownSolutionSmall", guo_niedermeier_algorithm_name, True)

bousquet_algorithm_name = "Bousquet et al."
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarLengthDistributionLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarLengthDistributionSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarRandomLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarRandomSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarThroughKnownSolutionLarge", bousquet_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarThroughKnownSolutionSmall", bousquet_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeLengthDistributionSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeLengthDistributionLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeRandomLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeRandomSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeThroughKnownSolutionLarge", bousquet_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeThroughKnownSolutionSmall", bousquet_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetGNPVertexCover", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferLengthDistributionSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferLengthDistributionLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferRandomLarge", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferRandomSmall", bousquet_algorithm_name, False)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferThroughKnownSolutionLarge", bousquet_algorithm_name, True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferThroughKnownSolutionSmall", bousquet_algorithm_name, True)

# TODO:
# - All experiments need to be rerun...
# - I am not sure whether the current method works for the GNPVertexCover instances
# - Only after the experiments have been rerun, create the new plots.
# - We still need to find a way to show the results on the 3SAT, CNF-SAT, and VertexCover (non-GNP version) results