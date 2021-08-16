import matplotlib.pyplot as plot
import numpy as np
import os

def read_file(filename: str) -> dict:
    length_occurrences = {}
    with open(os.path.join(os.path.dirname(os.path.realpath(__file__)), filename)) as file:
        for line in file:
            split = line.split()
            key = int(split[0])
            value = int(split[1])
            length_occurrences[key] = value
    return length_occurrences

def plot_histogram(title: str, output_name: str, length_occurrences: dict) -> None:
    fig, ax = plot.subplots(figsize=(10, 6))
    plot.ticklabel_format(axis="y", style="plain")
    ax.xaxis.get_major_locator().set_params(integer=True)
    ax.set_title(title, wrap=True)
    ax.set_xlabel("Length of the paths", wrap=True)
    ax.set_ylabel("Number of occurrences of each path length", wrap=True)
    # bins = len([0 for key, value in length_occurrences.items() if value != 0])
    # hist_values = [key for key, value in length_occurrences.items() for _ in range(value)]
    # ax.hist(hist_values, bins)
    ax.bar([key for key in length_occurrences.keys() if length_occurrences[key] != 0], [value for value in length_occurrences.values() if value != 0])
    fig.savefig(output_name + ".png")
    fig.savefig(output_name + ".pdf")
    plot.close(fig)

def make_plot(filename: str, title: str, output_name: str) -> None:
    length_occurrences = read_file(filename)
    plot_histogram(title, output_name, length_occurrences)

trees = ["Caterpillar", "Prufer", "Degree3Tree"]
nodes = ["128", "256", "384", "512", "640", "768", "896", "1024", "2048", "3072", "4096", "5120", "6144", "7168", "8192", "9216" , "10240"]
for tree in trees:
    for number_of_nodes in nodes:
        filename = tree + number_of_nodes + "nodes.txt"
        title = "Histogram with all path lengths in all generated " + tree + "s with " + number_of_nodes + " nodes."
        output_name = "histogram" + tree + number_of_nodes + "Nodes"
        make_plot(filename, title, output_name)