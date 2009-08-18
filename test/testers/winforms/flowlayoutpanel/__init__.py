# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/02/2009
# Description: Application wrapper for flowlayoutpanel.py
#              be called by ../flowlayoutpanel_basic_ops.py
##############################################################################

"""Application wrapper for flowlayoutpanel.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchFlowLayoutPanel(exe=None):
    """ 
    Launch flowlayoutpanel with accessibility enabled and return a flowlayoutpanel object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/flowlayoutpanel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    flowlayoutpanel = FlowLayoutPanel(app, subproc)

    cache.addApplication(flowlayoutpanel)

    flowlayoutpanel.flowLayoutPanelFrame.app = flowlayoutpanel

    return flowlayoutpanel

class FlowLayoutPanel(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the flowlayoutpanel window"""

        super(FlowLayoutPanel, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^FlowLayoutPanel'), logName='Flow Layout Panel')
