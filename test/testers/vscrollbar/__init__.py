
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/04/2008
# Description: Application wrapper for vscrollbar.py
#              Used by the vscrollbar-*.py tests
##############################################################################$

'Application wrapper for vscrollbar'

from strongwind import *

import os

def launchVScrollBar(exe=None):
    'Launch radiobutton with accessibility enabled and return a vscrollbar object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        uiaqa_path = os.environ.get("UIAQA_HOME")
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/vscrollbar.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    vscrollbar = VScrollBar(app, subproc)

    cache.addApplication(vscrollbar)

    vscrollbar.vScrollBarFrame.app = vscrollbar

    return vscrollbar

# class to represent the application
# because now program can't show winforms applicantion items 'showing' state,
# before runing this test, we should change 'x.showing' to 'not x.showing' in 
# accessibles.py line 180. remember to revert it after program improving
class VScrollBar(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the VscrollBar window'
        super(VScrollBar, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^VScrollBar control'), logName='V Scroll Bar')

