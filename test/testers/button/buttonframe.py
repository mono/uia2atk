# -*- coding: utf-8 -*-

##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: checkButton.py wrapper script
#              Used by the button-*.py tests
##############################################################################$

from strongwind import *
from button import *


# class to represent the main window.

class ButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Button 1"
    BUTTON_TWO = "Button 2"
    LABEL = "label text"

    # available results for the check boxes
    RESULT_UNCHECKED = "unchecked"
    RESULT_CHECKED = "checked"
    # end constants

    logName = 'Button'

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.label = self.findLabel(self.LABEL)
        #self.textview = self.find()

#send "press" action
    def press(self,button,log=True):
        button._doAction('press')
        if log:
            procedurelogger.action('Press the %s.' % button)

#send "release" action
    def release(self,button,log=True):
        button._doAction('release')
        if log:
            procedurelogger.action('Press the %s.' % button)

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
#check if there is rise a messagedialog when send "click" action.
    def clickResult(self,MessageDialog=True):

        if MessageDialog:
	      self.app.findAlert(None,logText="MessageDialog").altF4()

#check if the text in TextBox is the same as entering text.
    def textResult(self,text):

        procedurelogger.expectedResult('The text region displays %s.' % text)

        def resultMatches():
            return self.findText('').text == text

        assert retryUntilTrue(resultMatches)


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass
