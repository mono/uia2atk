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

from time import sleep
from os.path import abspath
import pyatspi

class AddRemoveAdapter(TestCase):
    """
    Exercises the basic functionality of the Adapter class
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/AddRemoveAdapterTest/AddRemoveAdapterTest.html'))

        cls.list = cls.app.slControl.findAllLists('')[0]
        cls.add_button = cls.app.slControl.findPushButton('Add Item')
        cls.remove_button = cls.app.slControl.findPushButton('Remove Item')

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_add_remove_events(self):
        def get_list_item_name(list, index):
            return list.getChildAtIndex(index).getChildAtIndex(0).name

        listener = EventListener()

        self.assertEqual(1, self.list.childCount)
        self.assertEqual('Item 1', get_list_item_name(self.list, 0))

        with listener.listenTo(self.list):
            self.add_button.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual(2, self.list.childCount)
            self.assertEqual('Item 1', get_list_item_name(self.list, 0))
            self.assertEqual('Item 2', get_list_item_name(self.list, 1))

        assert listener.containsEvent(self.list,
                                      'object:children-changed:add',
                                      qty=1)

        with listener.listenTo(self.list):
            self.remove_button.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual(1, self.list.childCount)
            self.assertEqual('Item 1', get_list_item_name(self.list, 0))

        assert listener.containsEvent(self.list,
                                      'object:children-changed:remove',
                                      qty=1)

        with listener.listenTo(self.list):
            self.add_button.click()
            self.add_button.click()

            sleep(config.SHORT_DELAY)

            self.assertEqual(3, self.list.childCount)
            self.assertEqual('Item 1', get_list_item_name(self.list, 0))
            self.assertEqual('Item 3', get_list_item_name(self.list, 1))
            self.assertEqual('Item 4', get_list_item_name(self.list, 2))

        assert listener.containsEvent(self.list,
                                      'object:children-changed:add',
                                      qty=2)
