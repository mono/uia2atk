#!/bin/sh

MOONLIGHT_PATH=../../../../trunk/moon_clean
OUTPUT_PATH=./sl
LINKER_PATH="$MOONLIGHT_PATH/class/lib/tuner"
LINKER="$LINKER_PATH/monolinker.exe"

if test -d $MOONLIGHT_PATH
then
	echo "Found moon"
else
	echo "$MOONLIGHT_PATH does not exist"
	exit 1
fi

INPUT_PATH="$GTK_SHARP_PATH/moonbin"


LINKER_STEPS="-s ResolveFromAssemblyStep:Mono.Tuner.MoonlightAssemblyStep,Mono.Tuner"

LINKER_COMMON_FLAGS="-l none -o $OUTPUT_PATH -d $INPUT_PATH -b true -m secattrs $MOONLIGHT_PATH/class/tuning/SecurityAttributes"

MONO_PATH="$LINKER_PATH:$MONO_PATH"
#MONO_LOG_LEVEL="debug" MONO_LOG_MASK="asm"
mono --debug $LINKER $LINKER_COMMON_FLAGS -a $INPUT_PATH/glib-sharp.dll $LINKER_STEPS
sn -q -R $OUTPUT_PATH/glib-sharp.dll $GTK_SHARP_PATH/gtk-sharp.snk

MONO_PATH="$LINKER_PATH:$$MONO_PATH" mono --debug $LINKER $LINKER_COMMON_FLAGS -a $INPUT_PATH/atk-sharp.dll $LINKER_STEPS
sn -q -R $OUTPUT_PATH/atk-sharp.dll $GTK_SHARP_PATH/gtk-sharp.snk

#FIXME: cp from $MONO_PREFIX/lib/moon/plugin to get the no-raw assemblies
cp -rfvp $OUTPUT_PATH/*.dll lib

