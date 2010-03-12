
##############################################################################
# Written by:  Neville Gao  <nevillegao@gmail.com>
# Date:        2009/09/27
#              Application wrapper for Moonlight thumb
#              Used by the thumb-*.py tests
##############################################################################

'Application wrapper for Moonlight Thumb'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/bin/firefox'

def launchThumb(exe=None):
    '''Launch Moonlight Thumb with accessibility enabled and return a
     Thumb object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/Thumb/ThumbSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe

    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Firefox", \
                                                        wait=config.LONG_DELAY)

    thumb = Thumb(app, subproc)

    cache.addApplication(thumb)

    thumb.thumbFrame.app = thumb

    return thumb

# class to represent the application
class Thumb(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None):
        'Get a reference to the Thumb window'
        super(Thumb, self).__init__(accessible, subproc)

        self.findFrame(re.compile('^ThumbSample'), logName='Thumb')
