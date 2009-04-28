#!/bin/sh

GTK_SHARP_PATH=../../../gtk-sharp
MOONLIGHT_PATH=../../../moon
MCS_PATH=../../../../trunk/mcs
OUTPUT_PATH=./sl
LINKER_PATH="$MOONLIGHT_PATH/class/lib/tuner"
LINKER="$LINKER_PATH/monolinker.exe"
ATK_VERSION="1.10.3"
ATK_FILENAME="atk-$ATK_VERSION.tar.bz2"
ATK_URL="http://ftp.gnome.org/pub/gnome/sources/atk/1.10/$ATK_FILENAME"

GAPI_PARSER_PATH="$GTK_SHARP_PATH/parser"
GAPI_PARSER_FILE="$GAPI_PARSER_PATH/gapi-parser.exe"

if test -d $GTK_SHARP_PATH
then
	echo "Found gtk-sharp"
else
	echo "$GTK_SHARP_PATH does not exist"
	exit 1
fi

if test -d $MOONLIGHT_PATH
then
	echo "Found moon"
else
	echo "$MOONLIGHT_PATH does not exist"
	exit 1
fi

if test -d $MCS_PATH
then
	echo "Found mcs"
else
	echo "$MCS_PATH does not exist"
	exit 1
fi

INPUT_PATH="$GTK_SHARP_PATH/moonbin"


if test -f $GAPI_PARSER_PATH/$ATK_FILENAME
then
	echo "Found $ATK_FILENAME"
else
	echo "$ATK_FILENAME does not exist, trying to download it..."
	wget $ATK_URL --output-document=$GAPI_PARSER_PATH/$ATK_FILENAME
	if test -f $GAPI_PARSER_PATH/$ATK_FILENAME
	then
		echo "Found $ATK_FILENAME"
		rm -rf $GAPI_PARSER_PATH/atk-$ATK_VERSION/
		(cd $GAPI_PARSER_PATH/; \
			tar -xvf $ATK_FILENAME )
	else
		echo "$ATK_FILENAME does not exist, it could not be downloaded"
		exit 1
	fi
fi

if test -f $GAPI_PARSER_FILE
then
	echo "Found $GAPI_PARSER_FILE"
else
	echo "$GAPI_PARSER_FILE does not exist, trying to construct it..."

	(cd $GAPI_PARSER_PATH/; \
		make && make install )
	if test -f $GAPI_PARSER_FILE
	then
		echo "Found $GAPI_PARSER_FILE"
	else
		echo "$GAPI_PARSER_FILE does not exist, it could not be constructed"
		exit 1
	fi
fi

cp gtk-sharp-2.8-sources.xml $GAPI_PARSER_PATH
(cd $GAPI_PARSER_PATH/; \
	./gapi2-parser gtk-sharp-2.8-sources.xml )

# TODO: verify that the .raw file generated has an <api> element with a parser_version attrib
cp bootstrap-2.8 $GTK_SHARP_PATH

if test -f $GTK_SHARP_PATH/gtksharp21code.diff
then
	echo "Found $GTK_SHARP_PATH/gtksharp21code.diff"
else
	echo "$GTK_SHARP_PATH/gtksharp21code.diff does not exist, copying and applying..."

	cp ../../patches/gtksharp21code.diff $GTK_SHARP_PATH
	(cd $GTK_SHARP_PATH/; \
		patch -p0 < gtksharp21code.diff )
fi

#this doesn't work on the parallel env from MD because of BNC#489961
(cd $GTK_SHARP_PATH; \
	./bootstrap-2.8 --prefix=$MONO_PREFIX && make && (cd glib; make moonlight) && (cd atk; make moonlight))

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

