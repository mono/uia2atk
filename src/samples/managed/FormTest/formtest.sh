#!/bin/bash

export LD_LIBRARY_PATH=../../../../../../../gtk-sharp/atk/glue/.libs/

cp /usr/lib/gtk-2.0/modules/libatk-bridge.so ../../../../../../../gtk-sharp/atk/glue/.libs/ && \
cp ../../../../../UiaAtkBridge/bridge-glue/bin/Debug/libbridge-glue.so ../../../../../../../gtk-sharp/atk/glue/.libs/ && \
exec mono FormTest.exe
