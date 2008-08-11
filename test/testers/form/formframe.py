
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/11/2008
# Description: form.py wrapper script
#              Used by the form-*.py tests
##############################################################################

import sys
import os
import actions
import states

from strongwind import *
from form import *


# class to represent the main window.
class FormFrame(accessibles.Frame):

    def __init__(self, accessible):
        super(FormFrame, self).__init__(accessible)

    #check Form's all expectant states
    def statesCheck(self):
        procedurelogger.action('check %s\'s all states' % self)

        procedurelogger.expectedResult('%s\'s all states can be found' % self)
        #for a in states.VScrollBar.states:
        #    cmd = "state = accessible." + a
        #   exec(cmd)

        #    if state == False:
        #        print "ERROR: %s can't be checked" % cmd
        #    else:
        #        pass
        #if there is just one state in list, should reset it like:
        cmd = "state = self." + states.Form.states
        exec(cmd)
        
        if state == False:
            print "ERROR: %s can't can't be checked" % cmd
        else:
            pass
    
    #close Form window
    def quit(self):
        self.altF4()
