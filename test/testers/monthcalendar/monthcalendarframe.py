# vim: set tabstop=4 shiftwidth=4 expandtab
##############################################################################
# Written by:  Sandy Armstrong <saarmstrong@novell.com>
#              Calen Chen <cachen@novell.com>
# Date:        02/23/2009
# Description: monthcalendar.py wrapper script
#              Used by the monthcalendar-*.py tests
##############################################################################$

import sys
import os
import actions
import states
import time
import datetime

from strongwind import *
from monthcalendar import *
from helpers import *

# get localtime and separate it to year/month/day as constants
localtime = time.localtime()
current_year = localtime[0]
current_month = localtime[1]
current_day = localtime[2]
mkdate = datetime.date(current_year, current_month, current_day)

# class to represent the main window.
class MonthCalendarFrame(accessibles.Frame):

    LABEL = "Your selection is:\n2008-01-23"
    DEFAULT_MONTH = "January"
    FILLER_NAME = "1/23/2008"
    COLUMN_HEADERS_NUM = 13
    TABLE_CELLS_NUM = 42

    def __init__(self, accessible):
        super(MonthCalendarFrame, self).__init__(accessible)

        self.label = self.findLabel(self.LABEL)
        self.back_button = self.findPushButton('Back by one month')
        self.forward_button = \
                           self.findPushButton('Forward by one month')

        # find the accessible that is showing the current daytime
        # BUG516719: push button to show current day is not accessible
        global mkdate
        current_daytime =  mkdate.strftime("%s/%s/%s" % \
                                     (current_month, current_day, current_year))
        #self.today_table_cell = \
        #      self.findPushButton("Today: %s" % current_daytime)

        self.findAccessiblesFromTreeTable()

    def findAccessiblesFromTreeTable(self):
        """
        Find all accessibles from tree_table
        """ 
        # find accessible which between back and forward button. title_month 
        # is a label, title_year is a push button and click it will be changed 
        # to a spin button
        # BUG516717: title month and title year are not accessible
        #self.title_month = self.monthcalendar_filler.findLabel(self.DEFAULT_MONTH)
        #self.title_year = self.monthcalendar_filler.findPushButton(None)

        self.monthcalendar_filler = self.findFiller(None)
        self.tree_table = self.monthcalendar_filler.findTreeTable(None)
        # find all week day column_headers with giving Name
        self.week_day_column_headers = [
                              self.tree_table.findTableColumnHeader('Sun'),
                              self.tree_table.findTableColumnHeader('Mon'),
                              self.tree_table.findTableColumnHeader('Tue'),
                              self.tree_table.findTableColumnHeader('Wed'),
                              self.tree_table.findTableColumnHeader('Thu'),
                              self.tree_table.findTableColumnHeader('Fri'),
                              self.tree_table.findTableColumnHeader('Sat')
                              ]
        # find all column_headers, there are 13 
        self.column_headers = self.tree_table.findAllTableColumnHeaders(None)
        # BUG516718: week numbers are not accessible
        #assert len(self.column_headers) == self.COLUMN_HEADERS_NUM, \
        #                 "actual number of column_headers:%s, expected: %s" % \
        #                                           (len(self.column_headers, 7)
        # the test didn't use self.column_headers[:] to set the list, because 
        # the index order of week_num_column_header is not clear in current, 
        # we needn't update the script after bug516718 be fixed, this method 
        # won't be affected by the index order
        self.week_num_column_headers = []
        for column_header in self.column_headers:
            try:
                int(column_header.name)
            except ValueError:
                self.week_day_column_headers
            else:
                self.week_num_column_headers.append(column_header)

        # find all day table_cells, we don't give exactly Name because the table 
        # will be refreshed when month or year is changed, the number of Day
        # table_cells is 42
        self.table_cells = [[self.tree_table.getChildAtIndex(7 * r + c + 7) \
                            for c in range(7)] for r in range(6)]
        sleep(config.SHORT_DELAY)
        assert len(self.table_cells * 7) == self.TABLE_CELLS_NUM, \
                          "actual number of table_cells:%s, expected: %s" % \
                          (len(self.table_cells * 7), self.TABLE_CELLS_NUM)

    def dayTableCell(self, day_table_cell, firstReached=True):
        """
        assume day_table_cell is the number of day which we want to do test for, 
        return the index of the day at TreeTable 
        """

        for r in xrange(6):
            for c in xrange(7):
                if self.table_cells[r][c].name == str(day_table_cell):
                    if firstReached is True:
                        return self.table_cells[r][c]
                    else:
                       firstReached = True
                       continue

    def testTableCellStates(self, day_table_cell, is_selected=True, firstReached=True):
        """
        test day_table_cell's states, if is_selected, the cell should have 
        focused and selected states
        """
        # day_table_cell is the table_cell that you want to check the states, 
        # call dayTableCell method to get the day_index of day_table_cell from 
        # its parent tree_table
        if is_selected:
            statesCheck(self.dayTableCell(day_table_cell, firstReached), 
                             "TableCell", add_states=["focused", "selected"])
        else:
            statesCheck(self.dayTableCell(day_table_cell), "TableCell")


    def assertText(self, accessible, expected_label):
        """Ensure accessible's text is expected"""
        procedurelogger.expectedResult('%s\'s text has been changed to "%s"' % \
                                                   (accessible, expected_label))
        accessible.text == expected_label, \
                                "actual label text:%s, expected:%s" % \
                                    (self.label.text, expected_label)

    def assertName(self, accessible, expected_name):
        """Ensure accessible's name is expected"""
        procedurelogger.expectedResult('the first day_table_cell should be %s' %\
                                                   (expected_name))
        assert accessible.name == expected_name, \
                                    "the actual name is %s, expected %s" % \
                                          (accessible.name, expected_name)

    def findTodayContextMenuAccessibles(self):
        """
        Find all of the accessibles on the today ContextMenuStrip when mouse 
        button3 click MonthCalendar
        """
        # find window, menu and menu_item
        self.today_window = self.app.findWindow(None)
        self.today_menu = self.today_window.findMenu(None)

        self.today_menu_item = self.today_menu.findMenuItem("Go to today")

    def todayContextMenuAccessiblesTest(self):
        """
        Test accessibles states and action, click menu_item will change to 
        current daytime
        """
        # test states 
        statesCheck(self.today_window, "ContextMenuStrip", add_states=["active"])
        statesCheck(self.today_menu, "Menu", add_states=["focusable"])
        statesCheck(self.today_menu_item, "MenuItem")

        # click menu_item to change label
        self.today_menu_item.click(log=True)
        sleep(config.SHORT_DELAY)
        global mkdate
        current_daytime =  mkdate.strftime("%y-%m-%d")
        self.assertText(self.label, "Your selection is:\n%s" % current_daytime)
        # the current day table_cell will be focused and selected
        self.refreshAccessibleRefs()
        sleep(config.SHORT_DELAY)
        self.testTableCellStates(day_table_cell=current_day)  

    def findMonthContextMenuAccessibles(self):
        """
        Find all of the accessibles on the month ContextMenuStrip when mouse 
        button1 click title_month on TreeTable list
        """
        procedurelogger.expectedResult("All of the widgets are found successfully")

        self.month_window = self.app.findWindow(None)
        self.month_menu = self.month_window.findMenu(None)

        self.month_menu_items = {}
        month_menu_item = ["January","February","March","April","May","June",\
                   "July","August","September","October","November","December"]

        for i in month_menu_item:
            self.month_menu_items[i] = \
                               self.month_menu.findMenuItem(i)

    def monthContextMenuAccessiblesTest(self):
        """
        Check all ContextMenu accessibles default states
        """
        statesCheck(self.window, "ContextMenuStrip", add_states=["active"])
        statesCheck(self.month_menu, "Menu", add_states=["focusable"])

        for menu_item in self.month_menu_item:
            statesCheck(menu_item, "MenuItem")

    def yearSpinButtonValueTest(self, expected_value, set_value=False):
        """
        Ensure title_year SpinButton's value is expected, if set_value is True, 
        the expected_value will be set for title_year SpinButton
        """
        procedurelogger.action("set value %s for title_year SpinButton" % \
                                                                expected_value)
        if set_value:
            self.title_year.value = expected_value

        procedurelogger.expectedResult("title_year SpinButton's value is %s" % \
                                                                 expected_value)
        assert self.title_year.value == expected_value, \
                       "actual value of SpinButton:%s, expected:%s" % \
                          (self.title_year.value, expected_value)      
 
    # close application main window after running test
    def quit(self):
        self.altF4()
