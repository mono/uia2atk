
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/22/2009
# Description: textblock.py wrapper script
#              Used by the textblock-*.py tests
##############################################################################

# imports

from strongwind import *
from textblock import *


# class to represent the main window.
class TextBlockFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    TEXT_ONE = "simply TextBlock"
    TEXT_TWO = "sample with LinkBreak and Run\r\nline2\r\nline3\r\nline4"

    def __init__(self, accessible):
        super(TextBlockFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("TextBlockSample")
        self.text1 = self.frame.findLabel(self.TEXT_ONE)
        self.text2 = self.frame.findLabel(self.TEXT_TWO)
