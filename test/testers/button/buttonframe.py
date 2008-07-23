
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
    def diffActions(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        alist = []
        for list in range(ca.nActions):
            alist.append(ca.getName(list))

        procedurelogger.expectedResult('%s\'s inital actions list live up to our expectation' % accessible)
        def resultMatches():
            return alist.sort() == actions.Button.alist.sort()
        assert retryUntilTrue(resultMatches)

    #check Button's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.Button.slist:
            cmd = "state = accessible." + a
            exec(cmd)

            if accessible == self.button3 and state == False:
                pass
            elif accessible != self.button3 and state == False:
                print "ERROR: %s can't be checked" % cmd


    #give 'click' action
    def click(self,button):
        procedurelogger.action('Click the %s.' % button)
        button._doAction(a.Button.CLICK)

    #check the Label text after click button2
    def assertLabel(self, labelText):
        procedurelogger.expectedResult('Label text has been changed to \"%s\"' % labelText)
        self.findLabel(labelText)

    #rise and close message window after click button1
    def assertMessage(self):
        self = self.findFrame('message', logName='successful clicked me')

        self.altF4()
    
    #close application main window after running test
    def quit(self):
        self.altF4()
