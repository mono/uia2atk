
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2009
#              Application wrapper for Moonlight image
#              Used by the image-*.py tests
##############################################################################

'Application wrapper for Moonlight image'

from strongwind import *

from os.path import exists, dirname
from sys import path

init_dir = path[0]
uiaqa_path = dirname(dirname(init_dir))
# Variable the path of Firefox to run the application, Please install 
# Firefox3.5.1 first which is accessible by accerciser
firefox_path = '/usr/local/bin/firefox'

def launchImage(exe=None):
    '''Launch Moonlight image with accessibility enabled and return a 
     image object.  Log an error and return None if something goes wrong'''

    if exe is None:
        # make sure we can find the sample applications
        exe = '%s/samples/moonlight/Image/ImageSample.html' % uiaqa_path
        if not exists(exe):
          raise IOError, "Could not find file %s" % exe
  
    args = [firefox_path, exe]

    (app, subproc) = cache.launchApplication(args=args, name="Minefield", \
                                                        wait=config.LONG_DELAY)
    image = Image(app, subproc)

    cache.addApplication(image)

    image.imageFrame.app = image

    return image

# class to represent the application
class Image(accessibles.Application):
    #checkShowing=False
    def __init__(self, accessible, subproc=None): 
        'Get a reference to the image window'
        super(Image, self).__init__(accessible, subproc)
        
        self.findFrame(re.compile('^ImageSample'), logName='Image')

