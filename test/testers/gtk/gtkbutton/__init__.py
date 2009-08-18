##############################################################################$
# Written by:  Brian G. Merrell <bgmerrell@novell.com>$
# Date:        May 23 2008$
# Description: Application wrapper for gtkbutton.py 
#              Used by the gtkbutton-*.py tests
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
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/gtk/gtkbutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args)

    button = GtkButton(app, subproc)
    cache.addApplication(button)

    button.gtkButtonFrame.app = button

    return button

# class to represent the application
class GtkButton(accessibles.Application):
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Button window'
        super(GtkButton, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^Buttons'), logName='Gtk Button')
