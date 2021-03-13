#!/usr/bin/env python3

"""
NOTE: This script is compatible with:
    * .Net in Windows
    * Mono in Linux
"""

import datetime
import re
import sys
import time

# Durty hack to help `pythonnet` module locate UIA assemblies:
if sys.platform in ('win32', 'cygwin'):
    sys.path.append(r"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\UIAutomationClient\v4.0_4.0.0.0__31bf3856ad364e35")
    sys.path.append(r"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\UIAutomationProvider\v4.0_4.0.0.0__31bf3856ad364e35")

import clr
clr.AddReference("System")
clr.AddReference("UIAutomationClient")  # System.Windows.Automation
clr.AddReference("UIAutomationTypes")  # System.Windows.Automation

from System import Console
from System.Windows.Automation import AutomationElement, TreeScope, TreeWalker, Condition


maxLevel = 400

RegexRemoveNewline = re.compile("[\r\n]+")


def NextLevel(parent, indentLevel=0, maxLevel=-1):
    PrintAutomationElement(parent, indentLevel)

    if indentLevel == maxLevel:
        return True

    children = parent.FindAll(TreeScope.Children, Condition.TrueCondition)
    maxLevelReached = False
    for ch in children:
        b = NextLevel(ch, indentLevel + 1, maxLevel)
        maxLevelReached = maxLevelReached or b

    return maxLevelReached


def PrintAutomationElement(element, indentLevel):
    try:
        name = str(element.Current.Name)
    except Exception as e:
        name = repr(element.Current.Name)
    print("{indent}'{name}' - {locctrl} ({ctrl}) - {autoid}".format(
        indent = "  " * indentLevel,
        name=RegexRemoveNewline.sub(" ", name),
        locctrl=element.Current.ControlType.LocalizedControlType,
        ctrl=element.Current.ControlType.Id,
        autoid=element.Current.AutomationId))


def main():
    Console.WriteLine("Hello world from CPython + .Net!")
    maxLevelReached = NextLevel(AutomationElement.RootElement, maxLevel=maxLevel)
    if maxLevelReached:
        print(f"!!! NextLevel: maxLevel={maxLevel} reached")

    
if __name__ == "__main__":
    main()