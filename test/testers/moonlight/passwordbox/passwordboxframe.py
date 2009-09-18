
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/16
# Description: passwordbox.py wrapper script
#              Used by the passwordbox-*.py tests
##############################################################################

from strongwind import *
from passwordbox import *

# class to represent the main window.
class PasswordBoxFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(PasswordBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('PasswordBoxSample')
        self.label1 = self.frame.findLabel(None)
        self.label2 = self.frame.findLabel(None)
        self.pwdBox = self.frame.findText(None)
