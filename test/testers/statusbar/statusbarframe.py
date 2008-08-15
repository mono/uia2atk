
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/15/2008
# Description: statusbar.py wrapper script
#              Used by the statusbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from statusbar import *


# class to represent the main window.
class StatusBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    BUTTON_ONE = "Click me"

    def __init__(self, accessible):
        super(StatusBarFrame, self).__init__(accessible)
        self.button1 = self.findPushButton(self.BUTTON_ONE)

    #check statusbar's all expectant states
    def statesCheck(self, accessible=None):
        accessible = self.findStatusBar(None)
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        #for a in states.VScrollBar.states:
        #    cmd = "state = accessible." + a
        #   exec(cmd)

        #    if state == False:
        #        print "ERROR: %s can't be checked" % cmd
        #    else:
        #        pass
        #if there is just one state in list, should reset it like:
        cmd = "state = accessible." + states.StatusBar.states
        exec(cmd)
        
        if state == False:
            print "ERROR: %s can't can't be checked" % cmd
        else:
            pass

    #give 'click' action
    def click(self,button):
        #procedurelogger.action('Click the %s.' % button)
        #button._doAction("click")
        button.click()

    #assert if can find statusbar
    def assertStatusBar(self):
        procedurelogger.action('search for statusbar role')

        procedurelogger.expectedResult('succeeded in finding StatusBar role')
        def resultMatches():
            return self.findStatusBar(None)
        assert retryUntilTrue(resultMatches)
    
    #close application main window after running test
    def quit(self):
        self.altF4()
