
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/11/2008
#              Application wrapper for form.py
#              Used by the form-*.py tests
##############################################################################

'Application wrapper for form'

from strongwind import *

from os.path import exists
from sys import path

def launchForm(exe=None):
    'Launch form with accessibility enabled and return a form object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/gtk/gtkform.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    #(app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)
    (app, subproc) = cache.launchApplication(args=args)

    form = GtkForm(app, subproc)

    cache.addApplication(form)

    form.gtkFormFrame.app = form

    return form

# class to represent the application
class GtkForm(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the form window'
        super(GtkForm, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Forms'), logName='Gtk Form')

