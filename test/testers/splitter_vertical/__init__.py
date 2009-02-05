##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/26/2009
#              Application wrapper for splitter.py
#              Used by the splitter-*.py tests
##############################################################################

'Application wrapper for splitter'

from strongwind import *

from os.path import exists
from sys import path

def launchSplitter(exe=None):
    'Launch splitter with accessibility enabled and return a splitter object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/splitter_vertical.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    splitter = Splitter(app, subproc)

    cache.addApplication(splitter)

    splitter.splitterFrame.app = splitter

    return splitter

# class to represent the application
class Splitter(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the splitter window'
        super(Splitter, self).__init__(accessible, subproc)
        
        self.findFrame("Vertical Splitter", logName='Splitter')

