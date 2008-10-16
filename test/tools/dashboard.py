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

class Settings(object):

    is_quiet = False

    def __init__(self):
        pass
 
    def argument_parser(self):
        opts = []
        args = []

        try:
          opts, args = getopt.getopt(sys.argv[1:],"qh",["help","quiet"])
        except getopt.GetoptError:
          self.help()
          sys.exit(1)

        for o,a in opts:
          if o in ("-q","--quiet"):
            Settings.is_quiet = True
        for o,a in opts:
          if o in ("-h","--help"):
            self.help()
            sys.exit(0)

        Settings.log_dir = args[0]

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
    
        test_dirs = os.listdir(Settings.log_dir)
        # take out directories that aren't really for tests, like .svn
        self.test_dirs = [s for s in test_dirs if "_" in s]
        self.controls_tested = [s[:s.find("_")].lower() for s in test_dirs]

    def get_status(self, control):
        '''get the status and return 0 (success), 1 (fail), or -1 (not run)'''
        if control.lower() in self.controls_tested:
            status_files = c.getoutput("find %s/%s* -name status" % (Settings.log_dir, control.lower())).split()
            status_codes = []
            for status_file in status_files:
                f = open(status_file)
                status_codes.append(int(f.read()))
            if sum(status_codes) == 0:
                return 0
            else:
                return 1  
        else:
            return -1

    def get_time(self, control):
        # if we have test results for the control we're getting the time for
        logs = None
        if control.lower() in self.controls_tested:
            # get the combined times of tests
            logs = c.getoutput("find %s/%s* -name procedures.xml" % (Settings.log_dir, control.lower())).split()
            times = [self.xmlp.get_time(log) for log in logs]
            return round(sum(times),1)
        else:
            return -1

    def build_all(self):
        root = ET.Element("dashboard")
        for control_name in self.controls:
            control = ET.SubElement(root, "control")
            ET.SubElement(control, "name").text = control_name
            control_status = str(self.get_status(control_name))
            ET.SubElement(control, "status").text = control_status
            control_time = str(self.get_time(control_name))
            ET.SubElement(control, "time").text = control_time
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
