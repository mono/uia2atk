
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/27
#              Application wrapper for Moonlight repeatbutton
#              Used by the repeatbutton-*.py tests
##############################################################################

'Application wrapper for Moonlight RepeatButton'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/local/bin/firefox'

def launchRepeatButton(exe=None):
    '''Launch Moonlight RepeatButton with accessibility enabled and return a
     RepeatButton object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/RepeatButton/RepeatButtonSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)

    repeatbutton = RepeatButton(app, subproc)

    cache.addApplication(repeatbutton)

    repeatbutton.repeatButtonFrame.app = repeatbutton

    return repeatbutton

# class to represent the application
class RepeatButton(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the RepeatButton window'
        super(RepeatButton, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^RepeatButtonSample'), logName='Repeat Button')
