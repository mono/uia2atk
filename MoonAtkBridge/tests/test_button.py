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
from strongwind.errors import NotSensitiveError

import pyatspi
from time import sleep
from os.path import abspath

class Button(TestCase):
    """
    Exercises the Button control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ButtonTest/ButtonTest.html'))
        cls.textblock = cls.app.slControl.findLabel('0')
        cls.button = cls.app.slControl.findPushButton('Add 1')
        cls.toggleSensitivity = cls.app.slControl.findPushButton('Toggle Sensitivity')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_button_basic(self):
        self.assertEqual('Add 1', self.button.name)

        self.assertInterfaces(self.button, [
            'accessible', 'component', 'action'
        ])

        # GTK+ labels also have multi_line, but Orca doesn't seem to care
        self.assertStates(self.button, [
            'enabled', 'focusable', 'sensitive',
            'showing', 'visible'
        ])

        self.assertEqual(1, self.button.childCount)

        label = self.button.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_LABEL, label.role)
        self.assertEqual('Add 1', label.name)
        self.assertEqual(0, label.childCount)

    def test_button_actions(self):
        button_action = self.button._accessible.queryAction()

        self.assertEqual(1, button_action.nActions)
        self.assertEqual('click', button_action.getName(0))
        self.assertEqual('', button_action.getDescription(0))
        self.assertEqual('', button_action.getKeyBinding(0))

    def test_button_click_events(self):
        self.assertEqual('0', self.textblock.name)

        listener = EventListener()
        with listener.listenTo(self.textblock):
            self.button.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual('1', self.textblock.name)

        assert listener.containsEvent(self.textblock,
                                      'object:property-change:accessible-name',
                                      qty=1)

        with listener.listenTo(self.textblock):
            self.button.mouseClick()

            sleep(config.SHORT_DELAY)

            self.assertEqual('2', self.textblock.name)

        assert listener.containsEvent(self.textblock,
                                      'object:property-change:accessible-name',
                                      qty=1)

        # Ensure that insensitive buttons can't be actioned
        self.assertTrue(self.button.sensitive)
        with listener.listenTo(self.button):
            self.toggleSensitivity.click()

            self.assertFalse(self.button.sensitive)

        assert listener.containsEvent(self.button,
                                      'object:state-changed:sensitive',
                                      qty=1)

        self.assertEqual('2', self.textblock.name)
        with listener.listenTo([self.textblock, self.button]):
            self.assertRaises(NotSensitiveError, lambda x: x.click(),
                              self.button)

            self.assertEqual('2', self.textblock.name)

        assert listener.containsEvent(qty=0)
