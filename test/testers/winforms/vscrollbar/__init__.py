
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/04/2008
# Description: Application wrapper for vscrollbar.py
#              Used by the vscrollbar-*.py tests
##############################################################################$

'Application wrapper for vscrollbar'

from strongwind import *

from os.path import exists
from sys import path

def launchVScrollBar(exe=None):
    'Launch vscrollbar with accessibility enabled and return a vscrollbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/vscrollbar.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    vscrollbar = VScrollBar(app, subproc)

    cache.addApplication(vscrollbar)

    vscrollbar.vScrollBarFrame.app = vscrollbar

    return vscrollbar

# class to represent the application
class VScrollBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the VscrollBar window'
        super(VScrollBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^VScrollBar control'), logName='V Scroll Bar')

