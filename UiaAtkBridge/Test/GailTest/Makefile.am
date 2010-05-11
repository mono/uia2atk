EXTRA_DIST =

ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/GailTest.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = $(GAILTESTAPP)
BUILD_DIR = bin

GAILTEST_DLL_MDB_SOURCE=bin/GailTest.dll.mdb
GAILTEST_DLL_MDB=$(BUILD_DIR)/GailTest.dll.mdb


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
	System.Xml \
	System.Core \
	nunit.framework \
	$(MONO_UIA_LIBS) \
	$(GTK_SHARP_20_LIBS)

DLL_REFERENCES = $(ATKTEST)

CLEANFILES = $(PROGRAMFILES) \
	bin/GailTestApp.exe \
	bin/GailTestApp.exe.mdb \
	bin/AtkTest.dll \
	bin/AtkTest.dll.mdb

MAINTAINERCLEANFILES = \
	gailtest.sh

EXTRA_DIST = \
	gailtest.sh.in

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

bin/GailTestApp.exe: ../GailTestApp/bin/GailTestApp.exe
	@mkdir -p bin
	cp -a $< $@

bin/AtkTest.dll: ../AtkTest/bin/AtkTest.dll
	@mkdir -p bin
	cp -a $< $@
	cp -a $<.mdb $@.mdb

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	@chmod +x ./gailtest.sh
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
