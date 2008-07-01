
##############################################################################$
# Written by:  Calen Chen <cachen@novell.com>
# Date:        06/27/2008
# Description: gtkbutton.py wrapper script
#              Used by the button_*.py tests
##############################################################################$

from strongwind import *
from button import *


# class to represent the main window.

class ButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Button 1"
    BUTTON_TWO = "Button 2"

    logName = 'Button'

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)

    #send "press" action
    def press(self,button,log=True):
        if log:
            procedurelogger.action('Press the %s.' % button)
            button._doAction('press')

    #send "release" action
    def release(self,button,log=True):
        if log:
            procedurelogger.action('release the %s.' % button)
            button._doAction('release')

    #check if there is "armed" status when send "release" action.
    def assertResult(self, button, result):
        'Raise exception if the button does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (button, result))

        # Check the result
        def resultMatches():
            if result == "armed":
                return button.armed
            elif result == "unarmed":
                return not button.armed
            else:
                raise InvalidState, "%s has no such state:  %s" %\
                                 (button, result)
        assert retryUntilTrue(resultMatches)

    #check if there is rise a messagedialog when send "click" action.
    def clickResult(self,MessageDialog=True):

        if MessageDialog:
            self = self.app.findAlert(None,logText="MessageDialog")

            self.altF4()

    #close application window after running test
    def quit(self):
        'Quit application'

        self.altF4()


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass
