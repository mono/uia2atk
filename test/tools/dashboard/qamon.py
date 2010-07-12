#!/usr/bin/env python
import os
import re
import dashboard
import getopt
import sys
from pyinotify import WatchManager, Notifier, ThreadedNotifier, ProcessEvent, \
                      EventsCodes

def output(s, newline=True):
  if not Settings.is_quiet:
    if newline:
      print s
    else:
      print s

def abort(s):
    sys.exit(s)

class Settings(object):

    def __init__(self):
        Settings.wm = WatchManager()
        Settings.wdd = None
        Settings.mask = EventsCodes.OP_FLAGS["IN_CREATE"]  # watched events
        Settings.is_quiet = False
        Settings.dashboard_path = None

    def argument_parser(self):
        opts = []
        args = []
        try:
            opts, args = getopt.getopt(sys.argv[1:],"hqd:c:",["help","quiet","dashboard=","component="])

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
            if o in ("-d","--dashboard"):
                Settings.dashboard_path = a

        try:
            Settings.monitor_path = args[0]
        except IndexError, e:
            output("Error: directory to monitor argument is required")
            self.help()
            abort(1)

    def help(self):
        output("Usage: qamon.py <directory>")
        output("Common Options:")
        output("  -h | --help        Print help information (this message)")
        output("  -q | --quiet       Don't print anything")
        output("  -d | --dashboard=  The directory in which the dashboard files are stored or")
        output("                     should be stored")
        output("  -c | --component=  Select at least and only one component to test (i.e.,")
        output("                     winforms or moonlight).")
        output("")
        output("Description:")
        output("  qamon will begin by monitoring <directory>.  Any new", False)
        output("subdirectories\n  (i.e., subdirectories created", False)
        output("after qamon runs) will also be monitored.\n  New", False)
        output("subdirectories of these directories will also be", False)
        output("monitored, and so\n  on.", False)

class Monitor(object):

    def __init__(self, monitor_path=None):
        if monitor_path is not None:
            Settings.monitor_path = monitor_path

    def run(self):
        p = PTmp()
        notifier = Notifier(Settings.wm, p)
        # TODO: Not necessary to watch the directories that already have
        # a procedures.xml
        Settings.wdd = Settings.wm.add_watch(Settings.monitor_path, Settings.mask, rec=True)
        notifier.loop()

class PTmp(ProcessEvent):
    def process_IN_CREATE(self, event):
        pkg_status_re = re.compile(".+_package_status")
        self.component = event.pathname.split('/')[4]
        if os.path.isdir(event.pathname):
            Settings.wdd = Settings.wm.add_watch(event.pathname, Settings.mask)
        elif os.path.basename(event.pathname) == "procedures.xml":
            print "INFO: New procedures.xml!"
            output(event.pathname)
            # no longer watch the the parent directory
            dir = os.path.dirname(event.pathname)
            # print "INFO: Removing watch from %s" % dir
            # Settings.wm.rm_watch(Settings.wdd[dir])
            # print "INFO: Removed"
            print "INFO: Building dashboard..."
            # build the dashboard
            # TODO:  Only call the dashboard module's update method when
            # available, instead of rebuilding the table from scratch for
            # each update
            self.build_dashboard()
        elif pkg_status_re.match(os.path.basename(event.pathname)):
            print "INFO: new package status file %s" % event.pathname
            print "INFO: Building dashboard..."
            self.build_dashboard()

    def build_dashboard(self):
        pb = None
        if Settings.dashboard_path is not None:
            pb = dashboard.PageBuilder(Settings.monitor_path,
                                       self.component,
                                       Settings.dashboard_path)
        else:
            pb = dashboard.PageBuilder(Settings.monitor_path,
                                       self.component)
        pb.build_regression()
        pb.build_smoke()
        pb.build_main()

if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    m = Monitor()
    m.run()
