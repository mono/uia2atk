#!/bin/bash

export LD_LIBRARY_PATH=../../../../../../gtk-sharp/glib/glue/.libs/

#method for NUnit 2.2.0:
#exec nunit-console2 GailTest.dll

#method for NUnit 2.4.7
cp `find /usr/ -name nunit-console.exe 2> /dev/null | grep -v "\.0"` .
cp `find /usr/ -name nunit-console-runner.dll 2> /dev/null | grep -v "\.0"` .
cp `find /usr/ -name nunit.util.dll 2> /dev/null | grep -v "\.0" | grep -v NAnt` .
cp `find /usr/ -name nunit.core.dll 2> /dev/null | grep -v "\.0" | grep -v NAnt | grep -v monodevelop` .
cp `find /usr/ -name nunit.core.interfaces.dll 2> /dev/null | grep -v "\.0"` .
exec /usr/bin/mono ./nunit-console.exe GailTest.dll -domain=none

