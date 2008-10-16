#!/usr/bin/env python
import os
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
      print s,

def abort(s):
    sys.exit(s)

class Settings(object):

    def __init__(self):
        Settings.wm = WatchManager()
        Settings.wdd = None
        Settings.mask = EventsCodes.OP_FLAGS["IN_CREATE"]  # watched events
        Settings.is_quiet = False
        Settings.watch_file = "procedures.xml"

    def argument_parser(self):
        opts = []
        args = []
        try:
          opts, args = getopt.getopt(sys.argv[1:],"qf:",["quiet","file="])
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
          if o in ("-f","--file"):
            Settings.watch_file = a

        try:
            Settings.monitor_path = args[0]
        except IndexError, e:
            output("Error: directory to monitor argument is required")
            self.help()
            abort(1) 

    def help(self):
        output("Usage: qamon.py [-f <filename>] <directory>")
        output("Common Options:")
        output("  -h | --help        Print help information (this message)")
        output("  -q | --quiet       Don't print anything")
        output("  -d | --dir=        The directory where monitoring will begin")
        output("  -f | --file=       The file for which we are looking")
        output("")
        output("Description:")
        output("  qamon will begin by monitoring <directory>.  Any new", False)
        output("subdirectories\n  (i.e., subdirectories created", False)
        output("after qamon runs) will also be monitored.\n  New", False)
        output("subdirectories of these directories will also be", False)
        output("monitored, and so\n  on.  When <filename> is found", False)
        output("in any of these directories, the full path\n", False)
        output("  of <filename> is printed and the parent directory", False)
        output("<filename> is no longer\n  watched.  By default", False)
        output("<filename> is \"procedures.xml\"", False)

class Monitor(object):

    def __init__(self, monitor_path=None, watch_file=None):
        if monitor_path is not None:
            Settings.monitor_path = monitor_path
        if watch_file is not None:
            Settings.watch_file = watch_file

    def run(self):
        p = PTmp()
        notifier = Notifier(Settings.wm, p)
        # TODO: Not necessary to watch the directories that already have
        # a procedures.xml
        Settings.wdd = Settings.wm.add_watch(Settings.monitor_path, Settings.mask, rec=True)
        notifier.loop()

class PTmp(ProcessEvent):
    def process_IN_CREATE(self, event):
 	if os.path.isdir(event.pathname):
        	Settings.wdd = Settings.wm.add_watch(event.pathname, Settings.mask)
        elif os.path.basename(event.pathname) == Settings.watch_file:
            # print "INFO: New procedures.xml!"
            output(event.pathname)
            # no longer watch the the parent directory
            dir = os.path.dirname(event.pathname)
            #print "INFO: Removing watch from %s" % dir
            Settings.wm.rm_watch(Settings.wdd[dir])
            #print "INFO: Removed"
            #print "INFO: Buildling dashboard..."
            # build the dashboard
            # TODO:  Only call the dashboard module's update method when
            # available, instead of rebuilding the table from scratch for
            # each update
            pb = dashboard.PageBuilder(Settings.monitor_path)
            pb.build_all()

if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    m = Monitor()
    m.run()
