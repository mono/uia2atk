
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/07/2008
#              Application wrapper for statusstrip.py
#              Used by the statusstrip-*.py tests
##############################################################################$

'Application wrapper for statusstrip'

from strongwind import *

from os.path import exists
from sys import path

def launchStatusStrip(exe=None):
    'Launch statusstrip with accessibility enabled and return a statusstrip object.  \
    Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]

        exe = '%s/samples/winforms/statusstrip.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    statusstrip = StatusStrip(app, subproc)

    cache.addApplication(statusstrip)

    statusstrip.statusStripFrame.app = statusstrip

    return statusstrip

# class to represent the application
class StatusStrip(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the statusstrip window'
        super(StatusStrip, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^StatusStrip control'), logName='Status Strip')

