
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
# Description: button.py wrapper script
#              Used by the button-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from button import *


# class to represent the main window.
class ButtonFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button1"
    BUTTON_TWO = "button2"
    BUTTON_TREE = "button3"

    def __init__(self, accessible):
        super(ButtonFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)
        self.button2 = self.findPushButton(self.BUTTON_TWO)
        self.button3 = self.findPushButton(self.BUTTON_TREE)

    #diff Button's inital actions list with expectant list in actions.py
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallist = ()
        for lists in range(ca.nActions):
            initallist = (ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions \"%s\" live up to\
	our expectation' % (accessible,initallists))
        def resultMatches():
            return sorted(initallist) == sorted(actions.Button.actions)
        assert retryUntilTrue(resultMatches)

    #check Button's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.Button.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #check disable Button's states
    def assertDisableButton(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can\'t be found except \'showing\'' % accessible)
        for a in states.Button.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == True and a != 'showing':
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #give 'click' action
    def click(self,button):
        procedurelogger.action('Click the %s.' % button)
        button.click()

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to \"%s\"' % labelText)
        self.findLabel(labelText)

    #rise message frame window after click button1
    def assertMessage(self):
        self.app.findFrame('message')
    
    #close application main window after running test
    def quit(self):
        self.altF4()
