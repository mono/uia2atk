
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
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/form.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    form = Form(app, subproc)

    cache.addApplication(form)

    form.formFrame.app = form

    return form

# class to represent the application
class Form(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the form window'
        super(Form, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Form control'), logName='Form')

