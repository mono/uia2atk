
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        07/24/2008
# Description: Application wrapper for CheckBox.py
#              Used by the checkbox-*.py tests
##############################################################################$

'Application wrapper for chexkbox'

from strongwind import *

from os.path import exists
from sys import path

def launchCheckBox(exe=None):
    'Launch checkbox with accessibility enabled and return a checkbox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/checkbox_radiobutton.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    checkbox = CheckBox(app, subproc)

    cache.addApplication(checkbox)

    checkbox.checkBoxFrame.app = checkbox

    return checkbox

# class to represent the application
class CheckBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the Button window'
        super(CheckBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^CheckBox_RadioButton controls'), logName='Check Box')

