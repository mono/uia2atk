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

import pyatspi
from time import sleep
from os.path import abspath

class RadioButton(TestCase):
    """
    Exercises the RadioButton control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/RadioButtonTest/RadioButtonTest.html'))
        cls.apple = cls.app.slControl.findRadioButton('Apple')
        cls.banana = cls.app.slControl.findRadioButton('Banana')
        cls.watermelon = cls.app.slControl.findRadioButton('Watermelon')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_radiobutton_basic(self):
        self.assertEqual('Apple', self.apple.name)

        self.assertInterfaces(self.apple, [
            'accessible', 'component', 'action',
        ])

        self.assertStates(self.apple, [
            # XXX: Missing focusable (#553160)
            'enabled', 'selectable',
            'sensitive', 'showing', 'visible',
        ])

        self.assertEqual(1, self.apple.childCount)

        label = self.apple.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_LABEL, label.role)
        self.assertEqual('Apple', label.name)
        self.assertEqual(0, label.childCount)

    def test_radiobutton_actions(self):
        apple_action = self.apple._accessible.queryAction()

        self.assertEqual(1, apple_action.nActions)
        self.assertEqual('click', apple_action.getName(0))
        self.assertEqual('', apple_action.getDescription(0))
        self.assertEqual('', apple_action.getKeyBinding(0))

    def test_radiobutton_click_events(self):
        self.assertFalse(self.apple.selected)
        self.assertFalse(self.banana.selected)
        self.assertFalse(self.watermelon.selected)

        listener = EventListener()
        with listener.listenTo(self.apple):
            self.apple.click()

            self.assertTrue(self.apple.selected)
            self.assertFalse(self.banana.selected)
            self.assertFalse(self.watermelon.selected)

        assert listener.containsEvent(self.apple,
                                      'object:state-changed:selected',
                                      qty=1)

        with listener.listenTo([self.apple, self.banana]):
            self.banana.click()

            self.assertFalse(self.apple.selected)
            self.assertTrue(self.banana.selected)
            self.assertFalse(self.watermelon.selected)

        assert listener.containsEvent(self.apple,
                                      'object:state-changed:selected',
                                      qty=1)

        assert listener.containsEvent(self.banana,
                                      'object:state-changed:selected',
                                      qty=1)
