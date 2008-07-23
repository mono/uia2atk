
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
#              Application wrapper for Button.py
#              Used by the button-*.py tests
##############################################################################$

'Application wrapper for button'

from strongwind import *

import os

def launchButton(exe=None):
    'Launch button with accessibility enabled and return a button object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        uiaqa_path = os.environ.get("UIAQA_HOME")
        if uiaqa_path is None:
          raise IOError, "When launching an application you must provide the "\
                         "full path or set the\nUIAQA_HOME environment "\
                         "variable."

        exe = '%s/samples/button_label_linklabel.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    button = Button(app, subproc)

    cache.addApplication(button)

    button.buttonFrame.app = button

    return button

# class to represent the application
# because now program can't show winforms applicantion items 'showing' state,
# before runing this test, we should change 'x.showing' to 'not x.showing' in 
# accessibles.py line 180. remember to revert it after program improving
class Button(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the Button window'
        super(Button, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Button_Label_LinkLabel controls'), logName='Button')

