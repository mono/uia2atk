#!/bin/bash

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../../bridge-glue/.libs/

exec nunit-console2 UiaAtkBridgeTest.dll $* 
