##
## Patch and build Gtk#
## Build is not strictly required, but it will generate all the necessary
## files.
##

PATCH_STAMP = patch.stamp

GTK_SHARP_PATCHES = \
	patches/001_gtk-sharp_target-smcs.diff \
	patches/002_gtk-sharp_private-api.diff \
	patches/003_gtk-sharp_remove-glue-dep.diff \
	patches/004_gtk-sharp_gtype-name.diff \
	patches/005_gtk-sharp_remove-args.diff \
	patches/006_gtk-sharp_gobject-debugging.diff

ATK_VERSION=1.10.3
ATK_TARBALL=atk-$(ATK_VERSION).tar.bz2
ATK_XML_URL=http://ftp.gnome.org/pub/gnome/sources/atk/1.10/$(ATK_TARBALL)

GTK_SHARP=gtk-sharp
GLIB_SHARP_PATH=$(GTK_SHARP)/glib/
GAPI_PARSER_PATH=$(GTK_SHARP)/parser/
GAPI_GENERATOR_PATH=$(GTK_SHARP)/generator/
ATK_SHARP_PATH=$(GTK_SHARP)/atk/

GTK_SHARP_28_SOURCES_FILE=gtk-sharp-2.8-sources.xml
GTK_SHARP_28_SOURCES_PATH=resources/$(GTK_SHARP_28_SOURCES_FILE)
GTK_SHARP_28_SOURCES_DEST=$(GAPI_PARSER_PATH)$(GTK_SHARP_28_SOURCES_FILE)

TUNER_FLAGS=-a ./DummyEntryPoint.exe -d ./ -o ../ -l none -b true

DESC_FILE_UPSTREAM=a11y.xml
DESC_FILE_LOCAL=descriptors.xml
DESC_FILE_UPSTREAM_PATH=resources/$(DESC_FILE_UPSTREAM)
DESC_FILE_LOCAL_PATH=resources/$(DESC_FILE_LOCAL)

ANONSVN_TRUNK=http://anonsvn.mono-project.com/source/trunk/
ANONSVN_DESCRIPTORS=$(ANONSVN_TRUNK)moon/class/tuning/Descriptors/$(DESC_FILE_UPSTREAM)
ANONSVN_GTKSHARP=$(ANONSVN_TRUNK)gtk-sharp

all-am: descriptors tests-build

#FIXME: not svn export all gtk-sharp tree (we don't need glade,gtk,gdk,gio,etc.)
$(GTK_SHARP):
	svn export $(ANONSVN_GTKSHARP) $(GTK_SHARP) -r142364

	## Apply patches to gtk-sharp to add moonlight targets
	$(foreach p,$(GTK_SHARP_PATCHES), patch -p0 -N -d gtk-sharp < $p;)

	## Bootstrap for the oldest version of atk that we support
	cp resources/bootstrap-2.8 $(GTK_SHARP)
	cd $(GTK_SHARP) && ./bootstrap-2.8

	## Give the parser the correct API definitions for 2.8
	cp $(GTK_SHARP_28_SOURCES_PATH) $(GTK_SHARP_28_SOURCES_DEST)
	$(WGET) $(ATK_XML_URL) --output-document=$(GAPI_PARSER_PATH)$(ATK_TARBALL)
	cd $(GAPI_PARSER_PATH) && tar -xvf $(ATK_TARBALL)
	cd $(GAPI_PARSER_PATH) && $(MAKE) \
		&& PATH=.:$$PATH $(MONO) --debug gapi-parser.exe $(GTK_SHARP_28_SOURCES_FILE)
	
	## Bootstrap is required the second time around to generate the
	## atk-api.raw symlink
	cd $(GTK_SHARP) && ./bootstrap-2.8
	cd $(GLIB_SHARP_PATH) && $(MAKE) 
	cd $(GAPI_GENERATOR_PATH) && $(MAKE)
	cd $(ATK_SHARP_PATH) && $(MAKE) 

stage:
	mkdir -p stage/components/

##
## Build complete assembly
##

ASSEMBLY = MoonAtkBridge.dll
ASSEMBLY_STAGE_PATH = $(top_builddir)/stage/components/$(ASSEMBLY)
ASSEMBLY_CONFIG = $(ASSEMBLY_STAGE_PATH).config

GTK_SHARP_FLAGS = -define:NET_2_1 -unsafe -nowarn:0169,0612,0618
CSCFLAGS = \
	-noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ \
	-keyfile:mono-uia.snk -target:library $(GTK_SHARP_FLAGS)

OUT = -out:bin/raw/$(ASSEMBLY)
DUMMY_REFERENCES = /r:mscorlib.dll /r:MoonAtkBridge.dll
DUMMY_COMPILATION = $(SMCS) /t:exe ../../resources/DummyEntryPoint.cs $(DUMMY_REFERENCES) /out:DummyEntryPoint.exe

# check if there are differences between moon upstream and local
# (if there are, build will fail until you commit the new file)
COMPARE_DESCS = $(DIFF) -q $(DESC_FILE_UPSTREAM_PATH) $(DESC_FILE_LOCAL_PATH)


REFERENCES = \
	System \
	System.Core \
	System.Xml \
	System.Windows 

build_references = $(addprefix -r:, $(REFERENCES))

GLIB_FILES = $(addprefix $(GLIB_SHARP_PATH), $(shell cd $(GLIB_SHARP_PATH) && $(MAKE) list-sources))
ATK_FILES = $(addprefix $(ATK_SHARP_PATH), $(shell cd $(ATK_SHARP_PATH) && $(MAKE) list-sources))

FILES = \
	$(filter-out %AssemblyInfo.cs, $(GLIB_FILES) $(ATK_FILES)) \
	$(addprefix MoonAtkBridge/, $(shell cat MoonAtkBridge/MoonAtkBridge.sources))

build_sources = $(addprefix $(srcdir)/, $(FILES))

CONFIG_FILES = \
	$(srcdir)/$(ATK_SHARP_PATH)atk-sharp.dll.config \
	$(srcdir)/$(GLIB_SHARP_PATH)glib-sharp.dll.config

$(ASSEMBLY_CONFIG): stage $(GTK_SHARP) $(CONFIG_FILES)
	echo "<configuration>" > $@
	cat $(CONFIG_FILES) | grep -v configuration | sort | uniq >> $@
	echo "</configuration>" >> $@

T4_EXE=$(srcdir)/t4/TextTransform.exe
T4_GENERATOR=$(srcdir)/MoonAtkBridge/Moonlight.AtkBridge.Adapters/GeneratedAdapters.tt
T4_OUT=$(srcdir)/MoonAtkBridge/Moonlight.AtkBridge.Adapters/GeneratedAdapters.cs
T4_REFERENCES = \
	-r=$(shell pkg-config cecil --variable Libraries) \
	-r=System.Core

# bin/raw/ dir contains MoonAtkBridge.dll without sec attrs, plus moonlight assemblies with sec attrs
# bin/ dir contains MoonAtkBridge.dll with sec attrs and with internalized API
$(ASSEMBLY_STAGE_PATH): stage/install.rdf $(GTK_SHARP) $(build_sources) $(ASSEMBLY_CONFIG) $(T4_GENERATOR) stage/components/init.js
	mkdir -p bin/raw/
	cp $(MOON_PATH)/class/lib/2.1/*.dll ./bin/raw/
	$(SMCS) $(CSCFLAGS) $(OUT) $(build_sources) $(build_references)
	$(MONO) $(T4_EXE) $(T4_GENERATOR) $(T4_REFERENCES) -o $(T4_OUT)
	$(SMCS) $(CSCFLAGS) $(OUT) $(build_sources) $(T4_OUT) $(build_references)
	cd bin/raw/ && $(DUMMY_COMPILATION)
	cd bin/raw/ && $(MONO) --debug $(LINKING_PATH)/monolinker.exe $(TUNER_FLAGS) \
		-s ResolveFromAssemblyStep:Mono.Tuner.MoonlightA11yAssemblyStep,Mono.Tuner
	rm bin/raw/DummyEntryPoint.exe
	rm bin/*.exe
	cp bin/*.dll stage/components/

fake:
	mkdir -p bin/fake/
	$(SMCS) $(CSCFLAGS) -out:bin/fake/$(ASSEMBLY) resources/AttackVector.cs

# bin/fullraw/ dir contains MoonAtkBridge.dll and moonlight assemblies without sec attrs
descriptors: $(ASSEMBLY_STAGE_PATH)
	mkdir -p bin/fullraw/
	cp ./bin/raw/MoonAtkBridge.dll ./bin/fullraw/
	cp $(MOON_PATH)/class/lib/2.1_raw/*.dll ./bin/fullraw/
	cp $(MOON_PATH)/class/tuning/masterinfos/*.info ./bin/fullraw/
	cd bin/fullraw/ && $(DUMMY_COMPILATION)
	cd bin/fullraw/ && time $(MONO) --debug $(LINKING_PATH)/monolinker.exe \
		-c link -a ./DummyEntryPoint.exe \
		-s ResolveFromAssemblyStep:Mono.Tuner.MoonlightA11yUsageInspectionStep,Mono.Tuner
	cp bin/fullraw/output/$(DESC_FILE_LOCAL) $(DESC_FILE_LOCAL_PATH)
	rm -f $(DESC_FILE_UPSTREAM_PATH)
	$(WGET) $(ANONSVN_DESCRIPTORS) --output-document=$(DESC_FILE_UPSTREAM_PATH)
	$(COMPARE_DESCS)

# stage contains bin/ assemblies plus the files needed for XPI generation
stage/install.rdf: stage resources/install.rdf
	cp -p resources/install.rdf stage/install.rdf

stage/components/init.js: stage
	cp -p resources/init.js stage/components/init.js

tests-build: fake
	cd tests && $(MAKE) $(AM_MAKEFLAGS)


##
## Generate XPI
##

xpi: $(ASSEMBLY_STAGE_PATH) stage/install.rdf novell-moonlight-a11y.xpi

novell-moonlight-a11y.xpi: 
	cd stage/ && zip -r9 ../novell-moonlight-a11y.xpi *

clean:
	rm -f $(PATCH_STAMP)
	rm -f $(DESC_FILE_LOCAL_PATH)
	rm -f $(DESC_FILE_UPSTREAM_PATH)
	rm -rf $(GTK_SHARP)/
	rm -rf stage/
	rm -rf bin/
	rm -f *.xpi
