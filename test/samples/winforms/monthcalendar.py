#!/usr/bin/env ipy

##############################################################################
# Written by:  Ray Wang <rawang@novell.com>
# Date:        06/24/2008
# Description: the sample for winforms control:
#              MonthCalendar
##############################################################################

# The docstring below is used in the generated log file
"""
Test accessibility of "MonthCalendar" control
"""

# imports
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')
from System import DateTime
from System.Windows.Forms import Application, Form, MonthCalendar, Label, SelectionRange
from System.Drawing import Point

class MonthCalendarSample(Form):
    """MonthCalendar control class"""

    def __init__(self):
        """MonthCalendarSample class init function."""

        # setup title
        self.Text = "MonthCalendar control"

        # setup monthcalendar
        self.monthcalendar = MonthCalendar()
        # set one days that can be selected in a month calendar control
        self.monthcalendar.MaxSelectionCount = 2
        self.monthcalendar.ShowWeekNumbers = True
        #NOTE: Strongwind tests depend on this date setting! Make sure to update them if you change it
        initialRange = SelectionRange(DateTime.Parse("1/23/2008"), DateTime.Parse("1/23/2008"))
        self.monthcalendar.SelectionRange = initialRange
        self.monthcalendar.DateChanged += self.date_select

        # setup label
        self.label = Label()
        self.label.Width = 200
        self.label.Height = 50
        self.label.Location = Point(5, 170)
        self.label.Text = ("Your selection is:\n" +
                (self.monthcalendar.SelectionRange.Start).ToString('o')[:10])

        # add controls
        self.Controls.Add(self.monthcalendar)
        self.Controls.Add(self.label)

    def date_select(self, sender, event):
        self.label.Text =  ('Your selection is:\n' + 
                (self.monthcalendar.SelectionRange.Start).ToString('o')[:10])


# run application
form = MonthCalendarSample()
Application.EnableVisualStyles()
Application.Run(form)
