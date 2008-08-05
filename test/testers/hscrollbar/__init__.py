
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: Application wrapper for hscrollbar.py
#              Used by the hscrollbar-*.py tests
##############################################################################$

'Application wrapper for hscrollbar'

from strongwind import *

from os.path import exists
from sys import path

def launchHScrollBar(exe=None):
    'Launch HScrollBar with accessibility enabled and return a Hscrollbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/gtkcheckbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    hscrollbar = HScrollBar(app, subproc)

    cache.addApplication(hscrollbar)

    hscrollbar.hScrollBarFrame.app = hscrollbar

    return hscrollbar

# class to represent the application
# because now program can't show winforms applicantion items 'showing' state,
# before runing this test, we should change 'x.showing' to 'not x.showing' in 
# accessibles.py line 180. remember to revert it after program improving
class HScrollBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the HscrollBar window'
        super(HScrollBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^HScrollBar control'), logName='H Scroll Bar')

