# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/21/2008
# Description: Application wrapper for panel.py
#              be called by ../panel_basic_ops.py
##############################################################################$

"""Application wrapper for panel"""

from strongwind import *
from os.path import exists
from sys import path

def launchPanel(exe=None):
    """ 
    Launch panel with accessibility enabled and return a panel object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/panel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    panel = Panel(app, subproc)

    cache.addApplication(panel)

    panel.panelFrame.app = panel

    return panel

class Panel(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the panel window"""

        super(Panel, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^Panel'), logName='Panel')
