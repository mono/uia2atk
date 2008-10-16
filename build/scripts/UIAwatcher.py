#!/usr/bin/env python
import commands as c
import os
import getopt
import shutil
import sys
import tempfile
import time
from pyinotify import WatchManager, Notifier, ThreadedNotifier, ProcessEvent, \
                      EventsCodes

dirListNoArch = """
    /home/sshaw/tmp
""".split()
dirList32 = """
    /home/sshaw
""".split()

dirList64 = """
    /var/tmp
""".split()

removeRPMList = """
    mono-devel-*.rpm
    mono-core-debug*.rpm
    mono-data-f*.rpm
    mono-data-o*.rpm
    mono-data-p*.rpm
    mono-data-sy*.rpm
    mono-jscript-*.rpm
    mono-locale-*.rpm
    bytefx-data-*.rpm
    ibm-data-*.rpm
    mono-complete-*.rpm
    gtk-sharp2-complete-*.rpm
    gtk-sharp2-gapi-*.rpm
    gtk-sharp2-doc-*.rpm
""".split()

class Manager(object):
    q = Queue(0)

    def Add(self, arch):
        q.add(Stage1.build(arch))


class Stage1(object):
    
    def build(self, arch):
        path = tempfile.mkdtemp
        output = open(path, 'w')
        output.write('RPM Rev values for - ' + time.strftime("%m%d%y" + "\n\n\n")
    
        if(arch == 32):
            for dir in dirList32 + dirListNoArch:
                rev_dir = getLatestDir(dir)
                chdir(os.path.join(dir, rev_dir))
                shutil.copy2(*.rpm, path)
                output.write(os.path.basename(dir) + " - " + rev_dir + "\n")              

        if(arch == 64):
            for dir in dirList64 + dirListNoArch:
                rev_dir = getLatestDir(dir)
                chdir(os.path.join(dir, rev_dir))
                shutil.copy2(*.rpm, path)
                output.write(os.path.basename(dir) + " - " + rev_dir + "\n")              

        removeRPMS(path)
        output.close()
        return path
                
    def getLatestDir(self, dir)
        chdir(dir)
        return c.getoutput('ls -rtd */ | tail -n 1r')

    def removeRPMS(self, path)
        chdir(path)
        for rpm in removeRPMList:
            os.remove(rpm)
            
           
class Stage2(object):

    def setupRemote(self):
        c.getoutput('ssh builder@build1.sled.lab.novell.com /home/builder/bin/UIAupdate')

    def copyRPMS(self, path):
        c.getoutput('scp path builder@build1.sled.lab.novell.com:/srv/www/htdocs/uia/current')

class Stage3(object):

    def runSmokeTests(self):
        c.getoutput('ssh qa@uiaqa.sled.lab.novell.com python /var/qa/code/test/harness/remote_run.py -su -e mono-a11y@forge.novell.com,bgmerrell@novell.com,stshaw@novell.com -f bgmerrell@novell.com') 


class Settings(object):

    Settings.wm = WatchManager()
    Settings.wdd = None
    Settings.mask = EventsCodes.OP_FLAGS["IN_CREATE"]  # watched events
    Settings.dirNoArch = dirListNoArch
    Settings.dir32 = dirList32
    Settings.dir64 = dirList64

class Monitor(object):

    def __init__(self):

    def run(self):
        p = PEvents()
        notifier = ThreadedNotifier(Settings.wm, p)
    notifier.start()
    for dir in Settings.dirNoArch:
        Settings.wdd = Settings.wm.add_watch(dir, Settings.mask)
    for dir in Settings.dir32:
        Settings.wdd = Settings.wm.add_watch(dir, Settings.mask)
    for dir in Settings.dir64:
        Settings.wdd = Settings.wm.add_watch(dir, Settings.mask)
        notifier.loop()

class PEvents(ProcessEvent):
    def process_IN_CREATE(self, event):
        if os.path.isdir(event.pathname):
            if(os.dir.dirname(event.pathname) in Settings.dirListNoArch):
                Manager.add(32)
            if(os.dir.dirname(event.pathname) in Settings.dirList32):
                Manager.add(32)
                Manager.add(64)
            if(os.dir.dirname(event.pathname) in Settings.dirList64):
                Manager.add(64)

if __name__ == "__main__":
    manager = Manager()
    monitor = Monitor()
    monitor.run()
