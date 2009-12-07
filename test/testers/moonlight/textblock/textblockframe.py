
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/22/2009
# Description: textblock.py wrapper script
#              Used by the textblock-*.py tests
##############################################################################

# imports

from strongwind import *
from textblock import *
import re

# class to represent the main window.
class TextBlockFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TEXT_ONE = "simply TextBlock"
    TEXT_TWO = "^sample with LinkBreak and Run"

    def __init__(self, accessible):
        super(TextBlockFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("TextBlockSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        self.text1 = self.filler.findLabel(self.TEXT_ONE)
        self.text2 = self.filler.findLabel(re.compile(self.TEXT_TWO))
