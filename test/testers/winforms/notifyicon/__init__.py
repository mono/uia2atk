
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2008
#              Application wrapper for notifyicon.py
#              Used by the notifyicon-*.py tests
##############################################################################$

'Application wrapper for notifyicon'

from strongwind import *

from os.path import exists
from sys import path

def launchNotifyIcon(exe=None):
    'Launch notifyicon with accessibility enabled and return a notifyicon object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/notifyicon.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    notifyicon = NotifyIcon(app, subproc)

    cache.addApplication(notifyicon)

    notifyicon.notifyIconFrame.app = notifyicon

    return notifyicon

# class to represent the application
class NotifyIcon(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the NotifyIcon window'
        super(NotifyIcon, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^NotifyIcon control'), logName='Notify Icon')

