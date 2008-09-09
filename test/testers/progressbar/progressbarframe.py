
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: progressbar.py wrapper script
#              Used by the progressbar-*.py tests
##############################################################################$

import sys
import os
import actions
import states

from strongwind import *
from progressbar import *


# class to represent the main window.
class ProgressBarFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    LABEL = "It is 0 percent of 100%"
    BUTTON = "Click"

    def __init__(self, accessible):
        super(ProgressBarFrame, self).__init__(accessible)
        self.label = self.findLabel(self.LABEL)
        self.button = self.findPushButton(self.BUTTON)
        self.progressbar = self.findProgressBar(None)

    #check progressbar's all expectant states
    def statesCheck(self, accessible=None):
        accessible = self.progressbar
        procedurelogger.action('check %s\'s all states' % accessible)

        procedurelogger.expectedResult('%s\'s all states can be found' % accessible)
        for a in states.ProgressBar.states:
            cmd = "state = accessible." + a
            exec(cmd)

            if state == False:
                print "ERROR: %s can't be checked" % cmd
            else:
                pass

    #give 'click' action
    def click(self, button):
        button.click()

    #assert the progress's percent after click button
    def assertLabel(self, percent):
        procedurelogger.expectedResult('%s percent of progress' % percent)

        def resultMatches():
            return self.findLabel(None).text == "It is %s percent " % percent + "of 100%"
        assert retryUntilTrue(resultMatches)

    #assert progressbar's value
    def assertValue(self, value=None):
        maximumValue = self.progressbar._accessible.queryValue().maximumValue

        def resultMatches():
            if 0 <= value <= maximumValue:
                procedurelogger.expectedResult('the progressbar\'s current value is \"%s\"' % value)
                print "progressbar's current value is:", self.progressbar.__getattr__('value')
                return self.progressbar.__getattr__('value') == value
            else:
                procedurelogger.expectedResult('value \"%s\" out of run' % value)
                return not self.progressbar.__getattr__('value') == value
        assert retryUntilTrue(resultMatches)
    
    #close application window
    def quit(self):
        self.altF4()
