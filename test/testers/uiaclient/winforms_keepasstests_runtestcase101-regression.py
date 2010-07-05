#!/usr/bin/env python

###############################################################################
# Written by:  calen chen <cachen@novell.com>
# Date:        May 12 2010
# Description: RunTestCase101 for winforms KeePass application via UIAClientAPI
###############################################################################

import os
import subprocess as s
from sys import path

test_dir = path[0]
i = test_dir.rfind("/")
test_path = test_dir[:i]
test_dll = "uiaclient/Tests/bin/Debug/MonoTests.Mono.UIAutomation.UIAClientAPI.dll"
uiatest_dll = os.path.join(test_path, test_dll)
app = "uiaclient/Tests/bin/Debug/KeePass.exe"
app_path = os.path.join(test_path, app)

if not os.path.exists(app_path):
    print "Are you sure %s exist?" % app_path
    
if not os.path.exists(uiatest_dll):
    os.system("%s/uiaclient/autogen.sh && make" % test_path)

os.system("nunit-console2 %s -run=MonoTests.Mono.UIAutomation.UIAClientAPI.Winforms.KeePassTests.RunTestCase101" % uiatest_dll)


