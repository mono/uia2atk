#!/bin/sh

GTK_SHARP_PATH=../../../gtk-sharp
MOONLIGHT_PATH=../../../moon
[ -a  $GTK_SHARP_PATH ] || (echo "$GTK_SHARP_PATH does not exist" && exit 1)

[ -a  $MOONLIGHT_PATH ] || (echo "$MOONLIGHT_PATH does not exist" && exit 1)

#this doesn't work on the parallel env because of BNC#489961
#(cd $GTK_SHARP_PATH; \
#	./bootstrap-2.12 --prefix=$MONO_PREFIX && make && (cd glib; make moonlight) && (cd atk; make moonlight))

#FIXME: move this to a pre-install target:
#cp *.dll $(MCS_PATH)/class/lib/net_2_1_raw
#FIXME: now call the tuner in moon/class/tuning

#FIXME: cp from $MONO_PREFIX/lib/moon/plugin to get the no-raw assemblies
cp -rfvp $GTK_SHARP_PATH/moonbin/*.dll lib
