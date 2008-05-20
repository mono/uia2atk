#!/bin/bash

export LD_LIBRARY_PATH=~/mono-svn/gtk-sharp/atk/glue/.libs/

exec mono FormTest.exe
