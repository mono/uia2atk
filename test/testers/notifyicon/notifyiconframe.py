
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
# Description: notifyicon.py wrapper script
#              Used by the notifyicon-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from notifyicon import *


# class to represent the main window.
class NotifyIconFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "button"

    def __init__(self, accessible):
        super(NotifyIconFrame, self).__init__(accessible)
        self.button = self.findPushButton(self.BUTTON_ONE)

    #check Button's all expectant actions
    def actionsCheck(self, accessible):
        procedurelogger.action('diff %s\'s actions list' % accessible)
        ca = accessible._accessible.queryAction()
        initallist = []
        for lists in range(ca.nActions):
            initallist.append(ca.getName(lists))

        procedurelogger.expectedResult('%s\'s inital actions "%s" live up to our expectation' % (accessible,initallist))
        def resultMatches():
            return sorted(initallist) == sorted(actions.Button.actions)
        assert retryUntilTrue(resultMatches), "%s != %s" % \
                       (sorted(initallist), sorted(actions.Button.actions))

    #check NotifyIcon frame's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.Form.states:
            state = getattr(accessible, a)
            assert state, "Expected state: %s" % (a)

    #give 'click' action
    def click(self, button):
        button.click()

    #assert notifyicon after click button
    def assertNotifyIcon(self):
        self.notifyicon = self.app.findFrame('I\'m NotifyIcon')
 
    #close application main window after running test
    def quit(self):
        self.altF4()
