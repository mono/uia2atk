#!/usr/bin/env python

import os
import getopt
import sys
import commands as c

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
      print s,

def abort(s):
    sys.exit(s)

class Settings(object):

    is_quiet = False
    log_dir = None

    def __init__(self):
        pass
 
    def argument_parser(self):
        opts = []
        args = []

        try:
          opts, args = getopt.getopt(sys.argv[1:],"qh",["help","quiet"])
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

        try:
            Settings.log_dir = args[0]
        except IndexError, e:
            output("ERROR: log directory argument is required")
            abort(1)
   
    def help(self):
        output("Usage: dashboard [options] <log directory>")
        output("  -h | --help        Print help information (this message)")
        output("  -q | --quiet       Don't print anything")

class XMLParser(object):

    def __init__(self, log_dir):
        Settings.log_dir = log_dir

    def get_time(self, log):
        tree = ET.ElementTree()
        tree.parse(log)
        time = tree.find("time")
        return float(time.text)

class PageBuilder(object):

    def __init__(self, log_dir):
        Settings.log_dir = log_dir
        self.xmlp = XMLParser(Settings.log_dir)
        self.controls = ("Button",
                         "CheckBox",
                         "CheckedListBox",
                         "ComboBox",
                         "DomainUpDown",
                         "ErrorProvider",
                         "Form",
                         "GroupBox",
                         "HelpProvider",
                         "HScrollBar",
                         "Label",
                         "LinkLabel",
                         "ListBox",
                         "ListView",
                         "MainMenu",
                         "MaskedTextBox",
                         "MenuItem",
                         "NumericUpDown",
                         "Panel",
                         "PictureBox",
                         "ProgressBar",
                         "RadioButton",
                         "RichTextBox",
                         "ScrollBar",
                         "StatusBar",
                         "StatusBarPanel",
                         "StatusStrip",
                         "TextBox",
                         "ToolStrip",
                         "ToolStripComboBox",
                         "ToolStripDropDownButton",
                         "ToolStripLabel",
                         "ToolStripMenuItem",
                         "ToolStripProgressBar",
                         "ToolStripSplitButton",
                         "ToolStripTextBox",
                         "ToolTip",
                         "VScrollBar",
                         "WebBrowser")

        tmp_test_dirs = os.listdir(Settings.log_dir)
        # take out directories that aren't really for tests, like .svn
        self.test_dirs = [s for s in tmp_test_dirs if "_" in s]
        self.controls_tested = [s[:s.find("_")].lower() for s in self.test_dirs]
        # dashboard's keys are control names and each value is a list
        # of the most recent log files for the tests associates with each
        # control
        self.dashboard = {}
        self.dashboard_smoke = {}
        self.dashboard_regression = {}
        self.newest_dirs = {}
        self.reports = {}
        for control in self.controls:
            if control.lower() in self.controls_tested:
                test_names = [dir for dir in self.test_dirs if dir.startswith("%s_" % control.lower())]
                test_paths = [os.path.join(Settings.log_dir, dir) for dir in test_names]

                new_to_old_dirs = []
                for dir in test_paths:
                    newest_subdirs = self.get_newest_subdirs(dir)
                    newest_smoke_subdirs = \
                     [dir for dir in newest_subdirs if "smoke_test" in dir]
                    newest_regression_subdirs = \
                     [dir for dir in newest_subdirs if "smoke_test" not in dir]
                    try:
                        # get the newest subdirs for each test directory and
                        # add them to the dashboard dictionary
                        self.dashboard[control] += newest_subdirs
                    except KeyError:
                        self.dashboard[control] = newest_subdirs
                    # do the same thing but create separate dashboard
                    # dictionaries for smoke tests and regression tests
                    if len(newest_smoke_subdirs) > 0:
                        try:
                            self.dashboard_smoke[control] += \
                                                           newest_smoke_subdirs
                        except KeyError:
                            self.dashboard_smoke[control] = \
                                                           newest_smoke_subdirs
                    if len(newest_regression_subdirs) > 0:
                        try:
                            self.dashboard_regression[control] += \
                                                      newest_regression_subdirs
                        except KeyError:
                            self.dashboard_regression[control] = \
                                                      newest_regression_subdirs
                    new_to_old_dirs.append(self.get_new_to_old_subdirs(dir))
                # done buildling list of the most recent log directorie(s), now
                # add the list to the dashboard for each control.
                self.reports[control] = new_to_old_dirs
        #for key in self.reports:
        #    for path in self.reports[key]:
        #        print path

    def update_newest_dirs(self, machine, time_path):
            try:
                self.newest_dirs[machine].append(time_path)
            except KeyError:
                self.newest_dirs[machine] = [time_path]
    
    def get_newest_subdirs(self, dir):
        '''update dashboard with the newest subdirs of the string dir for
        each unique machine'''
        machine_dirs = c.getoutput("ls %s" % dir).split()
        # get a list of each machine in the log directory
        machines = list(set([m[:m.find("_")] for m in machine_dirs]))
        # create a list of the time paths for each machine
        newest_subdirs = []
        for machine in machines:
            times = c.getoutput("find %s* -name time" % os.path.join(dir,machine)).split()
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
            newest_subdirs.append(test_path)
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

    def get_status(self, control, is_smoke=False):
        '''get the status of the most recent tests for control and return 0
        (success), 1 (fail), or -1 (not run)'''
        status_codes = []

        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard, then the test has
        # not been run, so return -1
        new_dirs = None
        if is_smoke:
            try:
                new_dirs = self.dashboard_smoke[control]
            except KeyError:
                return -1
        else:
            try:
                new_dirs = self.dashboard_regression[control]
            except KeyError:
                return -1
        for dir in new_dirs:
            f = open("%s/status" % dir)
            status_codes.append(int(f.read()))
        if sum(status_codes) == 0:
            return 0
        else:
            return 1
        
    def get_time(self, control, is_smoke=False):
        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard, then the test has
        # not been run, so return -1
        new_dirs = None
        if is_smoke:
            try:
                new_dirs = self.dashboard_smoke[control]
            except KeyError:
                return -1
        else:
            try:
                new_dirs = self.dashboard_regression[control]
            except KeyError:
                return -1
        procedures_logs = [os.path.join(log,"procedures.xml") for log in new_dirs]
        times = [self.xmlp.get_time(log) for log in procedures_logs]
        return round(sum(times),1)

    def build_report(self, control):
        raise NotImplementedError

    def build_all(self):
        smoke_time = 0.0
        smoke_num_passed = 0.0
        smoke_num_tests = 0.0
        regression_time = 0.0
        regression_num_passed = 0.0
        regression_num_tests = 0.0
        root = ET.Element("dashboard")

        # smoke test portion of XML dashboard file
        smoke = ET.SubElement(root, "smoke")
        for control_name in self.controls:
            smoke_num_tests += 1
            control = ET.SubElement(smoke, "control")
            ET.SubElement(control, "name").text = control_name
            control_status = self.get_status(control_name, is_smoke=True)
            if control_status == 0:
                smoke_num_passed += 1
            else:
                # build the report for control_name
                # self.build_report(control_name)
                pass
            ET.SubElement(control, "status").text = str(control_status)
            time = self.get_time(control_name, is_smoke=True)
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
            control_status = self.get_status(control_name)
            if control_status == 0:
                regression_num_passed += 1
            else:
                # build the report for control_name
                # self.build_report(control_name)
                pass
            ET.SubElement(control, "status").text = str(control_status)
            time = self.get_time(control_name)
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

        # write the dashboard file to disk
        f = open('dashboard.xml', 'w')
        f.write('<?xml version="1.0" encoding="UTF-8"?>')
        f.write('<?xml-stylesheet type="text/xsl" href="dashboard.xsl"?>')
        ET.ElementTree(root).write(f)
        f.close()

class Dashboard(object):

    def __init__(self, log_dir):
        Settings.log_dir = log_dir
        self.pb = PageBuilder(Settings.log_dir)

    def update(self, log):
        pass

    def update_all(self):
        self.pb.build_all()
        
if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    d = Dashboard(Settings.log_dir)
    d.update_all()
