#!/usr/bin/env python3

"""
NOTE: This script is compatible with:
    * .Net in Windows
    * Mono in Linux
"""

import sys
sys.path.append(r'D:\dev\src\python_shared_modules_git')

from pikuli.uia import Desktop


max_level = 400

def next_level(parent, indent_level=0):
    print_automation_element(parent, indent_level)

    if indent_level == max_level:
        return True

    children = parent.find_all(exact_level=1)

    max_level_reached = False
    for ch in children:
        mlr = next_level(ch, indent_level + 1)
        max_level_reached = max_level_reached or mlr

    return max_level_reached


def print_automation_element(element, indent_level):
    print("{indent}'{name}' - {locctrl} ({ctrl}) - {autoid}".format(
        indent = "  " * indent_level,
        name=element.Name,
        locctrl=element.LocalizedControlType,
        ctrl=element.ControlType,
        autoid=element.AutomationId))


def main():
    desktop = Desktop()

    max_level_reached = next_level(desktop)
    if max_level_reached:
        print(f"!!! next_level: max_level={max_level} reached")


if __name__ == '__main__':
    main()
