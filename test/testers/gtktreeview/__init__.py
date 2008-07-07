
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        06/18/2008
#              Application wrapper for gtktreeview.py
#              Used by the treeview-*.py tests
##############################################################################$

'Application wrapper for gtktreeview'

from strongwind import *

import os

def launchTreeView(exe=None):
    'Launch gtktreeview with accessibility enabled and return a treeview object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        uiaqa_path = os.environ.get("UIAQA_HOME")
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/gtktreeview.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args)

    treeview = GtkTreeView(app, subproc)
    cache.addApplication(treeview)

    treeview.gtkTreeViewFrame.app = treeview

    return treeview

# class to represent the application
class GtkTreeView(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Tree View window'
        super(GtkTreeView, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^Tree View'), logName='Gtk Tree View')

