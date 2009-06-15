##
## Patch and build Gtk#
## Build is not strictly required, but it will generate all the necessary
## files.
##

PATCH_STAMP = patch.stamp

GTK_SHARP_PATCHES = \
	patches/001_gtk-sharp_target-smcs.diff

ATK_VERSION=1.10.3
ATK_FILENAME=atk-$(ATK_VERSION).tar.bz2
ATK_XML_URL=http://ftp.gnome.org/pub/gnome/sources/atk/1.10/$(ATK_FILENAME)
GTK_SHARP=gtk-sharp
GLIB_SHARP_PATH=$(GTK_SHARP)/glib/
GAPI_PARSER_PATH=$(GTK_SHARP)/parser/
GAPI_GENERATOR_PATH=$(GTK_SHARP)/generator/
ATK_SHARP_PATH=$(GTK_SHARP)/atk/
ATK_API_FILE=$(GAPI_PARSER_PATH)$(ATK_FILENAME)
GTK_SHARP_28_SOURCES_FILE=gtk-sharp-2.8-sources.xml
GTK_SHARP_28_SRCDIR=$(GAPI_PARSER_PATH)$(GTK_SHARP_28_SOURCES_FILE)
TUNER_FLAGS=-a ./DummyEntryPoint.exe -d ./ -o ../ -l none -b true

all-am: $(ASSEMBLY_STAGE_PATH)

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
	-keyfile:mono-uia.snk -target:library $(GTK_SHARP_FLAGS) \
	-out:bin/raw/$(ASSEMBLY)

DUMMY_REFERENCES = /r:bin/raw/mscorlib.dll /r:bin/raw/MoonAtkBridge.dll

REFERENCES = \
	System \
	System.Core \
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

$(ASSEMBLY_STAGE_PATH): stage $(GTK_SHARP) $(build_sources) $(ASSEMBLY_CONFIG)
	mkdir -p bin/raw/
	cp ../../moon/class/lib/2.1_raw/*.dll ./bin/raw/
	$(SMCS) $(CSCFLAGS) $(build_sources) $(build_references)
	$(SMCS) /t:exe DummyEntryPoint.cs $(DUMMY_REFERENCES) /out:bin/raw/DummyEntryPoint.exe
	cd bin/raw/ && $(MONO) --debug ../../$(LINKING_PATH)/monolinker.exe $(TUNER_FLAGS) \
		-s ResolveFromAssemblyStep:Mono.Tuner.MoonlightA11yAssemblyStep,Mono.Tuner
	rm bin/raw/DummyEntryPoint.exe
	rm bin/*.exe
	cp bin/*.dll stage/components/

stage/install.rdf: stage install.rdf
	cp -p install.rdf stage/install.rdf


##
## Generate XPI
##

xpi: stage/install.rdf novell-moonlight-a11y.xpi

novell-moonlight-a11y.xpi: 
	cd stage/ && zip -r9 ../novell-moonlight-a11y.xpi *

clean:
	rm -f $(PATCH_STAMP)
	rm -rf $(GTK_SHARP)/
	rm -rf stage/
	rm -rf bin/
	rm -f *.xpi
