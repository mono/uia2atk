
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/09/2009
#              Application wrapper for Moonlight checkbox
#              Used by the checkbox-*.py tests
##############################################################################

'Application wrapper for Moonlight CheckBox'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install 
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchCheckBox(exe=None):
    '''Launch Moonlight CheckBox with accessibility enabled and return a 
     CheckBox object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/CheckBox/CheckBoxSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                        wait=config.LONG_DELAY)

    checkbox = CheckBox(app, subproc)

    cache.addApplication(checkbox)

    checkbox.checkBoxFrame.app = checkbox

    return checkbox

# class to represent the application
class CheckBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the CheckBox window'
        super(CheckBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^CheckBoxSample'), logName='Check Box')

