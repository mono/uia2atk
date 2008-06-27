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
#check if there is rise a messagedialog when send "click" action.
    def clickResult(self,MessageDialog=True,closeDialog=False):

        if MessageDialog:
	      self.app.findAlert(None,logText="MessageDialog")


#check if the text in TextBox is the same as entering text.
    def textResult(self,text):

        procedurelogger.expectedResult('The text region displays %s.' % text)

        def resultMatches():
            return self.findText('').text == text

        assert retryUntilTrue(resultMatches)

#close the dialog windows.
    def close(self, assertClosed=True):
        procedurelogger.action('In the Information alert, press <Alt>F4.')
        sleep(config.SHORT_DELAY)

        procedurelogger.expectedResult('the messagedialog has been closed')
        self.keyCombo('<Alt>F4')


class InvalidState(Exception):
  pass

class InvalidAccessible(Exception):
  pass
