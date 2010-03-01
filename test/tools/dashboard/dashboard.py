#!/usr/bin/env python

import os
import re
import getopt
import sys
import commands as c
import time as t

try:
    import xml.etree.ElementTree as ET # python 2.5
except ImportError:
    try:
        import cElementTree as ET # cElementTree is faster
    except ImportError:
        import elementtree.ElementTree as ET # fallback on regular ElementTree

def output(s, newline=True):
    if not Settings.is_quiet:
        if newline:
            print s
        else:
            print s

def abort(s):
    sys.exit(s)

class Settings(object):

    is_quiet = False
    log_dir = None
    output_path = None
    component = None

    def __init__(self):
        pass

    def argument_parser(self):
        opts = []
        args = []

        try:
            opts, args = getopt.getopt(sys.argv[1:],"qho:c:",["help","quiet","output=","component="])
        except getopt.GetoptError:
            self.help()
            abort(1)

        for o,a in opts:
            if o in ("-q","--quiet"):
                Settings.is_quiet = True
        for o,a in opts:
            if o in ("-h","--help"):
                self.help()
                sys.exit(0)
            if o in ("-o","--output"):
                Settings.output_path = a
            if o in ("-c","--component"):
                Settings.component = a

        try:
            Settings.log_dir = args[0]
        except IndexError, e:
            output("ERROR: log directory argument is required")
            abort(1)

    def help(self):
        output("Usage: dashboard [options] <log directory>")
        output("  -h       | --help         Print help information (this message)")
        output("  -q       | --quiet        Don't print anything")
        output("  -o <dir> | --output=<dir> Store output files in <dir>")
        output("  -c       | --component=   Select at least and only one component to test (i.e.,")
        output("                            winforms or moonlight).")

class XMLParser(object):

    def __init__(self, log_dir):
        Settings.log_dir = log_dir

    def get_time(self, log):
        tree = ET.ElementTree()
        tree.parse(log)
        time = tree.find("time")
        return float(time.text)

class PageBuilder(object):

    def __init__(self, log_dir, component=None, output_dir=None):
        if output_dir is not None:
            Settings.output_path = output_dir
        Settings.component = component
        Settings.log_dir = os.path.join(log_dir, component)
        self.xmlp = XMLParser(Settings.log_dir)
        # a list of every unique machines that has been tested
        self.all_tested_machines = []
        self.controls = []
        self.winforms_controls = ("Button",
                                  "Checkbox",
                                  "CheckedListBox",
                                  "ColorDialog",
                                  "ColumnHeader",
                                  "Combobox_DropDownList",
                                  "Combobox_DropDown",
                                  "Combobox_Simple",
                                  "Combobox_StyleChanges",
                                  "ContainerControl",
                                  "ContextMenu",
                                  "ContextMenuStrip",
                                  "DataGridBoolColumn",
                                  "DataGridComboboxColumn",
                                  "DataGrid",
                                  "DataGridTextBoxColumn",
                                  "DataGridView_Detail",
                                  "DateTimePicker_DropDown",
                                  "DateTimePicker_ShowUpDown",
                                  "DomainUpDown",
                                  "ErrorProvider",
                                  "FlowLayoutPanel",
                                  "FontDialog",
                                  "Form",
                                  "GroupBox",
                                  "HelpProvider",
                                  "HScrollBar",
                                  "Label",
                                  "LinkLabel",
                                  "ListBox",
                                  "ListView_LargeImage",
                                  "ListView_List",
                                  "ListView",
                                  "ListView_SmallImage",
                                  "MainMenu",
                                  "MaskedTextBox",
                                  "MenuStrip",
                                  "MonthCalendar",
                                  "NotifyIcon",
                                  "NumericUpdown",
                                  "OpenFileDialog",
                                  "PageSetupDialog",
                                  "Panel",
                                  "Picturebox",
                                  "PrintPreviewControl",
                                  "PrintPreviewDialog",
                                  "ProgressBar",
                                  "PropertyGrid",
                                  "RadioButton",
                                  "RichTextBox",
                                  "SaveFileDialog",
                                  "ScrollBar",
                                  "Splitter_Horizontal",
                                  "Splitter_Vertical",
                                  "StatusBarPanel",
                                  "StatusBar",
                                  "StatusStrip",
                                  "TabControl",
                                  "TableLayoutPanel",
                                  "TabPage",
                                  "TextBox",
                                  "ThreadExceptionDialog",
                                  "ToolBarButton",
                                  "ToolBar",
                                  "ToolStripButton",
                                  "ToolStripComboBox",
                                  "ToolStripDropDownButton",
                                  "ToolStripLabel",
                                  "ToolStripMenuItem",
                                  "ToolStripProgressBar",
                                  "ToolStrip",
                                  "ToolStripSeparator",
                                  "ToolStripSplitButton",
                                  "ToolStripTextBox",
                                  "ToolTip",
                                  "TrackBar",
                                  "TreeView",
                                  "VScrollBar")
        self.moonlight_controls = ("Button",
                                   "CheckBox",
                                   "ComboBox",
                                   "GridSplitter",
                                   "HyperlinkButton",
                                   "Image",
                                   "ListBox",
                                   "PasswordBox",
                                   "ProgressBar",
                                   "RadioButton",
                                   "RepeatButton",
                                   "ScrollBar",
                                   "ScrollViewer",
                                   "Slider",
                                   "TextBlock",
                                   "TextBox",
                                   "Thumb",
                                   "ToggleButton")
        if Settings.component == 'winforms':
            self.controls = self.winforms_controls
        elif Settings.component == 'moonlight':
            self.controls = self.moonlight_controls

        tmp_test_dirs = os.listdir(Settings.log_dir)
        # take out directories that aren't really for tests, like .svn
        self.test_dirs = [s for s in tmp_test_dirs if "-" in s]
        self.controls_tested = [s[:s.find("-")].lower() for s in self.test_dirs]

        self.dashboard_smoke = {}
        self.dashboard_regression = {}
        self.reports = {}
        self.update_statuses = {}
        self.newest_dirs = {}
        self.set_update_statuses()

        for control in self.controls:
            if control.lower() in self.controls_tested:
                # dashboard's keys are control names and each value is a list
                # of the most recent log files for the tests associates with
                # each control
                self.dashboard_smoke[control] = {}
                self.dashboard_regression[control] = {}
                self.reports[control] = {}

                test_names = [dir for dir in self.test_dirs if dir.startswith("%s-" % control.lower())]
                test_paths = [os.path.join(Settings.log_dir, dir) for dir in test_names]
                new_to_old_dirs = []
                for dir in test_paths:
                    self.machines = os.listdir(dir)
                    assert len(self.machines) > 0

                    #newest_subdir = self.get_newest_subdir(dir)
                    newest_smoke_subdirs = \
                                       self.get_newest_subdirs(dir, smoke=True)
                    newest_regression_subdirs = \
                                       self.get_newest_subdirs(dir, smoke=False)

                    for machine in self.machines:
                        # first, add the machine to the list of all tested
                        # machines if it is not already there
                        if machine not in self.all_tested_machines:
                            self.all_tested_machines.append(machine)
                        # get the newest subdir for each test directory and
                        # add them to the dashboard dictionaries
                        if newest_smoke_subdirs is not None and \
                                                  len(newest_smoke_subdirs) > 0:
                            try:
                                self.dashboard_smoke[control][machine] += \
                                    newest_smoke_subdirs[machine]
                            except KeyError:
                                self.dashboard_smoke[control][machine] = \
                                    newest_smoke_subdirs[machine]
                        if newest_regression_subdirs is not None and \
                                            len(newest_regression_subdirs) > 0:
                            try:
                                self.dashboard_regression[control][machine] += \
                                             newest_regression_subdirs[machine]
                            except KeyError:
                                self.dashboard_regression[control][machine] = \
                                             newest_regression_subdirs[machine]
                        new_to_old_dirs.append(self.get_new_to_old_subdirs(dir))
                    # done buildling list of the most recent log directorie(s),
                    # now add the list to the dashboard for each control.
                    self.reports[control][machine] = new_to_old_dirs

    def update_newest_dirs(self, machine, time_path):
        try:
            self.newest_dirs[machine].append(time_path)
        except KeyError:
            self.newest_dirs[machine] = [time_path]

    def set_update_statuses(self):
        reg = re.compile(".+_package_status")
        log_dir_ls = os.listdir(Settings.log_dir)
        update_status_files = \
             [file for file in log_dir_ls if reg.match(file)]
        for file in update_status_files:
            f = open(os.path.join(Settings.log_dir,file))
            status = f.read(1)
            machine = file.replace("_package_status","")
            self.update_statuses[machine] = status
            f.close()

    def get_newest_subdirs(self, dir, smoke=False):
        '''update dashboard with the newest subdir of the string dir for
        each unique machine'''
        newest_subdirs = {}
        for machine in self.machines:
            if smoke:
                times = c.getoutput("find %s* -name time | grep smoke" % \
                                              os.path.join(dir,machine)).split()
            else:
                times = \
                    c.getoutput("find %s* -name time | grep -v smoke" % \
                                            os.path.join(dir,machine)).split()
            if not len(times) > 0:
                return {}
            newest_time = 0
            for time_path in times:
                f = open(time_path)
                time = float(f.read())
                if time > newest_time:
                    newest_time = time
                    newest_time_path = time_path
            # add the parent directory of the time file to the
            # dashboard
            test_path = os.path.dirname(newest_time_path)
            self.update_newest_dirs(machine, test_path)
            newest_subdirs[machine] = test_path
        return newest_subdirs

    def get_new_to_old_subdirs(self, dir):
        '''return a list of of all subdirs of dir sorted from newest to
        oldest'''
        # get the list of all subdirs dir sorted from newest to oldest
        new_to_old_subdirs = c.getoutput("ls -tc %s" % dir).split()
        # get the entire path of the directories and store it in the same
        # variable
        for i in range(len(new_to_old_subdirs)):
            new_to_old_subdirs[i] = os.path.join(dir, new_to_old_subdirs[i])
        return new_to_old_subdirs

    def get_status(self, machine, control, is_smoke=False):
        '''get the status of the most recent tests for control and return 0
        (success), 1 (fail), or -1 (not run)'''
        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard, then the test has
        # not been run, so return -1
        new_dir = None
        if is_smoke:
            try:
                new_dir = self.dashboard_smoke[control][machine]
            except KeyError:
                return -1
        else:
            try:
                new_dir = self.dashboard_regression[control][machine]
            except KeyError:
                return -1
        assert new_dir is not None
        assert os.path.exists("%s/status" % new_dir)
        f = open("%s/status" % new_dir)
        status_code = int(f.read(1))
        f.close()
        return status_code

    def get_time(self, machine, control, is_smoke=False):
        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard, then the test has
        # not been run, so return -1
        new_dir = None
        if is_smoke:
            try:
                new_dir = self.dashboard_smoke[control][machine]
            except KeyError:
                return -1
        else:
            try:
                new_dir = self.dashboard_regression[control][machine]
            except KeyError:
                return -1
        assert new_dir is not None
        procedures_log = os.path.join(new_dir,"procedures.xml")
        assert os.path.exists(procedures_log)
        time = self.xmlp.get_time(procedures_log)
        return round(time,1)

    def build_report(self, control):
        raise NotImplementedError

    def make_directory(self, path):
        '''
        Attempt to make a directory and ignore any errors if the directory
        already exists
        '''
        try:
            os.mkdir(path)
        except OSError, err:
            # Errno 17 is "File exists", which is fine
            if not err.errno == 17:
                raise

    def build_regression(self):
        self.regression_time = {}
        self.regression_status = {}
        for control in self.controls:
            regression_status = 0
            regression = ET.Element('regression')
            ET.SubElement(regression, 'controlName').text = control
            ET.SubElement(regression, 'timeAndDate').text = \
                ' '.join(t.asctime().split()[:-1]+[t.tzname[t.daylight]])

            for machine_name in self.all_tested_machines:
                elapsed_time = 0.0
                status = self.get_status(machine_name, control)
                if status != -1:
                    regression_status |= status
                    # self.regression_status is used by the main page to
                    # display the status of the test.  We the main page to
                    # ignore any not run tests iff at least one machine has run
                    # the test
                    self.regression_status[control] = regression_status
                else:
                    regression_status = -1
                time = self.get_time(machine_name, control)
                if time > 0 and status == 0:
                    elapsed_time += time

                machine = ET.SubElement(regression, 'machine')
                ET.SubElement(machine, 'name').text = machine_name
                ET.SubElement(machine, 'status').text = str(status)
                ET.SubElement(machine, 'time').text = str(time)

            ET.SubElement(regression, 'elapsedTime').text = str(elapsed_time)
            self.regression_time[control] = elapsed_time

            # if self.regression_status was never set, then we need to set it
            # as not run
            if not self.regression_status.has_key(control):
                    self.regression_status[control] = -1

            # if an output path was not provided, we'll just stick
            # everything in the current working directory.  otherwise we want
            # to store the output in the specific output path.
            f = None
            if Settings.output_path is None:
                try:
                    self.make_directory('regression')
                except OSError:
                    output("ERROR: Could not store output for %s" % control)
                f = open('%s/regression/%s.xml' % (Settings.component, control), 'w')
            else:
                self.make_directory('%s/%s/regression' % (Settings.output_path, Settings.component))
                assert os.path.exists('%s/%s/regression' % (Settings.output_path, Settings.component))
                try:
                    f = open('%s/%s/regression/%s.xml' % (Settings.output_path, Settings.component, control), 'w')
                except IOError:
                    output('WARN: Failed to save output to %s/%s/regression/%s.xml, saving to current working directory instead.' % (Settings.output_path, Settings.component, control))
                    try:
                        self.make_directory('regression')
                    except OSError:
                        output("ERROR: Could not store output for %s" % control)
            if f is not None:
                f.write('<?xml version="1.0" encoding="UTF-8"?>')
                f.write('<?xml-stylesheet type="text/xsl" href="../regression.xsl"?>')
                ET.ElementTree(regression).write(f)
                f.close()

    def build_smoke(self):
        self.smoke_time = {}
        self.smoke_status = {}
        for control in self.controls:
            smoke_status = 0
            smoke = ET.Element('smoke')
            ET.SubElement(smoke, 'controlName').text = control
            ET.SubElement(smoke, 'timeAndDate').text = \
                ' '.join(t.asctime().split()[:-1]+[t.tzname[t.daylight]])

            for machine_name in self.all_tested_machines:
                elapsed_time = 0.0
                status = self.get_status(machine_name, control, is_smoke=True)
                if status != -1:
                    smoke_status |= status
                    # self.smoke_status is used by the main page to display the
                    # status of the test.  We the main page to ignore any not
                    # run tests iff at least one machine has run the test
                    self.smoke_status[control] = smoke_status
                else:
                    smoke_status = -1
                time = self.get_time(machine_name, control, is_smoke=True)
                if time > 0 and status == 0:
                    elapsed_time += time

                machine = ET.SubElement(smoke, 'machine')
                ET.SubElement(machine, 'name').text = machine_name
                ET.SubElement(machine, 'status').text = str(status)
                ET.SubElement(machine, 'time').text = str(time)

            ET.SubElement(smoke, 'elapsedTime').text = str(elapsed_time)
            self.smoke_time[control] = elapsed_time

            # if self.smoke_status was never set, then we need to set it as
            # not run
            if not self.smoke_status.has_key(control):
                    self.smoke_status[control] = -1

            # if an output path was not provided, we'll just stick
            # everything in the current working directory.  otherwise we want
            # to store the output in the specific output path.
            f = None
            if Settings.output_path is None:
                try:
                    self.make_directory('smoke')
                except OSError:
                    output("ERROR: Could not store output for %s" % control)
                f = open('smoke/%s.xml' % control, 'w')
            else:
                self.make_directory('%s/smoke' % Settings.output_path)
                assert os.path.exists('%s/smoke' % Settings.output_path)
                try:
                    f = open('%s/smoke/%s.xml' % (Settings.output_path, control), 'w')
                except IOError:
                    output('WARN: Failed to save output to %s/smoke/%s.xml, saving to current working directory instead.' % (Settings.output_path, control))
                    try:
                        self.make_directory('smoke')
                    except OSError:
                        output("ERROR: Could not store output for %s" % \
                                                                       control)
            if f is not None:
                f.write('<?xml version="1.0" encoding="UTF-8"?>')
                f.write('<?xml-stylesheet type="text/xsl" href="../smoke.xsl"?>')
                ET.ElementTree(smoke).write(f)
                f.close()

    def build_main(self):
        smoke_time = 0.0
        smoke_num_passed = 0.0
        smoke_num_tests = 0.0
        regression_time = 0.0
        regression_num_passed = 0.0
        regression_num_tests = 0.0
        root = ET.Element("dashboard")

        # add the current time and date to the XML dashboard file
        ET.SubElement(root, "timeAndDate").text = \
             " ".join(t.asctime().split()[:-1]+[t.tzname[t.daylight]])

        # smoke test portion of XML dashboard file
        smoke = ET.SubElement(root, "smoke")
        for control_name in self.controls:
            smoke_num_tests += 1
            control = ET.SubElement(smoke, "control")
            ET.SubElement(control, "name").text = control_name
            control_status = self.smoke_status[control_name]
            if control_status == 0:
                smoke_num_passed += 1
            ET.SubElement(control, "status").text = str(control_status)
            time = self.smoke_time[control_name]
            # keep track of the total time for successful tests
            if time > 0 and control_status == 0:
                smoke_time += time
            ET.SubElement(control, "time").text = str(time)
        ET.SubElement(smoke, "numTests").text = str(smoke_num_tests)
        ET.SubElement(smoke, "numPassed").text = str(smoke_num_passed)
        ET.SubElement(smoke, "percentPassed").text = "%s%s" % \
                   (str(round((smoke_num_passed / smoke_num_tests)*100,1)),
                   "%")
        ET.SubElement(smoke, "elapsedTime").text = str(smoke_time)

        # regression test portion of XML dashboard file
        regression = ET.SubElement(root, "regression")
        for control_name in self.controls:
            regression_num_tests += 1
            control = ET.SubElement(regression, "control")
            ET.SubElement(control, "name").text = control_name
            control_status = self.regression_status[control_name]
            if control_status == 0:
                regression_num_passed += 1
            ET.SubElement(control, "status").text = str(control_status)
            time = self.regression_time[control_name]
            # keep track of the total time for successful tests
            if time > 0 and control_status == 0:
                regression_time += time
            ET.SubElement(control, "time").text = str(time)
        ET.SubElement(regression, "numTests").text = str(regression_num_tests)
        ET.SubElement(regression, "numPassed").text = \
                                str(regression_num_passed)
        ET.SubElement(regression, "percentPassed").text = "%s%s" % \
             (str(round((regression_num_passed / regression_num_tests)*100,1)),
              "%")
        ET.SubElement(regression, "elapsedTime").text = str(regression_time)

        # get the status of each machines package update
        updates = ET.SubElement(root, "updateStatus")
        for machine_name in self.update_statuses:
            machine = ET.SubElement(updates, "machine")
            ET.SubElement(machine, "name").text = str(machine_name)
            ET.SubElement(machine, "status").text = \
                        str(self.update_statuses[machine_name])

        # write the dashboard file to disk; try to store in
        # Settings.output_path if it exists, if it doesn't exist or if it
        # fails, save in the cwd
        if Settings.output_path is None:
            f = open('%s.xml' % Settings.component, 'w')
        else:
            try:
                f = open('%s/%s.xml' % (Settings.output_path, Settings.component), 'w')
            except IOError:
                output("WARN:  Failed to save output to %s/dashboard.xml, saving to current working directory instead." % Settings.output_path)
                f = open('%s.xml' % Settings.component, 'w')

        f.write('<?xml version="1.0" encoding="UTF-8"?>')
        f.write('<?xml-stylesheet type="text/xsl" href="%s.xsl"?>' % Settings.component)
        ET.ElementTree(root).write(f)
        f.close()

class Dashboard(object):

    def __init__(self, log_dir):
        Settings.log_dir = log_dir
        self.pb = PageBuilder(Settings.log_dir, Settings.component)

    def update(self, log):
        pass

    def update_all(self):
        self.pb.build_regression()
        self.pb.build_smoke()
        self.pb.build_main()

if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    d = Dashboard(Settings.log_dir)
    d.update_all()
