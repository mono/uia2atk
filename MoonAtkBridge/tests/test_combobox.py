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
#      Mario Carrion <mcarrion@novell.com>
#

from moonlight import *
from strongwind import *

from os.path import abspath
import pyatspi

class ComboBox(TestCase):
    """
    Exercises ComboBox control. ComboBox control implements:
    - UIA.ExpandCollapse pattern, mapped to Action, and
    - UIA.Selection pattern, mapped to Selection
    """

    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ComboBoxTest/ComboBoxTest.html'))
        cls.combobox = cls.app.slControl.findComboBox('ComboBox')
        # 'is' as in "ItemsSource"
        cls.combobox_is = cls.app.slControl.findComboBox('ComboBoxItemsSource')
        cls.button = cls.app.slControl.findPushButton('Button');

        cls.action = cls.combobox._accessible.queryAction()
        cls.action_binding = cls.button._accessible.queryAction()
        cls.ACTIONS_COUNT = 1

        cls.selection = cls.combobox._accessible.querySelection()
        cls.selection_is = cls.combobox_is._accessible.querySelection()
        cls.action_is = cls.combobox_is._accessible.queryAction()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    # Action Tests

    def test_n_actions(self):
        self.assertEqual(self.action.nActions, self.ACTIONS_COUNT)

    def test_do_action(self):
        for x in xrange(0, self.ACTIONS_COUNT):
            self.assertTrue(self.action.doAction(x)) # Expand

        self.assertFalse(self.action.doAction(self.ACTIONS_COUNT))

    def test_get_description(self):
        # By default everything is empty
        self.assertEqual(self.action.getDescription(0), '')
        self.assertEqual(self.action.getDescription(1), '') # Doesn't exist

    def test_get_key_binding(self):
        # By default we'll need an Action to call, this action will change
        # the binding in our tested action. Notice that by default the 0 binding
        # must be set
        for x in xrange(0, self.ACTIONS_COUNT):
            self.assertEqual(self.action.getKeyBinding(x), '<Alt>A')
            self.assertTrue(self.action_binding.doAction(0))
            self.assertEqual(self.action.getKeyBinding(x), '<Alt>B')

        self.assertEqual(self.action.getKeyBinding(self.ACTIONS_COUNT),'')

    def test_get_name(self):
        self.assertEqual(self.action.getName(0), 'expand or collapse')
        self.assertEqual(self.action.getName(1), '') # Doesn't exist

    def test_action_events(self):
        listener = EventListener()

        with listener.listenTo(self.combobox): # Expanded
            self.assertTrue(self.action.doAction(0))
            assert listener.waitForEvent(self.combobox, 'object:visible-data-changed')

        with listener.listenTo(self.combobox): # Collapsed
            self.assertTrue(self.action.doAction(0))
            assert listener.waitForEvent(self.combobox, 'object:visible-data-changed')

    # Selection Tests

    def test_selection(self):
        # We need to expand/collapse the combobox to get the children
        listener = EventListener()
        with listener.listenTo(self.combobox): # expanded
            self.assertTrue(self.action.doAction(0))
            assert listener.containsEvent(self.combobox, 'object:visible-data-changed', 1)

        with listener.listenTo(self.combobox): # collapsed
            self.assertTrue(self.action.doAction(0))
            assert listener.containsEvent(self.combobox, 'object:visible-data-changed', 1)

        self.assertEqual(5, self.combobox.childCount)

        # By default everything is unselected
        for x in xrange(0, self.combobox.childCount):
            item = self.combobox.getChildAtIndex(x)
            self.assertFalse(item.selected)
            self.assertFalse(self.selection.isChildSelected(x))
            # We also check Name
            self.assertEqual('Item %d' % x,item.name)

        # Let's select and unselect
        for x in xrange(0, self.combobox.childCount):
            item = self.combobox.getChildAtIndex(x)

            self.assertTrue(self.selection.selectChild(x))
            self.assertTrue(item.selected)
            self.assertTrue(self.selection.isChildSelected(x))
            self.assertEqual(1, self.selection.nSelectedChildren)

            self.selection.deselectSelectedChild(0)
            self.assertFalse(item.selected)
            self.assertFalse(self.selection.isChildSelected(x))

        # ComboBox supports one selected item
        self.assertFalse(self.selection.selectAll())

        # First item is selected after calling selectAll
        item = self.combobox.getChildAtIndex(0)
        self.assertTrue(item.selected)
        self.assertTrue(self.selection.isChildSelected(0))

        # Unselect it
        self.selection.deselectSelectedChild(0)
        self.assertFalse(item.selected)
        self.assertFalse(self.selection.isChildSelected(0))
        self.assertEqual(self.selection.nSelectedChildren, 0)

    def test_selection_is(self):
        # We need to expand/collapse the combobox (ItemsSource) to get the children
        listener = EventListener()
        with listener.listenTo(self.combobox_is): # expanded
            self.assertTrue(self.action_is.doAction(0))
            assert listener.containsEvent(self.combobox_is, 'object:visible-data-changed', 1)

        with listener.listenTo(self.combobox_is): # collapsed
            self.assertTrue(self.action_is.doAction(0))
            assert listener.containsEvent(self.combobox_is, 'object:visible-data-changed', 1)

        self.assertEqual(3, self.combobox_is.childCount)

        # By default everything is unselected
        for x in xrange(0, self.combobox_is.childCount):
            item = self.combobox_is.getChildAtIndex(x)
            self.assertFalse(item.selected)
            self.assertFalse(self.selection_is.isChildSelected(x))
            # We also check Name. Type
            self.assertEqual('ComboBoxTest.Car', item.name)

        # Let's select and unselect
        for x in xrange(0, self.combobox_is.childCount):
            item = self.combobox_is.getChildAtIndex(x)

            self.assertTrue(self.selection_is.selectChild(x))
            self.assertTrue(item.selected)
            self.assertTrue(self.selection_is.isChildSelected(x))
            self.assertEqual(1, self.selection_is.nSelectedChildren)

            self.selection_is.deselectSelectedChild(0)
            self.assertFalse(item.selected)
            self.assertFalse(self.selection_is.isChildSelected(x))

        # ComboBox supports one selected item
        self.assertFalse(self.selection_is.selectAll())

        # First item is selected after calling selectAll
        item = self.combobox_is.getChildAtIndex(0)
        self.assertTrue(item.selected)
        self.assertTrue(self.selection_is.isChildSelected(0))

        # Unselect it
        self.selection_is.deselectSelectedChild(0)
        self.assertFalse(item.selected)
        self.assertFalse(self.selection_is.isChildSelected(0))
        self.assertEqual(self.selection_is.nSelectedChildren, 0)

    def test_selection_events(self):
        listener = EventListener()

        # We need to expand the combobox to get the children
        with listener.listenTo(self.combobox): # expanded
            self.assertTrue(self.action.doAction(0))
        assert listener.containsEvent(self.combobox, 'object:visible-data-changed', 1)

        with listener.listenTo(self.combobox): # collapsed
            self.assertTrue(self.action.doAction(0))
        assert listener.containsEvent(self.combobox, 'object:visible-data-changed', 1)

        self.assertFalse(self.selection.isChildSelected(0))
        self.assertEqual(5,self.combobox.childCount)

        child0 = self.combobox.getChildAtIndex(0)
        child1 = self.combobox.getChildAtIndex(1)

        for x in xrange(0, self.combobox.childCount):
            item = self.combobox.getChildAtIndex(x)

        with listener.listenTo(child0):
            self.assertTrue(self.selection.selectChild(0))

        self.assertTrue(child0.selected)
        assert listener.containsEvent(child0, 'object:state-changed:selected', 1)

        with listener.listenTo([child0, child1, self.combobox]):
            self.assertTrue(self.selection.selectChild(1))

        self.assertFalse(child0.selected)
        assert listener.containsEvent(child0, 'object:state-changed:selected', 1)

        self.assertTrue(child1.selected)
        assert listener.containsEvent(child1, 'object:state-changed:selected', 1)

        assert listener.containsEvent(self.combobox, 'object:selection-changed', 1)

