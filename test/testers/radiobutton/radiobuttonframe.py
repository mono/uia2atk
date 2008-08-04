
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/29/2008
# Description: radiobutton.py wrapper script
#              Used by the radiobutton-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from radiobutton import *


# class to represent the main window.
class RadioButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Male"
    BUTTON_TWO = "Female"
    BUTTON_THREE = "Disabled"
    LABEL = "Go On:____"

    def __init__(self, accessible):
        super(RadioButtonFrame, self).__init__(accessible)
        self.button1 = self.findRadioButton(self.BUTTON_ONE)
        self.button2 = self.findRadioButton(self.BUTTON_TWO)
        self.button3 = self.findRadioButton(self.BUTTON_THREE)
        self.label = self.findLabel(self.LABEL)

    #diff RadioButton's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallists = ()
        for lists in range(ca.nActions):
            initallists = (ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions \"%s\" live up to\
	our expectation' % (accessible,initallists))
        def resultMatches():
            return sorted(initallists) == sorted(actions.RadioButton.actions)
        assert retryUntilTrue(resultMatches)

    #check RadioButton's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.RadioButton.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #check disable RadioButton's states
    def statesDisableCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can\'t be found' % accessible)
        for a in states.RadioButton.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == True:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #give 'click' action
    def click(self,button):
        procedurelogger.action('Click the %s.' % button)
        button.click()

    #check the Label text after click RadioButton
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to \"%s\"' % labelText)
        self.findLabel(labelText)

    #check the state after click RadioButton
    def assertChecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('\"%s\" is %s' % (accessible, 'checked'))

        def resultMatches():
            return accessible.checked
	
        assert retryUntilTrue(resultMatches)

    def assertUnchecked(self, accessible):
        'Raise exception if the accessible does not match the given result'   
        procedurelogger.expectedResult('%s is %s.' % (accessible, "unchecked"))

        def resultMatches():
            return not accessible.checked
	
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
