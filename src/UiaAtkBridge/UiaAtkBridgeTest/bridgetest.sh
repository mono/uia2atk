#!/bin/bash

export LD_LIBRARY_PATH=../../../../../../gtk-sharp/glib/glue/.libs/

exec nunit-console2 UiaAtkBridgeTest.dll


