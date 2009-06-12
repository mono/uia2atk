
EXTRA_DIST =  

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/AtspiUiaSourceTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../AtspiUiaSource/bin/Debug/AtspiUiaSource.dll
BUILD_DIR = bin/Debug

ATSPIUIASOURCE_DLL_SOURCE=../AtspiUiaSource/bin/Debug/AtspiUiaSource.dll
ATSPIUIASOURCETESTS_DLL_MDB_SOURCE=bin/Debug/AtspiUiaSourceTests.dll.mdb
ATSPIUIASOURCETESTS_DLL_MDB=$(BUILD_DIR)/AtspiUiaSourceTests.dll.mdb
ATSPIUIASOURCE_DLL_MDB_SOURCE=../AtspiUiaSource/bin/Debug/AtspiUiaSource.dll.mdb
ATSPIUIASOURCE_DLL_MDB=$(BUILD_DIR)/AtspiUiaSource.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/AtspiUiaSourceTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../AtspiUiaSource/bin/Release/AtspiUiaSource.dll
BUILD_DIR = bin/Release

ATSPIUIASOURCE_DLL_SOURCE=../AtspiUiaSource/bin/Release/AtspiUiaSource.dll
ATSPIUIASOURCETESTS_DLL_MDB=
ATSPIUIASOURCE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=MonoTests.AtspiUiaSource.resources.dll

PROGRAMFILES = \
	$(ATSPIUIASOURCE_DLL) \
	$(ATSPIUIASOURCETESTS_DLL_MDB) \
	$(ATSPIUIASOURCE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AssemblyInfo.cs \
	Base.cs \
	IElementTest.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System \
	System.Data \
	Mono.Posix \
	-pkg:mono-nunit \
	../AtspiUiaSource/bin/Debug/AtspiUiaSource.dll \
	$(ATSPI_SHARP_LIBS) \
	$(MONO_UIA_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

ATSPIUIASOURCE_DLL = $(BUILD_DIR)/AtspiUiaSource.dll

$(eval $(call emit-deploy-target,ATSPIUIASOURCE_DLL))
$(eval $(call emit-deploy-target,ATSPIUIASOURCE_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x atspiuiasourcetest.sh
