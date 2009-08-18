
##############################################################################
# Written by:  Neville Gao <nevillegao@gmail.com>
# Date:        02/18/2009
# Description: Application wrapper for contextmenu_menuitem.py
#              Used by the contextmenu-*.py tests
##############################################################################

'Application wrapper for contextmenu_menuitem'

from strongwind import *

from os.path import exists
from sys import path


def launchContextMenu(exe=None):
    'Launch ContextMenu with accessibility enabled and return a ContextMenu object. Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/contextmenu_menuitem.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    contextmenu = ContextMenu(app, subproc)

    cache.addApplication(contextmenu)

    contextmenu.contextMenuFrame.app = contextmenu

    return contextmenu


# class to represent the application
class ContextMenu(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        super(ContextMenu, self).__init__(accessible, subproc)
        self.findFrame('ContextMenu_MenuItem control', logName='Context Menu')
