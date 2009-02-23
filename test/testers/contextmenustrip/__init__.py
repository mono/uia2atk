
##############################################################################
# Written by:  Mike Gorse <mgorse@novell.com>
# Date:        02/23/2009
# Description: Application wrapper for contextmenustrip.py
#              Used by the contextmenustrip-*.py tests
##############################################################################

'Application wrapper for contextmenustrip'

from strongwind import *

from os.path import exists
from sys import path


def launchContextMenuStrip(exe=None):
    'Launch ContextMenuStrip with accessibility enabled and return a ContextMenuStrip object. Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/contextmenustrip.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    contextmenustrip = ContextMenuStrip(app, subproc)

    cache.addApplication(contextmenustrip)

    contextmenustrip.contextMenuStripFrame.app = contextmenustrip

    return contextmenustrip


# class to represent the application
class ContextMenuStrip(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the label window'
        super(ContextMenuStrip, self).__init__(accessible, subproc)
        self.findFrame('ContextMenuStrip control', logName='Context Menu Strip')
