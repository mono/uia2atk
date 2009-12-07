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

class ScrollBar(TestCase):
    """
    Exercises the ScrollBar control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ScrollBarTest/ScrollBarTest.html'))
        cls.horizontal, cls.vertical = cls.app.slControl.findAllScrollBars('')[:2]
        cls.scrollviewer = cls.app.slControl.findAllPanels('')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_scrollbar_basic(self):
        self.assertEqual('', self.horizontal.name)

        self.assertInterfaces(self.horizontal, [
            'accessible', 'component', 'value',
        ])

        self.assertStates(self.horizontal, [
            'enabled', 'horizontal', 'sensitive',
            'showing', 'visible',
        ])

        self.assertStates(self.vertical, [
            'enabled', 'sensitive', 'showing',
            'vertical', 'visible',
        ])

        self.assertEqual(5, self.horizontal.childCount)

        for i in xrange(0, 4):
            button = self.horizontal.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, button.role)
            self.assertEqual('', button.name)
            self.assertEqual(0, button.childCount)

    def test_scrollbar_value(self):
        horiz_value = self.horizontal._accessible.queryValue()
        vert_value = self.vertical._accessible.queryValue()

        self.assertEqual(0, horiz_value.currentValue)
        self.assertEqual(0, horiz_value.minimumValue)
        self.assertEqual(100, horiz_value.maximumValue)

        # XXX: This is a Microsoft UIA bug that we are forced to copy
        self.assertEqual(0, horiz_value.minimumIncrement)

        self.assertEqual(0, vert_value.currentValue)
        self.assertEqual(0, vert_value.minimumValue)
        self.assertEqual(200, vert_value.maximumValue)

        # XXX: This is a Microsoft UIA bug that we are forced to copy
        self.assertEqual(0, vert_value.minimumIncrement)

    def test_scrollbar_value_events(self):
        horiz_value = self.horizontal._accessible.queryValue()
        self.assertEqual(0, horiz_value.currentValue)

        listener = EventListener()
        with listener.listenTo(self.horizontal):
            horiz_value.currentValue = -100
            self.assertEqual(0, horiz_value.currentValue)

        assert listener.containsEvent(self.horizontal,
                                      'object:property-change:accessible-value',
                                      qty=0)

        with listener.listenTo(self.horizontal):
            horiz_value.currentValue = 40
            self.assertEqual(40, horiz_value.currentValue)

        assert listener.containsEvent(self.horizontal,
                                      'object:property-change:accessible-value',
                                      qty=1)

        with listener.listenTo(self.horizontal):
            horiz_value.currentValue = 1000
            self.assertEqual(40, horiz_value.currentValue)

        assert listener.containsEvent(self.horizontal,
                                      'object:property-change:accessible-value',
                                      qty=0)
