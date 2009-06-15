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

import pyatspi
import unittest

class ExpandCollapseToggleInvokeButton(unittest.TestCase):
    """
    Exercises the basic functionality of the ExpandCollapseToggleInvoke pattern
    implementor for a button
    """
    @classmethod
    def setup_class(cls):
        # TODO: Host test apps in SVN
        cls.app = launchAddress('http://www.rootbeergames.com/games/rootbeerMaze/')

        # TODO: change this when buttons have a correct Name property
        cls.buttons = cls.app.slControl.findAllPushButtons('')
        cls.easy_button = cls.buttons[0]
        cls.easy_button_actions = cls.easy_button._accessible.queryAction()

    def test_actions_basic(self):
        act = self.easy_button_actions
        self.assertEqual(act.nActions(), 1)

        self.assertEqual(act.getName(0), "click")
        self.assertEqual(act.getName(1), "")
        self.assertEqual(act.getName(-1), "")

        self.assertEqual(act.getDescription(0), "")
        self.assertEqual(act.getDescription(1), "")
        self.assertEqual(act.getDescription(-1), "")

        self.assertEqual(act.getKeyBinding(0), "")
        self.assertEqual(act.getKeyBinding(1), "")
        self.assertEqual(act.getKeyBinding(-1), "")

        # TODO: Test for events that are fired due to click
