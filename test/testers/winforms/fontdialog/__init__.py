
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        02/20/2009
#              Application wrapper for fontdialog.py
#              Used by the fontdialog-*.py tests
##############################################################################$

'Application wrapper for fontdialog'

from strongwind import *

from os.path import exists
from sys import path

def launchFontDialog(exe=None):
    'Launch fontdialog with accessibility enabled and return a fontdialog object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/fontdialog.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    fontdialog = FontDialog(app, subproc)

    cache.addApplication(fontdialog)

    fontdialog.fontDialogFrame.app = fontdialog

    return fontdialog

# class to represent the application
class FontDialog(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the fontdialog window'
        super(FontDialog, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^FontDialog control'), logName='Font Dialog')

