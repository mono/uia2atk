#!/usr/bin/env python
import os
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
          opts, args = getopt.getopt(sys.argv[1:],"qf:d:",["quiet","file=","dir="])
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
          if o in ("-d","--dir"):
            Settings.monitor_path = a

    def help(self):
        output("Usage: qamon.py -d <directory> [-f <filename>]")
        output("Common Options:")
        output("  -h | --help        Print help information (this message)")
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
        Settings.wdd = Settings.wm.add_watch(Settings.monitor_path, Settings.mask)
        notifier.loop()

class PTmp(ProcessEvent):
    def process_IN_CREATE(self, event):
        if os.path.isdir(event.pathname):
            Settings.wdd = Settings.wm.add_watch(event.pathname, Settings.mask, rec=True)
        elif os.path.basename(event.pathname) == Settings.watch_file:
            # print "INFO: New procedures.xml!"
            output(event.pathname)
            # no longer watch the the parent directory
            dir = os.path.dirname(event.pathname)
            Settings.wm.rm_watch(Settings.wdd[dir])

if __name__ == "__main__":
    s = Settings()
    s.argument_parser()
    m = Monitor()
    m.run()
