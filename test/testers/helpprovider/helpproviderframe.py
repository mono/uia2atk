# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: helpprovider.py wrapper script
#              Used by the helpprovider-*.py tests
##############################################################################

'''Application wrapper for helpprovider.py'''

from strongwind import *
from helpers import *

# class to represent the main window.
class HelpProviderFrame(accessibles.Frame):

    # constants

    def __init__(self, accessible):
        super(HelpProviderFrame, self).__init__(accessible)
        #self.depth_label = self.findLabel(self.DEPTH_LABEL)

    # close sample application after running the test
    def quit(self):
        self.altF4()
