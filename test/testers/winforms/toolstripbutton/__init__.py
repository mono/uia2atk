
##############################################################################
# Written by:  Andres G. Aragoneses <aaragoneses@novell.com>
# Date:        02/23/2009
#              Application wrapper for toolstripbutton.py
#              Used by the toolstripbutton-*.py tests
##############################################################################

'Application wrapper for toolstripbutton'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStrip(exe=None):
    'Launch form with accessibility enabled and return a toolstripbutton object.  \
     Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/toolstripdropdown_toolstripbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripbutton = ToolStripButton(app, subproc)

    cache.addApplication(toolstripbutton)

    toolstripbutton.toolStripButtonFrame.app = toolstripbutton

    return toolstripbutton

# class to represent the application
class ToolStripButton(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstripbutton window'
        super(ToolStripButton, self).__init__(accessible, subproc)
        
        self.findFrame("ToolStripButton Example", logName='Tool Strip Button')

