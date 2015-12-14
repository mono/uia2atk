ASSEMBLY_COMPILER_FLAGS = 

ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS += -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG" -keyfile:$(top_srcdir)/mono-uia.snk

ASSEMBLY = bin/AtkTest.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin

ATKTEST_DLL_MDB_SOURCE=bin/AtkTest.dll.mdb
ATKTEST_DLL_MDB=$(BUILD_DIR)/AtkTest.dll.mdb


AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(ATKTEST_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	AtkTester.cs \
	AtkTests.cs \
	AtSpiEvent.cs \
	BasicWidgetType.cs \
	EventCollection.cs \
	EventMonitor.cs \
	TestBase.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System \
	System.Xml \
	System.Core \
	$(NUNIT_LIBS) \
	$(GTK_SHARP_30_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES)

EXTRA_DIST = 

include $(top_srcdir)/Makefile.include


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
