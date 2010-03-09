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

class ControlType(TestCase):
    """
    Tests that changing the ControlType of an AutomationPeer will cause the
    Adapter to react properly.
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/ControlTypeTest/ControlTypeTest.html'))
        cls.slControl = cls.app.slControl

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_controltype(self):
        listener = EventListener()

        self.assertEqual(1, self.slControl.childCount)

        subject = Accessible(self.slControl.getChildAtIndex(0))
        self.assertEqual('Hola', subject.name)
        self.assertEqual(pyatspi.ROLE_PUSH_BUTTON, subject.role)

        with listener.listenTo(self.slControl):
            subject.click()

            self.assertEqual(1, self.slControl.childCount)

            subject = Accessible(self.slControl.getChildAtIndex(0))
            self.assertEqual('Hola', subject.name)
            self.assertEqual(pyatspi.ROLE_RADIO_BUTTON, subject.role)

        assert listener.containsEvent(self.slControl,
                                      'object:children-changed:remove',
                                      qty=1)
        assert listener.containsEvent(self.slControl,
                                      'object:children-changed:add',
                                      qty=1)
