# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/10/2008
# Description: Application wrapper for toolstriplabel.py
#              be called by ../toolstriplabel_basic_ops.py
##############################################################################

"""Application wrapper for toolstriplabel.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchToolStripLabel(exe=None):
    """ 
    Launch toolstriplabel with accessibility enabled and return a toolstriplabel object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolstriplabel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstriplabel = ToolStripLabel(app, subproc)

    cache.addApplication(toolstriplabel)

    toolstriplabel.toolStripLabelFrame.app = toolstriplabel

    return toolstriplabel

class ToolStripLabel(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the toolstriplabel window"""

        super(ToolStripLabel, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ToolStripLabel'), logName='Tool Strip Label')
