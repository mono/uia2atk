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
GTK_SHARP_28_SOURCES=$(GAPI_PARSER_PATH)$(GTK_SHARP_28_SOURCES_FILE)
ATK_API_DIR=$(GAPI_PARSER_PATH)atk-$(ATK_VERSION)/atk



all: glib-atk-sharp-dll

glib-atk-sharp-dll: bin/glib-atk-sharp.dll

bin/glib-atk-sharp.dll: glib-sharp-dll atk-sharp-dll monomerge
	mkdir -p bin/
	cp $(GTK_SHARP)/moonbin/*.dll bin/
	mono merge/monomerge.exe -out bin/glib-atk-sharp.dll bin/glib-sharp.dll bin/atk-sharp.dll

atk-sharp-dll: $(GTK_SHARP)/moonbin/atk-sharp.dll

#after having parsed the API, the generator from gtk-sharp trunk will generate the .cs files of the binding
$(GTK_SHARP)/moonbin/atk-sharp.dll: bootstrap28 glib-sharp-dll $(ATK_API_RAW) $(GAPI_GENERATOR)
	( cd $(GTK_SHARP)/atk; make moonlight )

bootstrap28: $(GTK_SHARP)/bootstrap-2.8
	( cd $(GTK_SHARP); ./bootstrap-2.8 )

$(GTK_SHARP)/bootstrap-2.8: $(GTK_SHARP) patch
	cp bootstrap-2.8 $(GTK_SHARP)

$(GTK_SHARP):
	svn export http://anonsvn.mono-project.com/source/trunk/gtk-sharp $(GTK_SHARP)

patch: $(GTK_SHARP)
	if [ -f $(PATCH_STAMP) ]; then \
		for p in $(GTK_SHARP_PATCHES); do \
			if [ $(PATCH_STAMP) -ot $$p ]; then \
				rm -f $(PATCH_STAMP); \
				$(MAKE) $(AM_MAKEFLAGS) $(PATCH_STAMP); \
				break; \
			fi; \
		done; \
	else \
		$(MAKE) $(AM_MAKEFLAGS) $(PATCH_STAMP); \
	fi

$(PATCH_STAMP): $(GTK_SHARP_PATCHES)
	rm -f patch.tmp
	touch patch.tmp
	$(foreach p,$^, patch -p0 -N -d gtk-sharp < $p)
	mv -f patch.tmp $@

glib-sharp-dll: $(GTK_SHARP)/moonbin/glib-sharp.dll

$(GTK_SHARP)/moonbin/glib-sharp.dll: bootstrap28
	( cd $(GTK_SHARP)/glib; make moonlight )

#after parsing the API, we need to bootstrap again because this process will make a symbolic link
$(ATK_API_RAW): $(ATK_API_28_RAW)
	( cd $(GTK_SHARP); ./bootstrap-2.8 )

#we use the parser from gtk-sharp trunk to parse an old API
$(ATK_API_28_RAW): parser $(GTK_SHARP_28_SOURCES) $(ATK_API_DIR)
	( cd $(GTK_SHARP)/parser; PATH=.:$$PATH mono --debug gapi-parser.exe gtk-sharp-2.8-sources.xml )

parser: $(GAPI_PARSER)

$(GAPI_PARSER): $(GTK_SHARP) patch bootstrap28
	( cd $(GTK_SHARP)/parser && make )

$(GTK_SHARP_28_SOURCES):
	cp $(GTK_SHARP_28_SOURCES_FILE) $(GTK_SHARP_28_SOURCES)

$(ATK_API_DIR): $(ATK_API_FILE)
	( cd $(GAPI_PARSER_PATH); tar -xvf $(ATK_FILENAME) )

#we need to bring the API metadata from the oldest ATK version we support
$(ATK_API_FILE):
	wget $(ATK_XML_URL) --output-document=$(ATK_API_FILE)

$(GAPI_GENERATOR): bootstrap28
	( cd $(GAPI_GENERATOR_PATH); make )

monomerge: merge/monomerge.exe

merge/monomerge.exe: merge
	(cd merge; make)

merge:
	svn export http://anonsvn.mono-project.com/source/trunk/cecil/merge

clean:
	rm -f $(PATCH_STAMP)
	rm -rf $(GTK_SHARP)/
	rm -rf bin/
	rm -rf merge/
