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

from os.path import abspath
import pyatspi

class DataGrid(TestCase):
    """
    Exercises DataGrid control. DataGrid control implements:
    - UIA.Selection pattern, mapped to Selection
    - UIA.Table/UIA.Grid pattern, mapped to GridTable
    - UIA.Scroll pattern, NOT mapped.
    """

    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/DataGridTest/DataGridTest.html'))

        cls.datagrid = cls.app.slControl.findAllTables('')[0]
        cls.selection = cls.datagrid._accessible.querySelection()
        cls.table = cls.datagrid._accessible.queryTable()

        cls.textblock = cls.app.slControl.findLabel('3')

        cls.add_button = cls.app.slControl.findPushButton('Add Row')
        cls.remove_button = cls.app.slControl.findPushButton('Remove Row')


    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    # Basic Tests

    def test_datagrid_basic(self):
        self.assertEqual('', self.datagrid.name)

        self.assertInterfaces(self.datagrid, [
            'accessible', 'component', 'selection', 'table'
        ])

        self.assertStates(self.datagrid, [
            'enabled', 'focusable', 'sensitive', 'showing',
            'visible'
        ])

        # Table Row header, Table Cells, 2 Scrollbars
        self.assertEqual(4, self.datagrid.childCount)

    # We don't test Selection because children in DataGrid don't
    # implement SelectionItem.

    def test_grid_table(self):
        self.assertEqual(3, self.table.nColumns)
        self.assertEqual(int(self.textblock.name), self.table.nRows)

        self.add_button.click()
        self.assertEqual(3, self.table.nColumns)
        self.assertEqual(int(self.textblock.name), self.table.nRows)

    # ScrollBar Visibility
    def test_scrollbars_visibility(self):
        vertical = self.datagrid.getChildAtIndex(2)
        horizontal = self.datagrid.getChildAtIndex(3)

        # Horizontal
        self.assertEqual('', horizontal.name)

        self.assertInterfaces(horizontal, [
            'accessible', 'component', 'value'
        ])

        self.assertStates(horizontal, [
            'enabled', 'horizontal', 'sensitive'
        ])

        # Vertical
        self.assertEqual('', vertical.name)

        self.assertInterfaces(vertical, [
            'accessible', 'component', 'value'
        ])

        self.assertStates(vertical, [
            'enabled', 'vertical', 'sensitive'
        ])

        # We add 4 rows to show scrollbars
        listener = EventListener()
        for i in xrange(0, 4):
            self.add_button.click()

        with listener.listenTo([vertical, horizontal]):
            self.assertStates(vertical, [
                 'enabled', 'vertical', 'sensitive',
                 'showing', 'visible'
            ])

            self.assertStates(horizontal, [
                 'enabled', 'horizontal', 'sensitive',
                 'showing', 'visible'
            ])

