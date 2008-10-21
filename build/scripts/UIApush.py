# vim: set ts=4 sw=4 expandtab et: coding=UTF-8
#!/usr/bin/env python
import commands as c
import os
import tempfile
import time

base_path = "/home/sshaw/wa/msvn/release/packaging/snapshot_packages"

dir32 = {"mono":"/x86/mono",
         "olive":"/noarch/olive",
         "UiautomationWinforms":"/noarch/uiautomationwinforms",
         "libgdiplus":"/suse-110-i586/libgdiplus",
         "gtk-sharp212":"/suse-110-i586/gtk-sharp212",
         "UiaAtkBridge":"/suse-110-i586/uiaatkbridge"}

dir64 = {"mono-64":"/x86_64/mono",
         "olive":"/noarch/olive",
         "UiautomationWinforms":"/noarch/uiautomationwinforms",
         "libgdiplus-64":"/suse-110-x86_64/libgdiplus",
         "gtk-sharp212-64":"/suse-110-x86_64/gtk-sharp212",
         "UiaAtkBridge-64":"/suse-110-x86_64/uiaatkbridge"}

rmrpmlist = """
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

current_date = time.strftime("%m%d%y")

# Create both 32 and 64 tmp directories
tmp_path_32 = tempfile.mkdtemp()
tmp_path_64 = tempfile.mkdtemp()

# Print file headers for both 32 and 64 bit files
output_file_32 = open(os.path.join(tmp_path_32, "rpm_revs"), 'w')
output_file_32.write('RPM (32bit) Rev values for - ' + current_date + '\n\n\n')
# 64 bit
output_file_64 = open(os.path.join(tmp_path_64, "rpm_revs"), 'w')
output_file_64.write('RPM (64bit) Rev values for - ' + current_date + '\n\n\n')

# Fill 32 bit directories
def get32():
    for d in dir32:
        b = base_path + dir32[d]
        print b
        rev_dir = str(getLatestDir(b))
        print rev_dir
        os.chdir(b + "/" + rev_dir)
        cpcmd = 'cp *.rpm %s' % tmp_path_32
        c.getoutput(cpcmd)
        output_file_32.write(d + " - " + rev_dir + "\n")

# Fill 64 bit directories
def get64():
    for d in dir64:
        b = base_path + dir64[d]
        print b
        rev_dir = getLatestDir(b)
        print rev_dir
        os.chdir(b + "/" + rev_dir)
        cpcmd = 'cp *.rpm %s' % tmp_path_64
        c.getoutput(cpcmd)
        output_file_64.write(d + " - " + rev_dir + "\n")

def getLatestDir(dir):
    os.chdir(dir)
    lscmd = "ls -rtd */ | tail -n 1"
    return c.getoutput(lscmd)

def removerpms():
    os.chdir(tmp_path_32)
    for rpm in rmrpmlist:
        rmcmd = 'rm %s' % rpm
        c.getoutput(rmcmd)
    os.chdir(tmp_path_64)
    for rpm in rmrpmlist:
        rmcmd = 'rm %s' % rpm
        c.getoutput(rmcmd)


def Stage2():
    #ssh root@build1.sled.lab.novell.com /root/bin/UIAupdate
    print "ssh - UIAupdate"
    c.getoutput('ssh builder@build1.sled.lab.novell.com /home/builder/bin/UIAupdate')
    print "ssh - UIAupdate64"
    c.getoutput('ssh builder@build1.sled.lab.novell.com /home/builder/bin/UIAupdate64')

    #scp /tmp/uia/* root@build1.sled.lab.novell.com:/srv/www/htdocs/uia/current
    cpcmd = 'scp %s builder@build1.sled.lab.novell.com:/srv/www/htdocs/uia/current' % tmp_path_32
    print "scp - 32"
    c.getoutput(cpcmd)
    cpcmd = 'scp %s builder@build1.sled.lab.novell.com:/srv/www/htdocs/uia/64/current' % tmp_path_64
    print "scp - 64"
    c.getoutput(cpcmd)

def RunTests():
    #ssh qa@uiaqa.sled.lab.novell.com python /var/qa/code/test/harness/remote_run.py -su -e mono-a11y@forge.novell.com,bgmerrell@novell.com,stshaw@novell.com -f bgmerrell@novell.com
    results = c.getoutput('ssh qa@uiaqa.sled.lab.novell.com python /var/qa/code/test/harness/remote_run.py -su -e mono-a11y@forge.novell.com,bgmerrell@novell.com,stshaw@novell.com -f bgmerrell@novell.com')
    # If smoke tests pass, run regression tests
    if(results):
        c.getoutput('ssh qa@uiaqa.sled.lab.novell.com python /var/qa/code/test/harness/remote_run.py -su -e mono-a11y@forge.novell.com,bgmerrell@novell.com,stshaw@novell.com -f bgmerrell@novell.com')

def SendNotifications():
    #mailx stshaw@novell.com -s 'New RPMS - http://build1.sled.lab.novell.com/uia/current' -c bgmerrell@novell.com -c rawang@novell.com -c cachen@novell.com -c ngao@novell.com < /tmp/uia/rpm_revs
    #cmdstr = "mailx stshaw@novell.com -s 'New RPMS - http://build1.sled.lab.novell.com/uia/current' -c bgmerrell@novell.com -c rawang@novell.com -c cachen@novell.com -c ngao@novell.com < %s/rpm_revs" % tmp_path_32
    #c.getoutput(cmdstr)
    #cmdstr = "mailx stshaw@novell.com -s 'New RPMS - http://build1.sled.lab.novell.com/uia/current' -c bgmerrell@novell.com -c rawang@novell.com -c cachen@novell.com -c ngao@novell.com < %s/rpm_revs" % tmp_path_32
    #c.getoutput(cmdstr)
    mailcmd = "mailx stshaw@novell.com -s 'New RPM (32 and 64)'"
    c.getoutput(mailcmd)

if __name__ == "__main__":
    get32()
    get64()
    removerpms()
    Stage2()
    RunTests()
    SendNotifications()
