
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: hscrollbar.py wrapper script
#              Used by the hscrollbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from hscrollbar import *


# class to represent the main window.
class HScrollBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "Value:"

    def __init__(self, accessible):
        super(HScrollBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        #self.vscrollbar = self.findScrollBar(None)

    #check hscrollbar's all expectant states
    def statesCheck(self, accessible=None):
        accessible = self.findScrollBar(None)
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.HScrollBar.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #change vscrollbar's value
    def valueScrollBar(self, newValue=None):

        procedurelogger.action('set scrollbar value to \"%s\"' % newValue)
        self.findScrollBar(None).__setattr__('value', newValue)

    def assertScrollbar(self, newValue=None):
        maximumValue = self.findScrollBar(None)._accessible.queryValue().maximumValue

        def resultMatches():
            if maximumValue < newValue < 0 :
                procedurelogger.expectedResult('value \"%s\" out of run' % newValue)
                return not self.findScrollBar(None).__getattr__('value') == newValue
            else:
                procedurelogger.expectedResult('the scrollbar\'s current value is \"%s\"' % newValue)
                return self.findLABEL('Value:%s' % newValue)
        assert retryUntilTure(resultMatches)
    
    #close application window
    def quit(self):
        self.altF4()
