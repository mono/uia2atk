
##############################################################################
# Written by:  Cachen Chen <cachen@novell.com>
# Date:        09/16/2009
# Description: image.py wrapper script
#              Used by the image-*.py tests
##############################################################################

import sys
import os

from strongwind import *
from image import *


# class to represent the main window.
class ImageFrame(accessibles.Frame):

    # constants
    # the available widgets on the window
    IMAGE_ONE = "jpg"
    IMAGE_TWO = "png"

    def __init__(self, accessible):
        super(ImageFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ImageSample")
        self.image1 = self.frame.findImage(self.IMAGE_ONE)
        self.image2 = self.frame.findImage(self.IMAGE_TWO)
