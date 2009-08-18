
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/31/2008
#              Application wrapper for toolstrip.py
#              Used by the toolstrip-*.py tests
##############################################################################

'Application wrapper for toolstrip'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStrip(exe=None):
    'Launch form with accessibility enabled and return a toolstrip object.  \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolstrip.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstrip = ToolStrip(app, subproc)

    cache.addApplication(toolstrip)

    toolstrip.toolStripFrame.app = toolstrip

    return toolstrip

# class to represent the application
class ToolStrip(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstrip window'
        super(ToolStrip, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolStrip control'), logName='Tool Strip')

