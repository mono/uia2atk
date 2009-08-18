# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Brian G. Merrell <bgmerrell@novell.com>
# Date:        12/03/2008
# Description: Application wrapper for helpprovider.py
#              Used by the helprovider-*.py tests
##############################################################################$

'''Application wrapper for helpprovider.py'''

from strongwind import *
from os.path import exists
from sys import path

def launchHelpProvider(exe=None):
    '''
    Launch helpprovider.py with accessibility enabled and return a helpprovider
    object.  Log an error and return None if something goes wrong
    '''

    if exe is None:
        # make sure we can find the sample application
        harness_dir = path[0]
        i = harness_dir.rfind("/")
        uiaqa_path = harness_dir[:i]
        exe = '%s/samples/helpprovider.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    helpprovider = HelpProvider(app, subproc)

    cache.addApplication(helpprovider)

    helpprovider.helpProviderFrame.app = helpprovider

    return helpprovider

# class to represent the application
class HelpProvider(accessibles.Application):

    def __init__(self, accessible, subproc=None): 
        '''Get a reference to the helpprovider window'''
        super(HelpProvider, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^Help Provider Demonstration'), logName='Help Provider')
