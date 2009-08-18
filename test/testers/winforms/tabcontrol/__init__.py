
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        01/09/2009
# Description: Application wrapper for tabcontrol.py
#              Used by the tabcontrol-*.py tests
##############################################################################$

'Application wrapper for tabcontrol'

from strongwind import *

from os.path import exists
from sys import path

def launchTabControl(exe=None):
    'Launch tabcontrol with accessibility enabled and return a tabcontrol object.  Log an error and return None if something goes wrong'

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

    tabcontrol = TabControl(app, subproc)

    cache.addApplication(tabcontrol)

    tabcontrol.tabControlFrame.app = tabcontrol

    return tabcontrol

# class to represent the application
class TabControl(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the tabpage window'
        super(TabControl, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^TabControl'), logName='Tab Control')

