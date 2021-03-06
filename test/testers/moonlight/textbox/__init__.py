##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        11/03/2009
#              Application wrapper for Moonlight textbox
#              Used by the textbox-*.py tests
##############################################################################

'Application wrapper for Moonlight textbox'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchTextBox(exe=None):
    '''Launch Moonlight textbox with accessibility enabled and return a
     textbox object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/TextBox/TextBoxSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                 wait=config.LONG_DELAY)
    textbox = TextBox(app, subproc)

    cache.addApplication(textbox)

    textbox.textBoxFrame.app = textbox

    return textbox

# class to represent the application
class TextBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the TextBox window'
        super(TextBox, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^TextBoxSample'), logName='Text Box')
