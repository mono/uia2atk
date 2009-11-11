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
#      Andres G. Aragoneses <aaragoneses@novell.com>
#

from moonlight import *
from strongwind import *
from strongwind.events import EventListener

from os.path import abspath
import pyatspi

class TableCalendar(TestCase):
    """
    Exercises the basic functionality of the Table class using a Calendar
    control
    """
    @classmethod
    def setup_class(cls):
        cls.app = launchAddress(abspath('assets/TableTest/TableTest.html'))

    @classmethod
    def teardown_class(cls):
        cls.app.kill()

    def test_table(self):
        """
        Should test all of:
            - addRowSelection
            - caption
            - getAccessibleAt
            - getColumnAtIndex
            - getColumnDescription
            - getColumnExtentAt
            - getColumnHeader
            - getIndexAt
            - getRowAtIndex
            - getRowColumnExtentsAtIndex
            - getRowDescription
            - getRowExtentAt
            - getRowHeader
            - isColumnSelected
            - isRowSelected
            - isSelected
            - nColumns
            - nRows
            - nSelectedRows
            - ref <- refAt??
            - removeRowSelection
            - summary

        But not (as there's no UIA counterpart):
            - addColumnSelection
            - removeColumnSelection
            - getSelectedColumns
            - nSelectedColumns

        And not (because of BNC#512477):
            - getSelectedRows
        """
        sleep(config.LONG_DELAY)
        self.assertEqual(self.app.slControl.childCount, 1)
        calendar = self.app.slControl.findAllCalendars('')[0]._accessible
        table = calendar.queryTable()
        self.assertEqual(table.nRows, 6) # may vary depending on the month? anyway the asset uses a fixed date
        self.assertEqual(table.nColumns, 7) # days of the week
        self.assertEqual(table.nSelectedRows, 0)

        days = {'Su':0, 'Mo':1, 'Tu':2, 'We':3, 'Th':4, 'Fr':5, 'Sa': 6}
        for key, value in days.iteritems():
            self.assertEqual(table.getColumnHeader(value).name, key)

        table.getColumnDescription(0) # returns "" but rather not assert and just test that it doesn't raise any exception

        second_row_checked = False
        inside_table_already = False
        for i in xrange(0, calendar.childCount):
            name = calendar.getChildAtIndex(i).name
            number = 0
            try:
                number = int(name)
            except ValueError:
                number = -1

            if inside_table_already == False or calendar.getChildAtIndex(i).getRoleName() != "label":
                if number < 1:
                    if len(name) == 2:
                        self.assertEqual(table.getRowAtIndex(i), -1) # it's a day of the week, 0th row (column header)
                        self.assertEqual(table.getColumnAtIndex(i), days[name])
                    else:
                        if (inside_table_already == False):
                            self.assertEqual(table.getRowAtIndex(i), -1)    #invalid cell, it's not part of the table
                            self.assertEqual(table.getColumnAtIndex(i), -1) #invalid cell, it's not part of the table
                else:
                    inside_table_already = True
                    if second_row_checked == False:
                        if number < 8:
                            self.assertEqual(table.getRowAtIndex(i), 1) # second row
                            self.assertEqual(table.getColumnAtIndex(i), number - 1)
                            if number == 7:
                                second_row_checked = True
                        else:
                            if number > 21:
                                self.assertEqual(table.getRowAtIndex(i), 0) # first row
                    else:
                        if (second_row_checked == True and number == 8):
                            self.assertEqual(table.getRowAtIndex(i), 2) # third row

