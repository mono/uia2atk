
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/02/2008
#              Application wrapper for tooltip.py
#              Used by the tooltip-*.py tests
##############################################################################

'Application wrapper for tooltip'

from strongwind import *

from os.path import exists
from sys import path

def launchToolTip(exe=None):
    'Launch tooltip with accessibility enabled and return a tooltip object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/tooltip.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    tooltip = ToolTip(app, subproc)

    cache.addApplication(tooltip)

    tooltip.toolTipFrame.app = tooltip

    return tooltip

# class to represent the application
class ToolTip(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the tooptip window'
        super(ToolTip, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Simple ToolTip Example'), logName='Tool Tip')

