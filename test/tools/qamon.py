#!/usr/bin/env python
import os
from pyinotify import WatchManager, Notifier, ThreadedNotifier, ProcessEvent, \
                      EventsCodes

class Settings(object):
    MON_PATH = "/home/a11y/code/uia2atk/test/logs"
    wm = WatchManager()
    mask = EventsCodes.OP_FLAGS["IN_CREATE"]  # watched events
    watch_file = "procedures.xml"
    wdd = None

class Main(object):
    def run(self):
        p = PTmp()
        notifier = Notifier(Settings.wm, p)
        Settings.wdd = Settings.wm.add_watch(Settings.MON_PATH, Settings.mask)
        notifier.loop()

class PTmp(ProcessEvent):
    def process_IN_CLOSE_WRITE(self, event):
        print "Write close:", event.pathname
    def process_IN_MOVED_TO(self, event):
        print "Moved to:", event.pathname
    def process_IN_CREATE(self, event):
        print "Created:", event.pathname
        if os.path.isdir(event.pathname):
            Settings.wdd = Settings.wm.add_watch(event.pathname, Settings.mask, rec=True)
        elif os.path.basename(event.pathname) == Settings.watch_file:
            print "INFO: New procedures.xml!"
            # no longer watch the the parent directory
            dir = os.path.dirname(event.pathname)
            Settings.wm.rm_watch(Settings.wdd[dir])

if __name__ == "__main__":
    m = Main()
    m.run()
