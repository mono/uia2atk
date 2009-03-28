#!/bin/sh

GTK_SHARP_PATH=../../../gtksharp21
[ -a  $GTK_SHARP_PATH ] || (echo "$GTK_SHARP_PATH does not exist" && exit 1)

#this doesn't work on the parallel env because of BNC#489961
#(cd $GTK_SHARP_PATH; \
#	./bootstrap-2.12 --prefix=$MONO_PREFIX && make && (cd glib; make moonlight) && (cd atk; make moonlight))

cp -rfvp $GTK_SHARP_PATH/moonbin/*.dll lib
