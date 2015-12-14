ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/UiaAtkBridgeTest.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin

GAILTEST_DLL_MDB_SOURCE=bin/UiaAtkBridgeTest.dll.mdb
GAILTEST_DLL_MDB=$(BUILD_DIR)/UiaAtkBridgeTest.dll.mdb


AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GAILTEST_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	BridgeTester.cs \
	BridgeTests.cs \
	DialogTester.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

UIAATKBRIDGE = $(BUILD_DIR)/UiaAtkBridge.dll
ATKTEST = $(BUILD_DIR)/AtkTest.dll

REFERENCES =  \
	System  \
	System.Data  \
	System.Xml \
	System.Core \
	System.Drawing \
	$(NUNIT_LIBS) \
	System.Windows.Forms \
	$(MONO_UIA_LIBS) \
	$(GTK_SHARP_30_LIBS)

DLL_REFERENCES = $(ATKTEST) $(UIAATKBRIDGE)

CLEANFILES = $(PROGRAMFILES) \
	bin/AtkTest.dll \
	bin/AtkTest.dll.mdb \
	bin/UiaAtkBridge.dll \
	bin/UiaAtkBridge.dll.mdb

MAINTAINERCLEANFILES = \
	bridgetest.sh

EXTRA_DIST = \
	bridgetest.sh.in

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

bin/UiaAtkBridge.dll: ../../UiaAtkBridge/bin/UiaAtkBridge.dll
	@mkdir -p bin
	cp -a $< $@
	cp -a $<.mdb $@.mdb
	cp -a $<.config $@.config

bin/AtkTest.dll: ../AtkTest/bin/AtkTest.dll
	@mkdir -p bin
	cp -a $< $@
	cp -a $<.mdb $@.mdb

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	@chmod +x ./bridgetest.sh
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
