
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/27
#              Application wrapper for Moonlight listbox
#              Used by the listbox-*.py tests
##############################################################################

'Application wrapper for Moonlight ListBox'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchListBox(exe=None):
    '''Launch Moonlight ListBox with accessibility enabled and return a
     ListBox object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/ListBox/ListBoxSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                        wait=config.LONG_DELAY)

    listbox = ListBox(app, subproc)

    cache.addApplication(listbox)

    listbox.listBoxFrame.app = listbox

    return listbox

# class to represent the application
class ListBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the ListBox window'
        super(ListBox, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ListBoxSample'), logName='List Box')
