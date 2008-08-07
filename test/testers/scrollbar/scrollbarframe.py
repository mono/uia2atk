
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/06/2008
# Description: scrollbar.py wrapper script
#              Used by the scrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from scrollbar import *


# class to represent the main window.
class ScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = ""

    def __init__(self, accessible):
        super(ScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.scrollbar = self.findScrollBar(None)

    #check scrollbar's all expectant states
    def statesCheck(self, accessible):
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        
        for a in states.HScrollBar.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #change scrollbar's value
    def valueScrollBar(self, newValue=None):

        procedurelogger.action('set scrollbar value to \"%s\"' % newValue)
        self.findScrollBar(None).__setattr__('value', newValue)

    def assertScrollbar(self, newValue=None):
        maximumValue = self.findScrollBar(None)._accessible.queryValue().maximumValue

        def resultMatches():
            if 0 <= newValue <= maximumValue:
                procedurelogger.expectedResult('the scrollbar\'s current value is \"%s\"' % newValue)
                print "scrollbar's current value is:", self.findScrollBar(None).__getattr__('value')
                return self.findScrollBar(None).__getattr__('value') == newValue
            else:
                procedurelogger.expectedResult('value \"%s\" out of run' % newValue)
                return not self.findScrollBar(None).__getattr__('value') == newValue
        assert retryUntilTrue(resultMatches)
    
    #close application window
    def quit(self):
        self.altF4()
