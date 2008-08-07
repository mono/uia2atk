
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/29/2008
# Description: Application wrapper for radiobutton.py
#              Used by the radiobutton-*.py tests
##############################################################################$

'Application wrapper for radiobutton'

from strongwind import *

from os.path import exists
from sys import path

def launchRadioButton(exe=None):
    'Launch radiobutton with accessibility enabled and return a radiobutton object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/checkbox_radiobutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy')

    radiobutton = RadioButton(app, subproc)

    cache.addApplication(radiobutton)

    radiobutton.radioButtonFrame.app = radiobutton

    return radiobutton

# class to represent the application
class RadioButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the RadioButton window'
        super(RadioButton, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^CheckBox_RadioButton controls'), logName='Radio Button')

