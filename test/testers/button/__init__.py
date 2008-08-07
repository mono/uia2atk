
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/22/2008
#              Application wrapper for Button.py
#              Used by the button-*.py tests
##############################################################################$

'Application wrapper for button'

from strongwind import *

from os.path import exists
from sys import path

def launchButton(exe=None):
    'Launch button with accessibility enabled and return a button object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/button_label_linklabel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    button = Button(app, subproc)

    cache.addApplication(button)

    button.buttonFrame.app = button

    return button

# class to represent the application
class Button(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the Button window'
        super(Button, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Button_Label_LinkLabel controls'), logName='Button')

