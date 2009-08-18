##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        01/20/2009
# Description: Application wrapper for richtextbox.py
#              Used by the richtextbox-*.py tests
##############################################################################$

'Application wrapper for richtextbox'

from strongwind import *

from os.path import exists
from sys import path

def launchRichTextBox(exe=None):
    """
    Launch TextBox with accessibility enabled and return a richtextbox object.
    Log an error and return None if something goes wrong
    """

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/richtextbox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    richtextbox = RichTextBox(app, subproc)

    cache.addApplication(richtextbox)

    richtextbox.richTextBoxFrame.app = richtextbox

    return richtextbox

# class to represent the application
class RichTextBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the richtextbox window'
        super(RichTextBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Simple RichTextBox'), logName='Rich Text Box')
