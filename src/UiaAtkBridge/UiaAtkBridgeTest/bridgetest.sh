#!/bin/bash

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../../bridge-glue/.libs/

#NUnit 2.2.x
#exec nunit-console2 UiaAtkBridgeTest.dll $* 

#method for NUnit 2.4.7
mono `find /usr/ -name nunit-console.exe 2> /dev/null | grep -v "\.0"` UiaAtkBridgeTest.dll -nothread