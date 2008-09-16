#!/bin/bash

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../../bridge-glue/.libs/

#NUnit 2.2.x
#exec nunit-console2 UiaAtkBridgeTest.dll $* 

#method for NUnit 2.4.7
cp `find /usr/ -name nunit-console.exe 2> /dev/null | grep -v "\.0"` .
cp `find /usr/ -name nunit-console-runner.dll 2> /dev/null | grep -v "\.0"` .
cp `find /usr/ -name nunit.util.dll 2> /dev/null | grep -v "\.0" | grep -v NAnt` .
cp `find /usr/ -name nunit.core.dll 2> /dev/null | grep -v "\.0" | grep -v NAnt | grep -v monodevelop` .
cp `find /usr/ -name nunit.core.interfaces.dll 2> /dev/null | grep -v "\.0"` .
mono ./nunit-console.exe UiaAtkBridgeTest.dll -nothread -domain=none