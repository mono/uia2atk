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

class ToggleButton(TestCase):
    """
    Exercises the ToggleButton control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ToggleButtonTest/ToggleButtonTest.html'))
        cls.twostate = cls.app.slControl.findPushButton('Two State')
        cls.threestate = cls.app.slControl.findPushButton('Three State')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_togglebutton_basic(self):
        self.assertEqual('Two State', self.twostate.name)

        self.assertInterfaces(self.twostate, [
            'accessible', 'component', 'action',
        ])

        self.assertStates(self.twostate, [
            'enabled', 'focusable', 'sensitive',
            'showing', 'visible',
        ])

        self.assertEqual(1, self.twostate.childCount)

        label = self.twostate.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_LABEL, label.role)
        self.assertEqual('Two State', label.name)
        self.assertEqual(0, label.childCount)

    def test_togglebutton_actions(self):
        twostate_action = self.twostate._accessible.queryAction()

        self.assertEqual(1, twostate_action.nActions)
        self.assertEqual('click', twostate_action.getName(0))
        self.assertEqual('', twostate_action.getDescription(0))
        self.assertEqual('', twostate_action.getKeyBinding(0))

    def test_togglebutton_twostate_events(self):
        self.assertFalse(self.twostate.checked)
        self.assertFalse(self.twostate.indeterminate)

        listener = EventListener()
        with listener.listenTo(self.twostate):
            self.twostate.click()

            self.assertTrue(self.twostate.checked)

        assert listener.containsEvent(self.twostate,
                                      'object:state-changed:checked',
                                      qty=1)
        assert listener.containsEvent(self.twostate,
                                      'object:state-changed:indeterminate',
                                      qty=0)

        with listener.listenTo(self.twostate):
            self.twostate.click()

            self.assertFalse(self.twostate.checked);

        assert listener.containsEvent(self.twostate,
                                      'object:state-changed:checked',
                                      qty=1)
        assert listener.containsEvent(self.twostate,
                                      'object:state-changed:indeterminate',
                                      qty=0)

    def test_togglebutton_threestate_events(self):
        self.assertFalse(self.threestate.checked)
        self.assertFalse(self.threestate.indeterminate)

        listener = EventListener()
        with listener.listenTo(self.threestate):
            self.threestate.click()

            self.assertTrue(self.threestate.checked)

        assert listener.containsEvent(self.threestate,
                                      'object:state-changed:checked',
                                      qty=1)
        assert listener.containsEvent(self.twostate,
                                      'object:state-changed:indeterminate',
                                      qty=0)

        with listener.listenTo(self.threestate):
            self.threestate.click()

            self.assertFalse(self.threestate.checked)
            self.assertTrue(self.threestate.indeterminate)

        assert listener.containsEvent(self.threestate,
                                      'object:state-changed:checked',
                                      qty=1)
        assert listener.containsEvent(self.threestate,
                                      'object:state-changed:indeterminate',
                                      qty=1)

        with listener.listenTo(self.threestate):
            self.threestate.click()

            self.assertFalse(self.threestate.checked)
            self.assertFalse(self.threestate.indeterminate)

        assert listener.containsEvent(self.threestate,
                                      'object:state-changed:checked',
                                      qty=1)
        assert listener.containsEvent(self.threestate,
                                      'object:state-changed:indeterminate',
                                      qty=1)
