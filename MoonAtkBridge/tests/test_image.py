#!/usr/bin/env python

# Permission is hereby granted, free of charge, to any person obtaining
# a copy of this software and associated documentation files (the
# "Software"), to deal in the Software without restriction, including
# without limitation the rights to use, copy, modify, merge, publish,
# distribute, sublicense, and/or sell copies of the Software, and to
# permit persons to whom the Software is furnished to do so, subject to
# the following conditions:
#
# The above copyright notice and this permission notice shall be
# included in all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
# NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
# LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
# OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
# WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#
# Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
#
# Authors:
#      Brad Taylor <brad@getcoded.net>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener

from os.path import abspath
import pyatspi

class Image(TestCase):
    """
    Exercises the basic functionality of the Image class using an Image control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ImageTest/ImageTest.html'))
        cls.image = cls.app.slControl.findAllImages('')[0]

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_image_basic(self):
        self.assertEqual('', self.image.name)
        self.assertEqual(pyatspi.ROLE_IMAGE, self.image.role)

        self.assertInterfaces(self.image, [
            'accessible', 'component'
        ])

        self.assertStates(self.image, [
            'enabled', 'sensitive', 'showing', 'visible'
        ])

        self.assertEqual(0, self.image.childCount)
