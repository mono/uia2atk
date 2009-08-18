
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/03/2009
#              Application wrapper for toolstripseparator.py
#              Used by the toolstripseparator-*.py tests
##############################################################################

'Application wrapper for toolstripseparator'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStripSeparator(exe=None):
    'Launch form with accessibility enabled and return a toolstripseparator \
     object. Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolstripseparator.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripseparator = ToolStripSeparator(app, subproc)

    cache.addApplication(toolstripseparator)

    toolstripseparator.toolStripSeparatorFrame.app = toolstripseparator

    return toolstripseparator

# class to represent the application
class ToolStripSeparator(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstripseparator window'
        super(ToolStripSeparator, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolStripSeparator control'), logName='Tool Strip Separator')

