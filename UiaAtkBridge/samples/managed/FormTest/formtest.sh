#!/bin/bash

export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:../../../../../bridge-glue/.libs/

if [ -f /usr/lib/gtk-2.0/modules/libatk-bridge.so ]; then
	echo We are in a 32bits env
	export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/lib/gtk-2.0/modules/
elif [ -f /usr/lib64/gtk-2.0/modules/libatk-bridge.so ]; then
	echo We are in a 64bits env
	export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/usr/lib64/gtk-2.0/modules/
else
	echo libatk-bridge.so not found && exit
fi

#exec mono FormTest.exe
exec mono /usr/lib/IPCE/ipy/ipy.exe ../../../../../../test/samples/combobox_dropdownlist.py
