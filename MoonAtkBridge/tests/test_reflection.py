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

class ReflectionTest(TestCase):
    """
    Checks that the MoonAtkBridge assembly cannot be accessed even when the moonlight deployment includes it
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ReflectionTest/ReflectionTest.html'))
        cls.button1 = cls.app.slControl.findPushButton('user-code type')
        cls.button2 = cls.app.slControl.findPushButton('non-existant type')
        cls.button3 = cls.app.slControl.findPushButton('corlib private platform type')
        cls.button4 = cls.app.slControl.findPushButton('a11y private platform type')
        cls.button5 = cls.app.slControl.findPushButton('Load the MoonAtkBridge assembly')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_button_basic(self):

        self.assertInterfaces(self.button1, [
            'accessible', 'component', 'action'
        ])
        self.assertInterfaces(self.button2, [
            'accessible', 'component', 'action'
        ])
        self.assertInterfaces(self.button3, [
            'accessible', 'component', 'action'
        ])
        self.assertInterfaces(self.button4, [
            'accessible', 'component', 'action'
        ])
        self.assertInterfaces(self.button5, [
            'accessible', 'component', 'action'
        ])

        button_action = self.button1._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        self.button1.click()
        sleep(config.SHORT_DELAY)
        self.assertEqual('it worked', self.button1.name)

        button_action = self.button2._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        self.button2.click()
        sleep(config.SHORT_DELAY)
        self.assertEqual('Type.GetType returned null', self.button2.name)

        button_action = self.button3._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        self.button3.click()
        sleep(config.SHORT_DELAY)
        self.assertEqual('MAEx', self.button3.name) # MS.NET returns 'Type.GetType returned null', not a big deal

        button_action = self.button4._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        self.button4.click()
        sleep(config.SHORT_DELAY)
        self.assertEqual('MAEx', self.button4.name) # MS.NET returns 'Type.GetType returned null', not a big deal

        button_action = self.button5._accessible.queryAction()
        self.assertEqual(1, button_action.nActions)
        self.button5.click()
        sleep(config.SHORT_DELAY)
        self.assertEqual('it worked', self.button5.name) # MS.NET returns 'exception', low priority to fix
