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

from time import sleep
from os.path import abspath
import pyatspi

class Adapter(TestCase):
    """
    Exercises the basic functionality of the Adapter class
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/RootbeerMaze/RootbeerMaze.html'))

        # Wait until the RootbeerMaze ad has gone away
        sleep(35);

        cls.slAccessible = cls.app.slControl._accessible
        cls.easy_button = cls.app.slControl.findPushButton('Easy')
        cls.easy_button_component = cls.easy_button._accessible.queryComponent()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_states(self):
        self.assertTrue(self.easy_button.showing)
        self.assertTrue(self.easy_button.visible)
        self.assertTrue(self.easy_button.sensitive)
        self.assertTrue(self.easy_button.enabled)
        self.assertTrue(self.easy_button.focusable)

    def test_role(self):
        self.assertEqual(self.easy_button.roleName, "push button")

    def test_get_index_in_parent(self):
        self.assertEqual(self.slAccessible.getIndexInParent(), 0)

        # RootbeerMaze is only ever two accessibles deep
        for i in xrange(0, self.slAccessible.childCount):
            child = self.slAccessible.getChildAtIndex(i)
            self.assertEqual(child.getIndexInParent(), i)

            for j in xrange(0, child.childCount):
                grandchild = child.getChildAtIndex(j)
                self.assertEqual(grandchild.getIndexInParent(), j)

    def test_parent(self):
        self.assertTrue(self.slAccessible.parent is not None)

        # RootbeerMaze is only ever two accessibles deep
        for i in xrange(0, self.slAccessible.childCount):
            child = self.slAccessible.getChildAtIndex(i)
            self.assertEqual(child.parent, self.slAccessible)

            for j in xrange(0, child.childCount):
                grandchild = child.getChildAtIndex(j)
                self.assertEqual(grandchild.parent, child)

    def test_alpha(self):
        self.assertEqual(self.easy_button_component.getAlpha(), 1.0)

    def test_layer(self):
        self.assertEqual(self.easy_button.layer, pyatspi.LAYER_WIDGET)

    def test_contains(self):
        com = self.easy_button_component

        self.assertFalse(com.contains(0, 0, pyatspi.DESKTOP_COORDS))
        self.assertFalse(com.contains(0, 0, pyatspi.WINDOW_COORDS))

        # (132,264)
        #      +---------------+
        #      |               |
        #      |               |
        #      +---------------+
        #                   (312,324)
        self.assertTrue(com.contains(200, 300, pyatspi.WINDOW_COORDS))
        self.assertFalse(com.contains(131, 300, pyatspi.WINDOW_COORDS))
        self.assertFalse(com.contains(313, 300, pyatspi.WINDOW_COORDS))
        self.assertFalse(com.contains(263, 325, pyatspi.WINDOW_COORDS))
        self.assertFalse(com.contains(200, 325, pyatspi.WINDOW_COORDS))

    def test_extents(self):
        extents = self.easy_button_component.getExtents(pyatspi.WINDOW_COORDS)

        self.assertEqual(extents.x, 132)
        self.assertEqual(extents.y, 264)
        self.assertEqual(extents.width, 180)
        self.assertEqual(extents.height, 60)

    def test_position(self):
        position = self.easy_button_component.getPosition(pyatspi.WINDOW_COORDS)

        self.assertEqual(position, (132, 264))

    def test_size(self):
        size = self.easy_button_component.getSize()

        self.assertEqual(size, (180, 60))

    def test_focus(self):
        self.easy_button_component.grabFocus()

        self.assertTrue(self.easy_button.focused)

    def test_get_accessible_at_point(self):
        slComponent = self.slAccessible.queryComponent()
        acc = slComponent.getAccessibleAtPoint(0, 0, pyatspi.WINDOW_COORDS)
        self.assertEqual(acc, self.slAccessible)

        acc = slComponent.getAccessibleAtPoint(140, 280, pyatspi.WINDOW_COORDS)
        self.assertEqual(acc, self.easy_button._accessible)

        acc = slComponent.getAccessibleAtPoint(197, 285, pyatspi.WINDOW_COORDS)
        self.assertEqual(acc, self.easy_button._accessible)

        acc = self.easy_button_component.getAccessibleAtPoint(0, 0, pyatspi.WINDOW_COORDS)
        self.assertTrue(acc is None)

        acc = self.easy_button_component.getAccessibleAtPoint(200, 325, pyatspi.WINDOW_COORDS)
        self.assertTrue(acc is None)

        acc = self.easy_button_component.getAccessibleAtPoint(140, 280, pyatspi.WINDOW_COORDS)
        self.assertEqual(acc, self.easy_button._accessible)

        acc = self.easy_button_component.getAccessibleAtPoint(197, 285, pyatspi.WINDOW_COORDS)
        self.assertTrue(acc.getRoleName(), "label")
        self.assertTrue(acc.name, "Easy")
