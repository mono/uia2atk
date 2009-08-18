##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        02/12/2009
#              Application wrapper for treeview.py
#              Used by the treeview-*.py tests
##############################################################################

'Application wrapper for treeview'

from strongwind import *

from os.path import exists
from sys import path

def launchTreeView(exe=None):
    'Launch TreeView with accessibility enabled and return a TreeView object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/treeview.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    treeview = TreeView(app, subproc)

    cache.addApplication(treeview)

    treeview.treeViewFrame.app = treeview

    return treeview

# class to represent the application
class TreeView(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the label window'
        super(TreeView, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^TreeView control'), logName='Tree View')
