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

from os.path import abspath
import pyatspi
import unittest

class Bug537507(TestCase):
    """
    Ensures that the browser does not segfault on exit.
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ValueTest/ValueTest.html'))
        cls.mainFrame = cls.app.mainFrame

        # Poke the textbox to make sure that at least one accessible has been
        # created
        cls.textbox = cls.app.slControl.findText('Hello World!')

    def test(self):
        self.mainFrame.grabFocus()
        self.mainFrame.altF4(assertClosed=True)

        # Block until the app quits.  If we don't quit, we'll be killed by the
        # watchdog timer, and the test will fail as expected.
        self.app.subproc.wait()

        # If returncode is negative, the application was killed by a signal
        # (e.g.: SIGSEGV)
        self.assertEqual(self.app.subproc.returncode, 0)
