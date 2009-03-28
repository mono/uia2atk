#!/bin/bash

#FIXME: don't distribute .dll.config files, they should be generated on the fly
find `pkg-config --variable=libdir gtk-sharp` -name atk-sharp.dll.config | grep 2\.0 | grep gac | xargs -r cp -v -p --target-directory=.
find `pkg-config --variable=libdir gtk-sharp` -name glib-sharp.dll.config | grep 2\.0 | grep gac | xargs -r cp -v -p --target-directory=.

rm -rf components && rm -f *.xpi && mkdir components && mv *.dll* components
rm -f create-xpi.sh
zip -r9 novell-moonlight-a11y.xpi *
