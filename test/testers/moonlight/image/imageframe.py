
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

    IMAGES_NUM = 2

    def __init__(self, accessible):
        super(ImageFrame, self).__init__(accessible)
        self.frame = self.findDocumentFrame("ImageSample")
        self.filler = self.frame.findFiller("Silverlight Control")
        self.images = self.filler.findAllImages(None)
        assert len(self.images) == self.IMAGES_NUM, \
                             "actual image number:%s, expect:%s" % \
                                 (len(self.images), self.IMAGES_NUM)

        self.image1 = self.images[0]
        self.image2 = self.images[1]
