
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

    LABEL1 = "You changed 0 times."
    LABEL2 = "Your password is: "

    def __init__(self, accessible):
        super(PasswordBoxFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('PasswordBoxSample')
        self.label1 = self.frame.findLabel(self.LABEL1)
        self.label2 = self.frame.findLabel(self.LABEL2)
        self.pwdBox = self.frame.findPasswordText(None)
