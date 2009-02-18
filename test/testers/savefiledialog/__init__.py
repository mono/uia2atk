
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/17/2009
#              Application wrapper for savefiledialog.py
#              Used by the savefiledialog-*.py tests
##############################################################################$

'Application wrapper for savefiledialog'

from strongwind import *

from os.path import exists
from sys import path

def launchSaveFileDialog(exe=None):
    'Launch savefiledialog with accessibility enabled and return a savefiledialog object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/savefiledialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    savefiledialog = SaveFileDialog(app, subproc)

    cache.addApplication(savefiledialog)

    savefiledialog.saveFileDialogFrame.app = savefiledialog

    return savefiledialog

# class to represent the application
class SaveFileDialog(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the savefiledialog window'
        super(SaveFileDialog, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^SaveFileDialog control'), logName='Save File Dialog')

