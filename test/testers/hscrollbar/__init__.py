
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/05/2008
# Description: Application wrapper for hscrollbar.py
#              Used by the hscrollbar-*.py tests
##############################################################################$

'Application wrapper for hscrollbar'

from strongwind import *

import os

def launchHScrollBar(exe=None):
    'Launch HScrollBar with accessibility enabled and return a Hscrollbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        uiaqa_path = os.environ.get("UIAQA_HOME")
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/hscrollbar.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
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

