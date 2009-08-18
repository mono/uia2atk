# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        03/12/2009
# Description: Application wrapper for combobox_stylechanges.py
##############################################################################

"""Application wrapper for combobox_stylechanges.py"""

from strongwind import *
from os.path import exists
from sys import path

def launchComboBox(exe=None):
    """ 
    Launch the combobox_stylechanges sample application with accessibility
    enabled and return a combobox object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/combobox_stylechanges.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    combobox = ComboBox(app, subproc)

    cache.addApplication(combobox)

    combobox.comboBoxStyleChangesFrame.app = combobox

    return combobox

class ComboBox(accessibles.Application):
    """class to represent the application"""

    def __init__(self, accessible, subproc=None): 
        """Get a reference to the combobox_stylechanges window"""

        super(ComboBox, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ComboBox Style'), logName='Combo Box Style Changes')
