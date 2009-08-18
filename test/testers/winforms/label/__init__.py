
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/07/2008
#              Application wrapper for label.py
#              Used by the label-*.py tests
##############################################################################

'Application wrapper for label'

from strongwind import *

from os.path import exists
from sys import path

def launchLabel(exe=None):
    'Launch label with accessibility enabled and return a label object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/winforms/button_label_linklabel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    label = Label(app, subproc)

    cache.addApplication(label)

    label.labelFrame.app = label

    return label

# class to represent the application
class Label(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the label window'
        super(Label, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Button_Label_LinkLabel controls'), logName='Label')

