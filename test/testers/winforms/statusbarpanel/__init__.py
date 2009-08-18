
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/10/2008
#              Application wrapper for statusbarpanel.py
#              Used by the statusbarpanel-*.py tests
##############################################################################$

'Application wrapper for statusbarpanel'

from strongwind import *

from os.path import exists
from sys import path

def launchStatusBarPanel(exe=None):
    'Launch statusbarpanel with accessibility enabled and return a statusbarpanel object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]

        exe = '%s/samples/winforms/winforms/statusbarpanel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    statusbarpanel = StatusBarPanel(app, subproc)

    cache.addApplication(statusbarpanel)

    statusbarpanel.statusBarPanelFrame.app = statusbarpanel

    return statusbarpanel

# class to represent the application
class StatusBarPanel(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the statusbarpanel window'
        super(StatusBarPanel, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^StatusBar_StatusBarPanel controls'), logName='Status Bar Panel')

