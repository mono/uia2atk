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
# Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
#
# Authors:
#      Mario Carrion  <mcarrion@novell.com>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener

import pyatspi
from time import sleep
from os.path import abspath

class TabControl(TestCase):
    """
    Exercises the TabControl control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/TabControlTest/TabControlTest.html'))
        cls.tab = cls.app.slControl.findPageTabList('')
        cls.list_selection = cls.tab._accessible.querySelection()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_tabcontrol_basic(self):
        self.assertEqual('', self.tab.name)

        self.assertInterfaces(self.tab, [
            'accessible', 'component', 'selection',
        ])

        self.assertStates(self.tab, [
            'enabled', 'sensitive', 'showing', 'visible',
        ])

        self.assertEqual(6, self.tab.childCount)

        self.assertEqual(pyatspi.ROLE_PAGE_TAB_LIST, self.tab.role)

        for i in xrange(0, 6):
            item = self.tab.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_PAGE_TAB, item.role)
            self.assertEqual('TabItem%s' % i, item.name)

            if i == 1:
                self.assertStates(item, [
                    'enabled',   'focusable', 'selected', 'selectable',
                    'sensitive', 'showing',   'visible',
                ])
                self.assertEqual(1, item.childCount)
            else:
                self.assertStates(item, [
                    'enabled', 'selectable', 'sensitive',
                    'showing', 'visible',
                ])
                self.assertEqual(0, item.childCount)

    def test_tabcontrol_events(self):
        listener = EventListener()

        tabItem0 = self.tab.getChildAtIndex(0)
        tabItem1 = self.tab.getChildAtIndex(1)
        tabItem2 = self.tab.getChildAtIndex(2)

        with listener.listenTo([tabItem0, tabItem1]):
            self.assertTrue(self.list_selection.selectChild(0))

            self.assertTrue(tabItem0.selected)
            self.assertFalse(tabItem1.selected)

        sleep(config.SHORT_DELAY)

        assert listener.containsEvent(tabItem0, 'object:state-changed:selected', 1)
        assert listener.containsEvent(tabItem1, 'object:state-changed:selected', 1)

        with listener.listenTo([tabItem0, tabItem2]):
            self.assertTrue(self.list_selection.selectChild(2))

            self.assertFalse(tabItem0.selected)
            self.assertTrue(tabItem2.selected)

        sleep(config.SHORT_DELAY)

        assert listener.containsEvent(tabItem0, 'object:state-changed:selected', 1)
        assert listener.containsEvent(tabItem2, 'object:state-changed:selected', 1)
