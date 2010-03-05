
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/14
#              Application wrapper for Moonlight button
#              Used by the button-*.py tests
##############################################################################

'Application wrapper for Moonlight Button'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchButton(exe=None):
    '''Launch Moonlight Button with accessibility enabled and return a
     Button object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/Button/ButtonSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    button = Button(app, subproc)

    cache.addApplication(button)

    button.buttonFrame.app = button

    return button

# class to represent the application
class Button(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Button window'
        super(Button, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ButtonSample'), logName='Button')
