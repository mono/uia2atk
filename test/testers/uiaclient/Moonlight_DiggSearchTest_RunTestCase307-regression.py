#!/usr/bin/env python

###############################################################################
# Written by:  calen chen <cachen@novell.com>
# Date:        May 12 2010
# Description: RunTestCase307 for Moonlight DiggSearchTest application via UIAClientAPI
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
    
if not os.path.exists(uiatest_dll):
    os.system("%s/uiaclient/autogen.sh && make" % test_path)

os.system("nunit-console2 %s -run=MonoTests.Mono.UIAutomation.UIAClientAPI.Moonlight.DiggSearchTestTest.RunTestCase307 >%s%s" % \
(uiatest_dll, uiatest_path, "/Resources/logs"))


