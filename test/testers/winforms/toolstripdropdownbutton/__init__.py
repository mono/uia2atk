
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/09/2008
# Description: Application wrapper for toolstripdropdownbutton.py
#              Used by the toolstripdropdownbutton-*.py tests
##############################################################################$

'Application wrapper for toolstripdropdownbutton'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStripDropDownButton(exe=None):
    'Launch toolstripdropdownbutton with accessibility enabled and return a toolstripdropdownbutton object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/toolstripdropdownbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripdropdownbutton = ToolStripDropDownButton(app, subproc)

    cache.addApplication(toolstripdropdownbutton)

    toolstripdropdownbutton.toolStripDropDownButtonFrame.app = toolstripdropdownbutton

    return toolstripdropdownbutton

# class to represent the application
class ToolStripDropDownButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstripdropdownbutton window'
        super(ToolStripDropDownButton, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolStripDropDownButton control'), logName='Tool Strip Drop Down Button')

