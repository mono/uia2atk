# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        02/20/2008
# Description: Application wrapper for textbox.py
#              be called by ../textbox_basic_ops.py
##############################################################################

"""Application wrapper for textbox.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchTextBox(exe=None):
    """ 
    Launch textbox with accessibility enabled and return a textbox object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/textbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    textbox = TextBox(app, subproc)

    cache.addApplication(textbox)

    textbox.textBoxFrame.app = textbox

    return textbox

class TextBox(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the textbox window"""

        super(TextBox, self).__init__(accessible, subproc)
        self.findFrame(re.compile('^TextBox'), logName='Text Box')
