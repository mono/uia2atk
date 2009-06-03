PATCH_STAMP = patch.stamp

GTK_SHARP_PATCHES = \
	patches/001_gtk-sharp_target-smcs.diff

ATK_VERSION=1.10.3
ATK_FILENAME=atk-$(ATK_VERSION).tar.bz2
ATK_XML_URL=http://ftp.gnome.org/pub/gnome/sources/atk/1.10/$(ATK_FILENAME)
GTK_SHARP=gtk-sharp
GAPI_PARSER_PATH=$(GTK_SHARP)/parser/
GAPI_GENERATOR_PATH=$(GTK_SHARP)/generator/
GAPI_PARSER=$(GAPI_PARSER_PATH)gapi-parser.exe
GAPI_GENERATOR=$(GAPI_GENERATOR_PATH)gapi_codegen.exe
ATK_API_FILE=$(GAPI_PARSER_PATH)$(ATK_FILENAME)
ATK_SHARP_PATH=$(GTK_SHARP)/atk/
ATK_API_28_RAW=$(ATK_SHARP_PATH)atk-api-2.8.raw
ATK_API_RAW=$(ATK_SHARP_PATH)atk-api.raw
GTK_SHARP_28_SOURCES_FILE=gtk-sharp-2.8-sources.xml
GTK_SHARP_28_SRCDIR=$(GAPI_PARSER_PATH)$(GTK_SHARP_28_SOURCES_FILE)
ATK_API_DIR=$(GAPI_PARSER_PATH)atk-$(ATK_VERSION)/atk
MONO_CECIL_PATH=`pkg-config cecil --variable=Libraries`

all-am: bin/glib-atk-sharp.dll stage/install.rdf

stage:
	mkdir -p stage/components/

stage/install.rdf: stage install.rdf
	cp -p install.rdf stage/install.rdf

mono-merge/monomerge.exe:
	svn export http://anonsvn.mono-project.com/source/trunk/cecil/merge mono-merge
	$(GMCS) mono-merge/Mono.Merge/*.cs /r:$(MONO_CECIL_PATH) /r:System.Data.dll -out:mono-merge/monomerge.exe

bin/glib-atk-sharp.dll: $(GTK_SHARP) mono-merge/monomerge.exe
	mkdir -p bin/
	$(MONO) mono-merge/monomerge.exe -out bin/glib-atk-sharp.dll \
		$(GTK_SHARP)/moonbin/glib-sharp.dll $(GTK_SHARP)/moonbin/atk-sharp.dll

$(GTK_SHARP):
	svn export http://anonsvn.mono-project.com/source/trunk/gtk-sharp $(GTK_SHARP)

	## Apply patches to gtk-sharp to add moonlight targets
	$(foreach p,$(GTK_SHARP_PATCHES), patch -p0 -N -d gtk-sharp < $p)

	## Bootstrap for the oldest version of atk that we support
	cp bootstrap-2.8 $(GTK_SHARP)
	cd $(GTK_SHARP) && ./bootstrap-2.8

	## Give the parser the correct API definitions for 2.8
	cp $(GTK_SHARP_28_SOURCES_FILE) $(GTK_SHARP_28_SRCDIR)
	wget $(ATK_XML_URL) --output-document=$(ATK_API_FILE)
	cd $(GAPI_PARSER_PATH) && tar -xvf $(ATK_FILENAME)
	cd $(GTK_SHARP)/parser && $(MAKE) \
		&& PATH=.:$$PATH $(MONO) --debug gapi-parser.exe gtk-sharp-2.8-sources.xml
	
	## Bootstrap is required the second time around to generate the
	## atk-api.raw symlink
	cd $(GTK_SHARP) && ./bootstrap-2.8
	cd $(GTK_SHARP)/glib && $(MAKE) moonlight
	cp $(GTK_SHARP)/glib/glib-sharp.dll.config $(GTK_SHARP)/moonbin/
	cd $(GAPI_GENERATOR_PATH) && $(MAKE)
	cd $(GTK_SHARP)/atk && $(MAKE) moonlight
	cp $(GTK_SHARP)/atk/atk-sharp.dll.config $(GTK_SHARP)/moonbin/

merge: mono-merge/monomerge.exe bin/glib-atk-sharp.dll MoonAtkBridge/bin/*/MoonAtkBridge.dll
	$(MONO) mono-merge/monomerge.exe -out stage/components/MoonAtkBridge.dll \
		bin/glib-atk-sharp.dll MoonAtkBridge/bin/*/MoonAtkBridge.dll

xpi: stage/install.rdf merge bin/xpi/novell-moonlight-a11y.xpi

bin/xpi/novell-moonlight-a11y.xpi: 
	mkdir -p bin/xpi/
	cd stage/ && zip -r9 ../bin/xpi/novell-moonlight-a11y.xpi *

clean:
	rm -f $(PATCH_STAMP)
	rm -rf $(GTK_SHARP)/
	rm -rf bin/
	rm -rf stage/
	rm -rf mono-merge/

SUBDIRS = \
	. \
	MoonAtkBridge
