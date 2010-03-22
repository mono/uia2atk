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
#      Mario Carrion <mcarrion@novell.com>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener

from os.path import abspath
import pyatspi

class GridSplitter(TestCase):
    """
    Exercises GridSplitter control. GridSplitter implements RangeValue.
    - Value event is not raised by GridSplitter
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/GridSplitterTest/GridSplitterTest.html'))
        cls.hpane = cls.app.slControl.findAllSplitPanes('')[0]
        cls.value_h = cls.hpane._accessible.queryValue()

        cls.vpane = cls.app.slControl.findAllSplitPanes('')[1]
        cls.value_v = cls.vpane._accessible.queryValue()

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_gridsplitter_basic_h(self):
        self.assertEqual('', self.hpane.name)

        self.assertInterfaces(self.hpane, [
            'accessible', 'component', 'value'
        ])

        self.assertStates(self.hpane, [
            'enabled', 'sensitive', 'focusable',
            'showing',  'visible'
        ])

        # Children
        self.assertEqual(0, self.hpane.childCount)

    def test_minimum_value(self):
        self.assertEqual(self.value_v.minimumValue, 0)
        self.assertEqual(self.value_h.minimumValue, 0)

    def test_maximum_value(self):
        self.assertEqual(self.value_v.maximumValue, 100)
        self.assertEqual(self.value_h.maximumValue, 100)

    def test_minimum_increment(self):
        self.assertEqual(self.value_h.minimumIncrement, 0)
        self.assertEqual(self.value_v.minimumIncrement, 0)

    def test_current_value_h(self):
        # Horizontal Pane
        self.value_h.currentValue = 50
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 50)

        self.value_h.currentValue = 20
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 20)

        self.value_h.currentValue = 50
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 50)

        self.value_h.currentValue = -10
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 50)

        self.value_h.currentValue = 24
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 24)

        self.value_h.currentValue = 90
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 90)

        self.value_h.currentValue = 150
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_h.currentValue, 90)

    def test_current_value_v(self):
        # Horizontal Pane
        self.assertEqual(self.value_v.currentValue, 50)

        self.value_v.currentValue = 30
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 30)

        self.value_v.currentValue = 10
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 10)

        self.value_v.currentValue = -10
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 10)

        self.value_v.currentValue = 90
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 90)

        self.value_v.currentValue = 40
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 40)

        self.value_v.currentValue = 101
        sleep(config.SHORT_DELAY)
        self.assertEqual(self.value_v.currentValue, 40)
