##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/13/2009
# Description: Application wrapper for maskedtextbox.py
#              Used by the maskedtextbox-*.py tests
##############################################################################$

'Application wrapper for maskedtextbox'

from strongwind import *

from os.path import exists
from sys import path

def launchMaskedTextBox(exe=None):
    'Launch maskedTextBox with accessibility enabled and return a maskedtextbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/maskedtextbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    maskedtextbox = MaskedTextBox(app, subproc)

    cache.addApplication(maskedtextbox)

    maskedtextbox.maskedTextBoxFrame.app = maskedtextbox

    return maskedtextbox

# class to represent the application
class MaskedTextBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the maskedtextbox window'
        super(MaskedTextBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Simple MaskedTextBox'), logName='Masked Text Box')
