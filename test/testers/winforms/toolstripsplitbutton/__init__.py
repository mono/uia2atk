# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        01/08/2008
# Description: Application wrapper for toolstripsplitbutton.py
#              be called by ../toolstripsplitbutton_basic_ops.py
##############################################################################

"""Application wrapper for toolstripsplitbutton.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchToolStripSplitButton(exe=None):
    """ 
    Launch toolstripsplitbutton with accessibility enabled and return a toolstripsplitbutton object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolstripsplitbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripsplitbutton = ToolStripSplitButton(app, subproc)

    cache.addApplication(toolstripsplitbutton)

    toolstripsplitbutton.toolStripSplitButtonFrame.app = toolstripsplitbutton

    return toolstripsplitbutton

class ToolStripSplitButton(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the toolstripsplitbutton window"""

        super(ToolStripSplitButton, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ToolStripSplitButton'), logName='Tool Strip Split Button')
