# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.
#
# A script that will gather the non-granular experimental stats for C# files within
# the current folder
#
# Works by opening up all .cs files in the current working directory, looking
# for code that lives in the namespace with the "Experimental" word, and then
# counting the number of lines and files that satisfy that.

import os
import re

log_details = False
namespace_regex = re.compile('^namespace.*\.Experimental')

class ExperimentalStats:
    def __init__(self, experimental = 0, non_experimental = 0, experimental_files = 0, non_experimental_files = 0):
        self.experimental = experimental
        self.non_experimental = non_experimental
        self.experimental_files = experimental_files
        self.non_experimental_files = non_experimental_files

    def merge(self, other):
        self.experimental += other.experimental
        self.non_experimental += other.non_experimental
        self.experimental_files += other.experimental_files
        self.non_experimental_files += other.non_experimental_files

def is_experimental_namespace(line: str) -> bool:
    return namespace_regex.match(line)

def is_csharp_file(filename: str) -> bool:
    return filename.endswith('.cs')

def log(line: str):
    if log_details:
        print(line)

def get_experimental_stats(filename: str) -> ExperimentalStats:
    is_experimental = False
    line_count = 0
    log(filename)
    with open(filename, encoding="utf8", errors="ignore") as f:
        for line in f:
            if is_experimental_namespace(line):
                is_experimental = True
                log(line)
            line_count+=1
    log("Experimental: {0}, Line Count: {1}".format(is_experimental, line_count))

    stats = ExperimentalStats()
    if is_experimental:
        stats.experimental = line_count
        stats.experimental_files += 1
    else:
        stats.non_experimental = line_count
        stats.non_experimental_files += 1
    return stats

experimental_stats = ExperimentalStats()

for root, dirs, files in os.walk('.'):
    for name in files:
        if is_csharp_file(name):
            experimental_stats.merge(get_experimental_stats(os.path.join(root, name)))
        
print('Experimental Stats')
print('Experimental files: {0}'.format(experimental_stats.experimental_files))
print('Experimental lines: {0}'.format(experimental_stats.experimental))
print('Non-Experimental files: {0}'.format(experimental_stats.non_experimental_files))
print('Non-Experimental lines: {0}'.format(experimental_stats.non_experimental))
print('Relative percent of experimental to total (by lines): {:.2%}'.format(
    experimental_stats.experimental / (experimental_stats.experimental + experimental_stats.non_experimental)))