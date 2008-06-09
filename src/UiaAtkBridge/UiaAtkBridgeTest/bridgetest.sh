#!/bin/bash

export LD_LIBRARY_PATH=../../../../../../gtk-sharp/glib/glue/.libs/:../../../../../../gtk-sharp/atk/glue/.libs/

exec nunit-console2 UiaAtkBridgeTest.dll


