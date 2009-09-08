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

from os.path import abspath
import pyatspi
import unittest

class SelectionListBox(unittest.TestCase):
    """
    Exercises the basic functionality of the Selection class using a ListBox
    control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/SelectionTest/SelectionTest.html'))

        cls.list = cls.app.slControl.findAllLists('')[0]
        cls.list_selection = cls.list._accessible.querySelection()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_selection(self):
        """
        Should test all of:
            - selectChild
            - clearSelection
            - getSelectedChild
            - nSelectedChildren
            - isChildSelected
            - deselectChild
            - selectAll
        """
        for i in xrange(0, self.list.childCount):
            self.assertTrue(self.list_selection.selectChild(i))

            item = self.list.getChildAtIndex(i)

            self.assertTrue(item.selected)
            self.assertTrue(self.list_selection.isChildSelected(i))

            self.assertEqual(self.list_selection.getSelectedChild(0), item._accessible)
            self.assertEqual(self.list_selection.nSelectedChildren, 1)

            for j in xrange(0, self.list.childCount):
                if i == j:
                    continue

                self.assertFalse(self.list.getChildAtIndex(j).selected)

            self.list_selection.deselectSelectedChild(0)
            self.assertEqual(self.list_selection.nSelectedChildren, 0)

        self.assertTrue(self.list_selection.selectChild(0))
        self.list_selection.clearSelection()

        for i in xrange(0, self.list.childCount):
            self.assertFalse(self.list_selection.isChildSelected(i))

        # ListBox only supports single selection
        self.assertFalse(self.list_selection.selectAll())

        # but the first item should still be selected
        item = self.list.getChildAtIndex(0)
        self.assertTrue(item.selected)

        self.assertTrue(self.list_selection.deselectSelectedChild(0))
        self.assertFalse(item.selected)
        self.assertEqual(self.list_selection.nSelectedChildren, 0)

    def test_selection_events(self):
        """
        Verify that events are being properly raised.
        """
        listener = EventListener()

        child0 = self.list.getChildAtIndex(0)
        child1 = self.list.getChildAtIndex(1)

        with listener.listenTo(child0):
            self.assertTrue(self.list_selection.selectChild(0))

        assert listener.containsEvent(child0, 'object:state-changed:selected', 1)
        self.assertTrue(child0.selected)

        with listener.listenTo([child0, child1]):
            self.assertTrue(self.list_selection.selectChild(1))

        assert listener.containsEvent(child0, 'object:state-changed:selected', 1)
        self.assertFalse(child0.selected)

        assert listener.containsEvent(child1, 'object:state-changed:selected', 1)
        self.assertTrue(child1.selected)
