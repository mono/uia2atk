
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/11/2009
#              Application wrapper for Moonlight radiobutton
#              Used by the radiobutton-*.py tests
##############################################################################

'Application wrapper for Moonlight radiobutton'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install 
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchRadioButton(exe=None):
    '''Launch Moonlight RadioButton with accessibility enabled and return a 
     RadioButton object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/RadioButton/RadioButtonSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                        wait=config.LONG_DELAY)
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
        
        self.findFrame(re.compile('^RadioButtonSample'), logName='Radio Button')

