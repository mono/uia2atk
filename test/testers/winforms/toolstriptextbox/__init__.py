
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        12/11/2008
# Description: Application wrapper for toolstriptextbox.py
#              Used by the toolstriptextbox-*.py tests
##############################################################################$

'Application wrapper for toolstriptextbox'

from strongwind import *

from os.path import exists
from sys import path

def launchToolStripTextBox(exe=None):
    'Launch toolstripTextBox with accessibility enabled and return a toolstriptextbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/toolstriptextbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    toolstriptextbox = ToolStripTextBox(app, subproc)

    cache.addApplication(toolstriptextbox)

    toolstriptextbox.toolStripTextBoxFrame.app = toolstriptextbox

    return toolstriptextbox

# class to represent the application
class ToolStripTextBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the toolstriptextbox window'
        super(ToolStripTextBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToolStripTextBox control'), logName='Tool Strip Text Box')

