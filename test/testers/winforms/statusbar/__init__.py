
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/15/2008
#              Application wrapper for statusbar.py
#              Used by the statusbar-*.py tests
##############################################################################$

'Application wrapper for statusbar'

from strongwind import *

from os.path import exists
from sys import path

def launchStatusBar(exe=None):
    'Launch statusbar with accessibility enabled and return a statusbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]

        exe = '%s/samples/winforms/statusbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    statusbar = StatusBar(app, subproc)

    cache.addApplication(statusbar)

    statusbar.statusBarFrame.app = statusbar

    return statusbar

# class to represent the application
class StatusBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the statusbar window'
        super(StatusBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^StatusBar controls'), logName='Status Bar')

