
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/12/2008
# Description: Application wrapper for listview_smallimage.py
#              Used by the listview_smallimage-*.py tests
##############################################################################$

'Application wrapper for listview_smallimage'

from strongwind import *

from os.path import exists
from sys import path

def launchListView(exe=None):
    'Launch listview smallimage mode with accessibility enabled and return a listview object. \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/listview_smallimage.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    listview = ListView(app, subproc)

    cache.addApplication(listview)

    listview.listViewSmallImageFrame.app = listview

    return listview

# class to represent the application
class ListView(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the listview window'
        super(ListView, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ListView control'), logName='List View Small Image')

