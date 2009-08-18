
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        10/21/2008
#              Application wrapper for linklabel.py
#              Used by the linklabel-*.py tests
##############################################################################

'Application wrapper for linklabel'

from strongwind import *

from os.path import exists
from sys import path

def launchLinkLabel(exe=None):
    'Launch linklabel with accessibility enabled and return a linklabel object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        j = harness_dir[:i].rfind("/")
        uiaqa_path = harness_dir[:j]
        exe = '%s/samples/winforms/linklabel.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    linklabel = LinkLabel(app, subproc)

    cache.addApplication(linklabel)

    linklabel.linkLabelFrame.app = linklabel

    return linklabel

# class to represent the application
class LinkLabel(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        'Get a reference to the label window'
        super(LinkLabel, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^LinkLabel control'), logName='Link Label')

# lauch a new application when click link label
def launchNewApp(applicationName):
        
    application = pyatspi.findDescendant(cache._desktop, lambda x: x.name == applicationName, True)

    cache.addApplication(application)

    return application

