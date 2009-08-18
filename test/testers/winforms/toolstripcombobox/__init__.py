# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        12/11/2008
# Description: Application wrapper for toolstripcombobox.py
#              be called by ../toolstripcombobox_basic_ops.py
##############################################################################

"""Application wrapper for toolstripcombobox.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchToolStripComboBox(exe=None):
    """ 
    Launch toolstripcombobox with accessibility enabled and return a toolstripcombobox object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/toolstripcombobox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripcombobox = ToolStripComboBox(app, subproc)

    cache.addApplication(toolstripcombobox)

    toolstripcombobox.toolStripComboBoxFrame.app = toolstripcombobox

    return toolstripcombobox

class ToolStripComboBox(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the toolstripcombobox window"""

        super(ToolStripComboBox, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ToolStripComboBox'), logName='Tool Strip Combo Box')
