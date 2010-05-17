#!/usr/bin/env python

###############################################################################
# Written by:  calen chen <cachen@novell.com>
# Date:        May 12 2010
# Description: RunTestCase103 for winforms KeePass application via UIAClientAPI
###############################################################################

import os
from sys import path

test_dir = path[0]
i = test_dir.rfind("/")
test_path = test_dir[:i]
dll_path = "uiaclient/Tests/bin/Debug"
test_dll = "MonoTests.Mono.UIAutomation.UIAClientAPI.dll"
uiatest_path = os.path.join(test_path, dll_path)
uiatest_dll = os.path.join(uiatest_path, test_dll)
app = "KeePass.exe"
app_path = os.path.join(uiatest_path, app)

if not os.path.exists(app_path):
    print "Please copy %s to %s" % (app, uiatest_path)
    
if not os.path.exists(uiatest_dll):
    os.system("%s/uiaclient/autogen.sh && make" % test_path)

os.system("nunit-console2 %s -run=MonoTests.Mono.UIAutomation.UIAClientAPI.Winforms.KeePassTests.RunTestCase103 >%s%s" % \
(uiatest_dll, uiatest_path, "/Resources/logs"))


