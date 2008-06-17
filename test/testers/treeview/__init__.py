# -*- coding: utf-8 -*-

##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: Application wrapper for checkButton.py 
#              Used by the checkbutton-*.py tests
##############################################################################$

'Application wrapper for checkbutton'

import os

from strongwind import *


def launchTreeView(exe=None):
    'Launch treeview with accessibility enabled and return a treeview object.  Log an error and return None if something goes wrong'

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

    treeView = TreeView(app, subproc)
    cache.addApplication(treeView)

    treeView.treeViewFrame.app = treeView

    return treeView

# class to represent the application
class TreeView(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the treeview window'
        super(TreeView, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^TreeView'), logName='Tree View')

