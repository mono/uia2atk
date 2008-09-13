
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
        for s in states.Form.states:
            cmd = "state = self." + s
            exec(cmd)

            assert state
    
    #close Form window
    def quit(self):
        self.altF4()
