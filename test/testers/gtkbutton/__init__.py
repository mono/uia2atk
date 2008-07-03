##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: Application wrapper for gtkbutton.py 
#              Used by the gtkbutton-*.py tests
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

        exe = '%s/samples/gtkbutton.py' % uiaqa_path
   
    if not os.path.exists(exe):
      raise IOError, "%s does not exist" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args)

    button = Button(app, subproc)
    cache.addApplication(button)

    button.buttonFrame.app = button

    return button

# class to represent the application
class Button(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Button window'
        super(Button, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^Buttons'), logName='Button')
