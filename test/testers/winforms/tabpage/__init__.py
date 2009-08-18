
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/07/2009
# Description: Application wrapper for tabpage.py
#              Used by the tabpage-*.py tests
##############################################################################$

'Application wrapper for tabpage'

from strongwind import *

from os.path import exists
from sys import path

def launchTabPage(exe=None):
    'Launch tabpage with accessibility enabled and return a tabpage object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/tabpage.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    tabpage = TabPage(app, subproc)

    cache.addApplication(tabpage)

    tabpage.tabPageFrame.app = tabpage

    return tabpage

# class to represent the application
class TabPage(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the tabpage window'
        super(TabPage, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^TabControl'), logName='Tab Page')

