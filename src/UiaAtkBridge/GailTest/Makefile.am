EXTRA_DIST =

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/GailTest.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = $(GAILTESTAPP)
BUILD_DIR = bin/Debug

GAILTEST_DLL_MDB_SOURCE=bin/Debug/GailTest.dll.mdb
GAILTEST_DLL_MDB=$(BUILD_DIR)/GailTest.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -keyfile:../mono-uia.snk
ASSEMBLY = bin/Release/GailTest.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GAILTEST_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GAILTEST_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	GailTester.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

GAILTESTAPP = $(BUILD_DIR)/GailTestApp.exe
ATKTEST = $(BUILD_DIR)/AtkTest.dll
REFERENCES =  \
	System  \
	System.Core \
	WindowsBase \
	nunit.framework \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS)

DLL_REFERENCES = $(ATKTEST)

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/GailTestApp.exe \
	bin/Debug/GailTestApp.exe.mdb \
	bin/Debug/AtkTest.dll \
	bin/Debug/AtkTest.dll.mdb \
	bin/Release/GailTestApp.exe

MAINTAINERCLEANFILES = \
	gailtest.sh

EXTRA_DIST = \
	gailtest.sh.in

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

bin/Debug/GailTestApp.exe: ../../samples/managed/GailTestApp/bin/Debug/GailTestApp.exe
	@mkdir -p bin/Debug
	cp -a $< $@

bin/Release/GailTestApp.exe: ../../samples/managed/GailTestApp/bin/Release/GailTestApp.exe
	@mkdir -p bin/Release
	cp -a $< $@

bin/Debug/AtkTest.dll: ../AtkTest/bin/Debug/AtkTest.dll
	@mkdir -p bin/Debug
	cp -a $< $@
	cp -a $<.mdb $@.mdb

bin/Release/AtkTest.dll: ../AtkTest/bin/Release/AtkTest.dll
	@mkdir -p bin/Release
	cp -a $< $@

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	@chmod +x ./gailtest.sh
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
