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
#      Andres G. Aragoneses <aaragoneses@novell.com>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener
from strongwind.errors import NotSensitiveError

import pyatspi
from time import sleep
from os.path import abspath

class FakeBridgeTest(TestCase):
    """
    Checks that a MoonAtkBridge assembly cannot be loaded after our official MoonAtkBridge with the same privileges
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/FakeBridgeTest/FakeBridgeTest.html'))
        cls.button1 = cls.app.slControl.findPushButton('attack')
        cls.button2 = cls.app.slControl.findPushButton('Status: Ready')

    @classmethod
    def teardown_class(cls):
        try:
            cls.app.kill()
        except :
            pass

    def test_button_basic(self):

        self.assertInterfaces(self.button1, [
            'accessible', 'component', 'action'
        ])

        button_action = self.button1._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        try :
           self.button1.click()
        except :
           pass

        sleep(config.SHORT_DELAY)
        new_name = ""
        try:
            new_name = self.button2.name
        except :
            pass

        self.assertNotEqual('it worked', new_name)

