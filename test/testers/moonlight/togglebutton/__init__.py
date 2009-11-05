
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/14/2009
#              Application wrapper for Moonlight togglebutton
#              Used by the togglebutton-*.py tests
##############################################################################

'Application wrapper for Moonlight togglebutton'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install 
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchToggleButton(exe=None):
    '''Launch Moonlight togglebutton with accessibility enabled and return a 
     togglebutton object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/ToggleButton/ToggleButtonSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)
    togglebutton = ToggleButton(app, subproc)

    cache.addApplication(togglebutton)

    togglebutton.toggleButtonFrame.app = togglebutton

    return togglebutton

# class to represent the application
class ToggleButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the ToggleButton window'
        super(ToggleButton, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ToggleButtonSample'), logName='Toggle Button')

