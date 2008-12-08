# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        11/19/2008
# Description: Application wrapper for menustrip.py
#              be called by ../menustrip_basic_ops.py
##############################################################################

"""Application wrapper for menustrip.py"""

from strongwind import *

class MenuStripFrame(accessibles.Frame):
    """the profile of the menustrip sample"""

    def __init__(self, accessible):
        super(MenuStripFrame, self).__init__(accessible)
        self.menustrip = self.findMenuBar(None)

    # close sample application after running the test
    def quit(self):
        self.altF4()
