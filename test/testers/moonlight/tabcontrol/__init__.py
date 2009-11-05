
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/21/2009
#              Application wrapper for Moonlight tabcontrol
#              Used by the tabcontrol-*.py tests
##############################################################################

'Application wrapper for Moonlight tabcontrol'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchTabControl(exe=None):
    '''Launch Moonlight tabcontrol with accessibility enabled and return a 
     tabcontrol object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/TabControl/TabControlSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)
    tabcontrol = TabControl(app, subproc)

    cache.addApplication(tabcontrol)

    tabcontrol.tabControlFrame.app = tabcontrol

    return tabcontrol

# class to represent the application
class TabControl(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the TabControl window'
        super(TabControl, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^TabControlSample'), logName='Tab Control')

