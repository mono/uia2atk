
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/14
# Description: button.py wrapper script
#              Used by the button-*.py tests
##############################################################################

from strongwind import *
from button import *

# class to represent the main window.
class ButtonFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ButtonSample')
        self.label = self.frame.findLabel(None)
        self.button1 = self.frame.findButton('Button1')
        self.button2 = self.frame.findButton('Button2')
        self.button3 = self.frame.findButton('Button3')
        self.button4 = self.frame.findButton('Button4')
        self.button5 = self.frame.findButton('Button5')

    def assertDialog(self):
        self.dialog = self.frame.findDialog('message')
        self.dialog.findPushButton('OK').click()
