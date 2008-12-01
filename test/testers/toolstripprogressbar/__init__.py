
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/04/2008
# Description: Application wrapper for toolstripprogressbar.py
#              Used by the toolstripprogressbar-*.py tests
##############################################################################$

'Application wrapper for toolstripprogressbar'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStripProgressBar(exe=None):
    'Launch toolstripprogressbar with accessibility enabled and return a toolstripprogressbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/toolstripprogressbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstripprogressbar = ToolStripProgressBar(app, subproc)

    cache.addApplication(toolstripprogressbar)

    toolstripprogressbar.toolStripProgressBarFrame.app = toolstripprogressbar

    return toolstripprogressbar

# class to represent the application
class ToolStripProgressBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstripprogressbar window'
        super(ToolStripProgressBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolStripProgressBar control'), logName='Tool Strip Progress Bar')

