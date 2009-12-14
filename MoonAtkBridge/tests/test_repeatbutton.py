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

class RepeatButton(TestCase):
    """
    Exercises the RepeatButton control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/RepeatButtonTest/RepeatButtonTest.html'))
        cls.repeatbutton = cls.app.slControl.findPushButton('Loop it!')
        cls.textblock = cls.app.slControl.findLabel('Boom placeholder')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_repeatbutton_basic(self):
        self.assertEqual('Loop it!', self.repeatbutton.name)

        self.assertInterfaces(self.repeatbutton, [
            'accessible', 'component', 'action'
        ])

        self.assertStates(self.repeatbutton, [
            'enabled', 'focusable', 'sensitive',
            'showing', 'visible'
        ])

        self.assertEqual(1, self.repeatbutton.childCount)

        label = self.repeatbutton.getChildAtIndex(0)
        self.assertEqual(pyatspi.ROLE_LABEL, label.role)
        self.assertEqual('Loop it!', label.name)
        self.assertEqual(0, label.childCount)

    def test_repeatbutton_actions(self):
        rb_action = self.repeatbutton._accessible.queryAction()

        self.assertEqual(1, rb_action.nActions)
        self.assertEqual('click', rb_action.getName(0))
        self.assertEqual('', rb_action.getDescription(0))
        self.assertEqual('', rb_action.getKeyBinding(0))

    def test_repeatbutton_click(self):
        self.assertEqual(self.textblock.name, 'Boom placeholder')

        listener = EventListener()
        with listener.listenTo(self.textblock):
            self.repeatbutton.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual(self.textblock.name, 'Boom 1!')

        assert listener.containsEvent(self.textblock,
                                      'object:property-change:accessible-name',
                                      qty=1)

        with listener.listenTo(self.textblock):
            self.repeatbutton.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual(self.textblock.name, 'Boom 2!')

        assert listener.containsEvent(self.textblock,
                                      'object:property-change:accessible-name',
                                      qty=1)
