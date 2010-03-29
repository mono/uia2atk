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

NAMES = [ 'Boolean', 'Editable', 'ReadOnly' ]

class DataGridColumn(TestCase):
    """
    Exercises DataGridColumn control. DataGridColumn control implements:
    - UIA.Invoke pattern, mapped to Action.
    - UIA.ScrollItem pattern, NOT mapped.
    - UIA.Transform pattern, NOT mapped.
    """

    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/DataGridTest/DataGridTest.html'))

        # Hierarchy is like this:
        # DataGrid
        # - Table Row Header - TableRowHeader
        # -- DataGridColumn0 - TableCell
        # -- DataGridColumn1 - TableCell
        # -- DataGridColumnN - TableCell
        # - Rows - Unknown
        # - HScrollbar - ScrollBar
        # - VScrollbar - ScrollBar

        cls.datagrid = cls.app.slControl.findAllTables('')[0]
        cls.columns_container = cls.app.slControl.findAllTableRowHeaders('')[0]
        cls.rows_container = cls.app.slControl.findAllUnknowns('')[0]

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    # Basic Tests

    def test_datagridcolumn_basic(self):
        # Test DataGridIColumns container
        self.assertEqual('', self.columns_container.name)

        self.assertInterfaces(self.columns_container, [
            'accessible', 'component'
        ])

        self.assertStates(self.columns_container, [
            'enabled', 'sensitive', 'showing', 'visible'
        ])

        # 3 Columns
        self.assertEqual(3, self.columns_container.childCount)

        # Test all DataGridColumns
        for i in xrange(0, 3):
            datagridcell = self.columns_container.getChildAtIndex(i)
            self.assertEqual(pyatspi.ROLE_TABLE_CELL, datagridcell.role)
            self.assertEqual(NAMES[i], datagridcell.name)
            self.assertEqual(1, datagridcell.childCount)

            self.assertInterfaces(datagridcell, [
                'accessible', 'component', 'action'
            ])

            self.assertStates(datagridcell, [
                'enabled', 'sensitive', 'showing', 'visible'
            ])

    # Tests selection children, aka, DataGridCells
    def test_datagridcell_basic(self):
        for i in xrange(0, 3):
            datagridcell = self.columns_container.getChildAtIndex(i)

            # The item has 1 child:
            # - Label
            for x in xrange(0, datagridcell.childCount):
                item = datagridcell.getChildAtIndex(x)
                self.assertEqual(pyatspi.ROLE_LABEL, item.role)
                self.assertEqual(NAMES[i], item.name)
                self.assertEqual(0, item.childCount)
                self.assertInterfaces(item, [ 'accessible', 'component' ])
                self.assertStates(item, [ 'enabled', 'sensitive', 'showing', 'visible' ])

    # Action Tests
    # No event testing: calling the action will sort the rows based
    # on the column, but won't raise any UIA event associated to this sorting
    def test_action(self):
        datagridcolumn = self.columns_container.getChildAtIndex(1)
        action = datagridcolumn._accessible.queryAction()

        self.assertEqual(1, action.nActions)
        self.assertEqual('click', action.getName(0))
        self.assertEqual('', action.getDescription(0))
        self.assertEqual('', action.getKeyBinding(0))

        datagridcell0 = self.rows_container.getChildAtIndex(0)
        datagridcell3 = self.rows_container.getChildAtIndex(3)

        self.assertTrue(datagridcell0.selected)
        self.assertFalse(datagridcell3.selected)

        listener = EventListener()
        with listener.listenTo([ datagridcell0, datagridcell3 ]):
            datagridcolumn.click()
            datagridcolumn.click()
            sleep(config.SHORT_DELAY)

        self.assertFalse(datagridcell0.selected)
        sleep(config.SHORT_DELAY)

        self.assertTrue(datagridcell3.selected)


