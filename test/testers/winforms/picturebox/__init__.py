
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        08/14/2008
#              Application wrapper for picturebox.py
#              Used by the picturebox-*.py tests
##############################################################################$

'Application wrapper for picturebox'

from strongwind import *

from os.path import exists
from sys import path

harness_dir = path[0]
i = harness_dir.rfind("/")
uiaqa_path = harness_dir[:i]

def launchPictureBox(exe=None):
    'Launch picturebox with accessibility enabled and return a picturebox object.  Log an error and return None if something goes wrong'

    if exe is None:
        # make sure we can find the sample application
        exe = '%s/samples/picturebox.py' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [exe]

    (app, subproc) = cache.launchApplication(args=args, name='ipy', wait=config.LONG_DELAY)

    picturebox = PictureBox(app, subproc)

    cache.addApplication(picturebox)

    picturebox.pictureBoxFrame.app = picturebox

    return picturebox

# class to represent the application
class PictureBox(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the picturebox window'
        super(PictureBox, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^PictureBox control'), logName='Picture Box')

