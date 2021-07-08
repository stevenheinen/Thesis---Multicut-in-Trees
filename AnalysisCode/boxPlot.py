from functools import reduce
import numpy as np
from numpy.lib.shape_base import apply_along_axis
import pandas as pd
import matplotlib.pyplot as plot
from pandas.core.frame import DataFrame
import seaborn as sns
import csv
import os

def read_csv_files_in_folder(folder: str, algorithm: str, nodes: set, dps: set, remaining_nodes: dict, remaining_dps: dict, remaining_k: dict, original_k: dict, reduction_rule_names: list, applicability_ticks: dict, modifying_ticks: dict, throughKnownSolution: bool, ks: list) -> tuple:
    first_file = True
    titles = []
    short_titles = []
    for file in os.listdir(folder):
        if not file.endswith(".csv"):
            continue
        name_indices = []
        applicability_indices = []
        modifying_indices = []
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
                        title = "tree with " + row[0] + " nodes generated with the " + row[2] + " method, demand pairs generated using the " + row[3] + " method, and the " + algorithm + " algorithm."
                        if algorithm == "Guo and Niedermeier":
                            short_title = "GuoNiedermeier" + row[2] + row[0] + "nodes" + row[3] + "DPs"
                        else:
                            short_title = algorithm + row[2] + row[0] + "nodes" + row[3] + "DPs"
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
                    if not applicability_ticks.__contains__(key):
                        applicability_ticks[key] = dict()
                        for i in range(len(applicability_indices)):
                            applicability_ticks[key][i] = []
                    if not modifying_ticks.__contains__(key):
                        modifying_ticks[key] = dict()
                        for i in range(len(applicability_indices)):
                            modifying_ticks[key][i] = []
                    remaining_nodes[key].append(int(row[10]))
                    remaining_dps[key].append(int(row[11]))
                    remaining_k[key].append(int(row[12]))
                    original_k[key].append(int(row[8]))
                    for i in range(len(applicability_indices)):
                        applicability_ticks[key][i].append(int(row[applicability_indices[i]]))
                        modifying_ticks[key][i].append(int(row[modifying_indices[i]]))
            title += " Average over " + str(line_count - 1) + " experiments."
            titles.append(title)
            short_titles.append(short_title)
    return (titles[::len(dps)], short_titles[::len(dps)])

def draw_kernel_size_plots(titles: list, short_titles: list, nodes: set, dps: set, remaining_nodes: dict, remaining_dps: dict) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    for titleIndex, node in enumerate(nodes):
        fig, ax = plot.subplots(figsize=(10, 6))
        ax.set_title("Remaining number of nodes and demand pairs in the kernel for a " + titles[titleIndex], wrap=True)
        ax.set_xticklabels(dps)
        node_data = []
        dp_data = []
        cols = []
        for dp in dps:
            for _ in range(len(remaining_nodes[(node, dp)])):
                cols.append(dp)
            for i, dp_result in enumerate(remaining_dps[(node, dp)]):
                dp_data.append(dp_result)
                if dp_result == 0:
                    node_data.append(0)
                else:
                    node_data.append(remaining_nodes[(node, dp)][i])
        x_axis_name = "Number of demand pairs in the original instance"
        y_axis_name = "Number of nodes and demand pairs in the kernel"
        legend_name = "Legend"
        remaining_nodes_var_name = "Remaining nodes"
        remaining_dps_var_name = "Remaining demand pairs"
        df = pd.DataFrame({remaining_nodes_var_name: node_data, remaining_dps_var_name: dp_data, x_axis_name: cols})
        df_plot = df.melt(id_vars=x_axis_name, value_vars=[remaining_nodes_var_name, remaining_dps_var_name], var_name=legend_name, value_name=y_axis_name)
        sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
        title = "remainingNodesDPs" + short_titles[titleIndex]
        fig.savefig(title + ".png")
        fig.savefig(title + ".pdf")
        plot.close(fig)

def draw_kernel_size_plots_through_known_solution(titles: list, short_titles: list, nodes: set, dps: set, remaining_nodes: dict, remaining_dps: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_experiments = int(len(remaining_nodes[(nodes[0], dps[0])]) / different_ks)
    for k in range(different_ks):
        for titleIndex, node in enumerate(nodes):
            fig, ax = plot.subplots(figsize=(10, 6))
            ax.set_title("Remaining number of nodes and demand pairs in the kernel for a " + titles[k * len(nodes) + titleIndex], wrap=True)
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
            title = "remainingNodesDPs" + short_titles[titleIndex]
            fig.savefig(title + ".png", wrap=True)
            fig.savefig(title + ".pdf", wrap=True)
            plot.close(fig)

def draw_time_plots_applicability(titles: list, short_titles: list, nodes: set, dps: set, reduction_rules_names: list, applicability_ticks: dict) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    for titleIndex, node in enumerate(nodes):
        fig, ax = plot.subplots(figsize=(22, 10))
        ax.set_title("Number of operations per reduction rule checking applicability for a " + titles[titleIndex], wrap=True)
        plot.ticklabel_format(axis="y", style="plain")
        ax.set_xticklabels(dps)
        applicability_ticks_data = []
        cols = []
        for dp in dps:
            for _ in range(len(applicability_ticks[(node, dp)][0])):
                cols.append(dp)
        for i in range(number_of_reduction_rules):
            applicability_ticks_data.append([])
            for dp in dps:
                applicability_ticks_data[i].append(applicability_ticks[(node, dp)][i])
            applicability_ticks_data[i] = [item for sublist in applicability_ticks_data[i] for item in sublist]
        x_axis_name = "Number of demand pairs in the original instance"
        y_axis_name = "Number of operations"
        legend_name = "Legend"
        legend = []
        data_frame_dict = {x_axis_name: cols}
        for i in range(number_of_reduction_rules):
            name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
            legend.append(name)
            data_frame_dict[name] = applicability_ticks_data[i]
        df = pd.DataFrame(data_frame_dict)
        df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
        sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
        title = "applicabilityOperations" + short_titles[titleIndex]
        fig.savefig(title + ".png", wrap=True)
        fig.savefig(title + ".pdf", wrap=True)
        plot.close(fig)

def draw_time_plots_applicability_through_known_solution(titles: list, short_titles: list, nodes: set, dps: set, reduction_rules_names: list, applicability_ticks: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    number_of_experiments = int(len(applicability_ticks[(nodes[0], dps[0])][0]) / different_ks)
    for k in range(different_ks):
        for titleIndex, node in enumerate(nodes):
            fig, ax = plot.subplots(figsize=(22, 10))
            ax.set_title("Number of operations per reduction rule checking applicability for a " + titles[k * len(nodes) + titleIndex], wrap=True)
            plot.ticklabel_format(axis="y", style="plain")
            ax.set_xticklabels(dps)
            applicability_ticks_data = []
            cols = []
            for dp in dps:
                for _ in range(number_of_experiments):
                    cols.append(dp)
            for i in range(number_of_reduction_rules):
                applicability_ticks_data.append([])
                for dp in dps:
                    applicability_ticks_data[i].append(applicability_ticks[(node, dp)][i][k * number_of_experiments : (k + 1) * number_of_experiments])
                applicability_ticks_data[i] = [item for sublist in applicability_ticks_data[i] for item in sublist]
            x_axis_name = "Number of demand pairs in the original instance"
            y_axis_name = "Number of operations"
            legend_name = "Legend"
            legend = []
            data_frame_dict = {x_axis_name: cols}
            for i in range(number_of_reduction_rules):
                name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
                legend.append(name)
                data_frame_dict[name] = applicability_ticks_data[i]
            df = pd.DataFrame(data_frame_dict)
            df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
            sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
            title = "applicabilityOperations" + short_titles[titleIndex]
            fig.savefig(title + ".png", wrap=True)
            fig.savefig(title + ".pdf", wrap=True)
            plot.close(fig)

def draw_time_plots_modifying(titles: list, short_titles: list, nodes: set, dps: set, reduction_rules_names: list, modifying_ticks: dict) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    for titleIndex, node in enumerate(nodes):
        fig, ax = plot.subplots(figsize=(22, 10))
        ax.set_title("Number of operations per reduction rule to modify the instance for a " + titles[titleIndex], wrap=True)
        plot.ticklabel_format(axis="y", style="plain")
        ax.set_xticklabels(dps)
        modifying_ticks_data = []
        cols = []
        for dp in dps:
            for _ in range(len(modifying_ticks[(node, dp)][0])):
                cols.append(dp)
        for i in range(number_of_reduction_rules):
            modifying_ticks_data.append([])
            for dp in dps:
                modifying_ticks_data[i].append(modifying_ticks[(node, dp)][i])
            modifying_ticks_data[i] = [item for sublist in modifying_ticks_data[i] for item in sublist]
        x_axis_name = "Number of demand pairs in the original instance"
        y_axis_name = "Number of operations"
        legend_name = "Legend"
        legend = []
        data_frame_dict = {x_axis_name: cols}
        for i in range(number_of_reduction_rules):
            name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
            legend.append(name)
            data_frame_dict[name] = modifying_ticks_data[i]
        df = pd.DataFrame(data_frame_dict)
        df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
        sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
        title = "modifyingOperations" + short_titles[titleIndex]
        fig.savefig(title + ".png", wrap=True)
        fig.savefig(title + ".pdf", wrap=True)
        plot.close(fig)

def draw_time_plots_modifying_through_known_solution(titles: list, short_titles: list, nodes: set, dps: set, reduction_rules_names: list, modifying_ticks: dict, different_ks: int) -> None:
    nodes = sorted(nodes)
    dps = sorted(dps)
    number_of_reduction_rules = len(reduction_rules_names)
    number_of_experiments = int(len(modifying_ticks[(nodes[0], dps[0])][0]) / different_ks)
    for k in range(different_ks):
        for titleIndex, node in enumerate(nodes):
            fig, ax = plot.subplots(figsize=(22, 10))
            ax.set_title("Number of operations per reduction rule to modify the instance for a " + titles[k * len(nodes) + titleIndex], wrap=True)
            plot.ticklabel_format(axis="y", style="plain")
            ax.set_xticklabels(dps)
            modifying_ticks_data = []
            cols = []
            for dp in dps:
                for _ in range(number_of_experiments):
                    cols.append(dp)
            for i in range(number_of_reduction_rules):
                modifying_ticks_data.append([])
                for dp in dps:
                    modifying_ticks_data[i].append(modifying_ticks[(node, dp)][i][k * number_of_experiments : (k + 1) * number_of_experiments])
                modifying_ticks_data[i] = [item for sublist in modifying_ticks_data[i] for item in sublist]
            x_axis_name = "Number of demand pairs in the original instance"
            y_axis_name = "Number of operations"
            legend_name = "Legend"
            legend = []
            data_frame_dict = {x_axis_name: cols}
            for i in range(number_of_reduction_rules):
                name = "Reduction rule " + str(i + 1) + " (" + reduction_rules_names[i] + ")"
                legend.append(name)
                data_frame_dict[name] = modifying_ticks_data[i]
            df = pd.DataFrame(data_frame_dict)
            df_plot = df.melt(id_vars=x_axis_name, value_vars=legend, var_name=legend_name, value_name=y_axis_name)
            sns.boxplot(x=x_axis_name, y=y_axis_name, hue=legend_name, data=df_plot)
            title = "modifyingOperations" + short_titles[titleIndex]
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
    applicability_ticks = {}
    modifying_ticks = {}
    titles, short_titles = read_csv_files_in_folder(folder, algorithm, nodes, dps, remaining_nodes, remaining_dps, remaining_k, original_k, reduction_rules_names, applicability_ticks, modifying_ticks, throughKnownSolution, [2, 4, 6, 8, 10])
    if throughKnownSolution:
        draw_kernel_size_plots_through_known_solution(titles, short_titles, nodes, dps, remaining_nodes, remaining_dps, 5)
        draw_time_plots_applicability_through_known_solution(titles, short_titles, nodes, dps, reduction_rules_names, applicability_ticks, 5)
        draw_time_plots_modifying_through_known_solution(titles, short_titles, nodes, dps, reduction_rules_names, modifying_ticks, 5)
    else:
        draw_kernel_size_plots(titles, short_titles, nodes, dps, remaining_nodes, remaining_dps)
        draw_time_plots_applicability(titles, short_titles, nodes, dps, reduction_rules_names, applicability_ticks)
        draw_time_plots_modifying(titles, short_titles, nodes, dps, reduction_rules_names, modifying_ticks)

make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferRandomSmall", "Guo and Niedermeier", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferThroughKnownSolutionSmall", "Guo and Niedermeier", True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierPruferRandomLarge", "Guo and Niedermeier", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarRandomSmall", "Guo and Niedermeier", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierCaterpillarThroughKnownSolutionSmall", "Guo and Niedermeier", True)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeRandomSmall", "Guo and Niedermeier", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\GuoNiedermeierDegree3TreeThroughKnownSolutionSmall", "Guo and Niedermeier", True)

make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferRandomSmall", "Bousquet", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferThroughKnownSolutionSmall", "Bousquet", True)
# make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetPruferRandomLarge", "Bousquet", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarRandomSmall", "Bousquet", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetCaterpillarThroughKnownSolutionSmall", "Bousquet", True)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeRandomSmall", "Bousquet", False)
make_plots("D:\\Documents\\Universiteit\\Thesis\\ExperimentResults\\BousquetDegree3TreeThroughKnownSolutionSmall", "Bousquet", True)
