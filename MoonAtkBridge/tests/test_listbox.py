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

LISTBOX_FRUITS = (
    'Apple', 'Blueberry', 'Currant',
    'Durian', 'Entawak', 'Fig',
    'Grape', 'Huckleberry', 'Ita Palm',
    'Jujube', 'Kiwi', 'Lime',
    'Mango', 'Nectarine', 'Orange',
    'Pineapple', 'Quince', 'Rasin',
    'Strawberry', 'Tangerine', 'Ugli',
    'Voavanga',
)

class ListBox(TestCase):
    """
    Exercises the ListBox control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ListBoxTest/ListBoxTest.html'))
        cls.list = cls.app.slControl.findList('')
        cls.list_selection = cls.list._accessible.querySelection()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_listbox_basic(self):
        self.assertEqual('', self.list.name)

        self.assertInterfaces(self.list, [
            'accessible', 'component', 'selection',
        ])

        self.assertStates(self.list, [
            'enabled', 'sensitive', 'showing', 'visible',
        ])

        self.assertEqual(22, self.list.childCount)

        for i in xrange(0, 21):
            item = self.list.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_LIST_ITEM, item.role)
            self.assertEqual('', item.name)

            self.assertStates(item, [
                'enabled', 'focusable', 'selectable',
                'sensitive', 'showing', 'visible',
            ])

            self.assertEqual(1, item.childCount)
            label = item.getChildAtIndex(0)
            self.assertEqual(pyatspi.ROLE_LABEL, label.role)
            self.assertEqual(LISTBOX_FRUITS[i], label.name)

    def test_listbox_events(self):
        listener = EventListener()

        apple = self.list.getChildAtIndex(0)
        blueberry = self.list.getChildAtIndex(1)

        with listener.listenTo([apple, blueberry]):
            self.assertTrue(self.list_selection.selectChild(0))

            self.assertTrue(apple.selected)
            self.assertFalse(blueberry.selected)

        assert listener.containsEvent(apple, 'object:state-changed:selected', 1)
        assert listener.containsEvent(blueberry, 'object:state-changed:selected', 0)

        with listener.listenTo([apple, blueberry]):
            self.assertTrue(self.list_selection.selectChild(1))

            self.assertFalse(apple.selected)
            self.assertTrue(blueberry.selected)

        assert listener.containsEvent(apple, 'object:state-changed:selected', 1)
        assert listener.containsEvent(blueberry, 'object:state-changed:selected', 1)

        with listener.listenTo([apple, blueberry]):
            self.assertTrue(self.list_selection.clearSelection())

            self.assertFalse(apple.selected)
            self.assertFalse(blueberry.selected)

        assert listener.containsEvent(apple, 'object:state-changed:selected', 0)
        assert listener.containsEvent(blueberry, 'object:state-changed:selected', 1)
