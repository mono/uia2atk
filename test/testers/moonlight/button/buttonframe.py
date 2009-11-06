
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

    BUTTONS_NUM = 5

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame('ButtonSample')
        self.filler = self.frame.findFiller('Silverlight Control')
        self.label = self.filler.findLabel(None)

        self.buttons = self.filler.findAllPushButtons(None)
        assert len(self.buttons) == self.BUTTONS_NUM, \
                   "actual number of button is %s, expected %s" % \
                    (len(self.buttons), self.BUTTONS_NUM)
        self.button1 = self.buttons[0]
        self.button2 = self.buttons[1]
        self.button3 = self.buttons[2]
        self.button4 = self.buttons[3]
        self.button5 = self.buttons[4]

        self.b4_image = self.button4.findImage(None)
        self.b5_image = self.button5.findImage(None)
        self.b5_label = self.button5.findLabel(None)

    def assertDialog(self):
        self.dialog = self.frame.findDialog(None)
        self.dialog.findPushButton('OK').click()
