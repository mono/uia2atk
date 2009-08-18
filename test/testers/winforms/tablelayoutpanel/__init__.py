# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/04/2009
# Description: Application wrapper for tablelayoutpanel.py
#              be called by ../tablelayoutpanel_basic_ops.py
##############################################################################

"""Application wrapper for tablelayoutpanel.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchTableLayoutPanel(exe=None):
    """ 
    Launch tablelayoutpanel with accessibility enabled and return a tablelayoutpanel object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/tablelayoutpanel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    tablelayoutpanel = TableLayoutPanel(app, subproc)

    cache.addApplication(tablelayoutpanel)

    tablelayoutpanel.tableLayoutPanelFrame.app = tablelayoutpanel

    return tablelayoutpanel

class TableLayoutPanel(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the tablelayoutpanel window"""

        super(TableLayoutPanel, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^TableLayoutPanel'), logName='Table Layout Panel')
