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
        self.controls = ("Button","CheckBox","CheckedListBox","ComboBox",
                    "DomainUpDown","ErrorProvider","GroupBox","HelpProvider",
                    "HScrollBar","Label","LinkLabel","ListBox","ListView",
                    "MainMenu", "MaskedTextBox","MenuItem","NumericUpDown",
                    "Panel", "PictureBox","ProgressBar","RadioButton",
                    "RichTextBox", "ScrollBar","StatusBar","StatusBarPanel",
                    "StatusStrip", "TextBox","ToolStrip","ToolStripComboBox",
                    "ToolStripDropDownButton","ToolStripLabel",
                    "ToolStripMenuItem","ToolStripProgressBar",
                    "ToolStripSplitButton","ToolStripTextBox","ToolTip",
                    "VScrollBar","WebBrowser")

        tmp_test_dirs = os.listdir(Settings.log_dir)
        # take out directories that aren't really for tests, like .svn
        self.test_dirs = [s for s in tmp_test_dirs if "_" in s]
        self.controls_tested = [s[:s.find("_")].lower() for s in self.test_dirs]
        # dashboard_dict's keys are control names and each value is a list
        # of the most recent log files for the tests associates with each
        # control
        self.dashboard_dict = {}
        self.reports_dict = {}
        for control in self.controls:
            if control.lower() in self.controls_tested:
                test_names = [dir for dir in self.test_dirs if dir.startswith("%s_" % control.lower())]
                test_paths = [os.path.join(Settings.log_dir, dir) for dir in test_names]
                new_dirs = []
                for dir in test_paths:
                    new_dirs.append(self.get_newest_subdir(dir))
                # done buildling list of the most recent log directorie(s), now
                # add the list to the dashboard_dict for each control.
                self.dashboard_dict[control] = new_dirs
    
    def get_newest_subdir(self, dir):
        '''return a string of the newest subdir of the string dir'''
        newest_subdir = c.getoutput("ls -tc %s | head -n1" % dir)
        return os.path.join(dir, newest_subdir)

    def get_new_to_old_subdirs(self, dir):
        '''return a string of all subdirs of dir sorted from newest to
        oldest'''
        newest_subdir = c.getoutput("ls -tc %s" % dir)
        return os.path.join(dir, newest_subdir)

    def get_status(self, control):
        '''get the status of the most recent tests for control and return 0
        (success), 1 (fail), or -1 (not run)'''
        status_codes = []

        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard_dict, then the test has
        # not been run, so return -1
        try:
            new_dirs = self.dashboard_dict[control]
        except KeyError:
            return -1
        for dir in new_dirs:
            f = open("%s/status" % dir)
            status_codes.append(int(f.read()))
        if sum(status_codes) == 0:
            return 0
        else:
            return 1
        
    def get_time(self, control):
        # get the list of the most recent log directorie(s) for each control
        # if the control is not found in dashboard_dict, then the test has
        # not been run, so return -1
        new_dirs = []
        try:
            new_dirs = self.dashboard_dict[control]
        except KeyError:
            return -1
        procedures_logs = [os.path.join(log,"procedures.xml") for log in new_dirs]
        times = [self.xmlp.get_time(log) for log in procedures_logs]
        return round(sum(times),1)

    def build_report(self, control):
        raise NotImplementedError

    def build_all(self):
        total_time = 0.0
        num_passed = 0.0
        num_tests = 0.0
        root = ET.Element("dashboard")
        for control_name in self.controls:
            num_tests += 1
            control = ET.SubElement(root, "control")
            ET.SubElement(control, "name").text = control_name
            control_status = self.get_status(control_name)
            if control_status == 0:
                num_passed += 1
            else:
                # build the report for control_name
                # self.build_report(control_name)
                pass
            ET.SubElement(control, "status").text = str(control_status)
            time = self.get_time(control_name)
            # keep track of the total time for successful tests
            if time > 0 and control_status == 0:
                total_time += time
            ET.SubElement(control, "time").text = str(time)
        ET.SubElement(root, "numTests").text = str(num_tests)
        ET.SubElement(root, "numPassed").text = str(num_passed)
        ET.SubElement(root, "percentPassed").text = "%s%s" % \
                                (str(round((num_passed / num_tests)*100,1)),
                                 "%")
        ET.SubElement(root, "totalTime").text = str(total_time)
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
