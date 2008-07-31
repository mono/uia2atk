
##############################################################################$
# Written by:  Calen Chen <cachen@novell.com>
# Date:        06/27/2008
# Description: gtkbutton.py wrapper script
#              Used by the gtkbutton_*.py tests
##############################################################################$

import actions as a

from strongwind import *
from gtkbutton import *

# class to represent the main window.
class GtkButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Button 1"
    BUTTON_TWO = "Button 2"

    def __init__(self, accessible):
        super(GtkButtonFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)

    #send "press" action
    def press(self, button):
        procedurelogger.action('Press the %s.' % button)
        button.press()

    #send "release" action
    def release(self, button):
        procedurelogger.action('release the %s.' % button)
        button.release()

    #send "click" action, accerciser abstracts this for us
    def click(self, button):
        button.click()

    def assertArmed(self, accessible):
        'Raise exception if the accessible is not armed'
        procedurelogger.expectedResult('%s is %s.' % (accessible, "armed"))
        def resultMatches():
            return accessible.armed
        assert retryUntilTrue(resultMatches)

    def assertUnarmed(self, accessible):
        'Raise exception if the accessible is armed'
        procedurelogger.expectedResult('%s is %s.' % (accessible, "unarmed"))
        def resultMatches():
            return not accessible.armed
        assert retryUntilTrue(resultMatches)

    #check if there is rise a messagedialog when send "click" action.
    def assertClicked(self):

        self = self.app.findDialog(None,"Message Dialog")

        self.altF4()

    #close application window after running test
    def quit(self):
        'Quit application'

        self.altF4()
