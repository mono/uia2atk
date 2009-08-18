# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2008
# Description: Application wrapper for listview_list.py
#              be called by ../listview_list_basic_ops.py
##############################################################################$

"""Application wrapper for listview_list"""

from strongwind import *
from os.path import exists
from sys import path

def launchListView(exe=None):
    """
    Launch listview list mode with accessibility enabled and return a listview object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/listview_list.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    listview = ListView(app, subproc)

    cache.addApplication(listview)

    listview.listViewFrame.app = listview

    return listview

class ListView(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the listview window"""

        super(ListView, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ListView'), logName='List View')
