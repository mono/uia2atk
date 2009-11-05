
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/16
#              Application wrapper for Moonlight passwordbox
#              Used by the passwordbox-*.py tests
##############################################################################

'Application wrapper for Moonlight PasswordBox'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchPasswordBox(exe=None):
    '''Launch Moonlight PasswordBox with accessibility enabled and return a
     PasswordBox object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/PasswordBox/PasswordBoxSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    passwordbox = PasswordBox(app, subproc)

    cache.addApplication(passwordbox)

    passwordbox.passwordBoxFrame.app = passwordbox

    return passwordbox

# class to represent the application
class PasswordBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the PasswordBox window'
        super(PasswordBox, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^PasswordBoxSample'), logName='Password Box')
