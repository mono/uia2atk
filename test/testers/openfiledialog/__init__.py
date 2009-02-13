
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/11/2009
#              Application wrapper for openfiledialog.py
#              Used by the openfiledialog-*.py tests
##############################################################################$

'Application wrapper for openfiledialog'

from strongwind import *

from os.path import exists
from sys import path

def launchOpenFileDialog(exe=None):
    'Launch openfiledialog with accessibility enabled and return a openfiledialog object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/openfiledialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    openfiledialog = OpenFileDialog(app, subproc)

    cache.addApplication(openfiledialog)

    openfiledialog.openFileDialogFrame.app = openfiledialog

    return openfiledialog

# class to represent the application
class OpenFileDialog(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the printpreviewdialog window'
        super(OpenFileDialog, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^OpenFileDialog control'), logName='Open File Dialog')

