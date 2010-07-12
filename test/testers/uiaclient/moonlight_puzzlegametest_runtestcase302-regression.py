#!/usr/bin/env python

###############################################################################
# Written by:  calen chen <cachen@novell.com>
# Date:        May 12 2010
# Description: RunTestCase302 for Moonlight PuzzleGame application via UIAClientAPI
###############################################################################

import os
from sys import path

test_dir = path[0]
i = test_dir.rfind("/")
test_path = test_dir[:i]
test_dll = "uiaclient/Tests/bin/Debug/MonoTests.Mono.UIAutomation.UIAClientAPI.dll"
uiatest_dll = os.path.join(test_path, test_dll)

if not os.path.exists(uiatest_dll):
    os.system("(cd %s/uiaclient/ && ./autogen.sh && make) " % test_path)

os.system("nunit-console2 %s -run=MonoTests.Mono.UIAutomation.UIAClientAPI.Moonlight.PuzzleGameTest.RunTestCase302" % uiatest_dll)


