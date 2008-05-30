#!/bin/bash

export LD_LIBRARY_PATH=../../../../../../../gtk-sharp/atk/glue/.libs/

exec mono FormTest.exe
