
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/UiaDbusCoreBridgeTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../DbusCore/bin/Debug/DbusCore.dll \
	../UiaDbusCoreBridge/bin/Debug/UiaDbusCoreBridge.dll
BUILD_DIR = bin/Debug

UIADBUSCOREBRIDGE_DLL_SOURCE=../UiaDbusCoreBridge/bin/Debug/UiaDbusCoreBridge.dll
DBUSCORE_DLL_SOURCE=../DbusCore/bin/Debug/DbusCore.dll
UIADBUSCOREBRIDGETESTS_DLL_MDB_SOURCE=bin/Debug/UiaDbusCoreBridgeTests.dll.mdb
UIADBUSCOREBRIDGETESTS_DLL_MDB=$(BUILD_DIR)/UiaDbusCoreBridgeTests.dll.mdb
UIADBUSCOREBRIDGE_DLL_MDB_SOURCE=../UiaDbusCoreBridge/bin/Debug/UiaDbusCoreBridge.dll.mdb
UIADBUSCOREBRIDGE_DLL_MDB=$(BUILD_DIR)/UiaDbusCoreBridge.dll.mdb
DBUSCORE_DLL_MDB_SOURCE=../DbusCore/bin/Debug/DbusCore.dll.mdb
DBUSCORE_DLL_MDB=$(BUILD_DIR)/DbusCore.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:../mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/UiaDbusCoreBridgeTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../DbusCore/bin/Debug/DbusCore.dll \
	../UiaDbusCoreBridge/bin/Release/UiaDbusCoreBridge.dll
BUILD_DIR = bin/Release

UIADBUSCOREBRIDGE_DLL_SOURCE=../UiaDbusCoreBridge/bin/Release/UiaDbusCoreBridge.dll
DBUSCORE_DLL_SOURCE=../DbusCore/bin/Release/DbusCore.dll
UIADBUSCOREBRIDGETESTS_DLL_MDB=
UIADBUSCOREBRIDGE_DLL_MDB=
DBUSCORE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=MonoTests.UiaDbusCoreBridge.resources.dll

PROGRAMFILES = \
	$(DBUSCORE_DLL) \
	$(UIADBUSCOREBRIDGE_DLL) \
	$(UIADBUSCOREBRIDGETESTS_DLL_MDB) \
	$(DBUSCORE_DLL_MDB) \
	$(UIADBUSCOREBRIDGE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AutomationBridgeTest.cs \
	DbusCorePointTest.cs \
	DbusCoreRectTest.cs \
	ProviderElementWrapperTest.cs \
	TestHelper.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System \
	System.Data \
	System.Drawing \
	Mono.Posix \
	-pkg:mono-nunit \
	System.Windows.Forms \
	../DbusCore/bin/Debug/DbusCore.dll \
	../UiaDbusCoreBridge/bin/Debug/UiaDbusCoreBridge.dll \
	$(NDESK_DBUS_LIBS) \
	$(MONO_UIA_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

UIADBUSCOREBRIDGE_DLL = $(BUILD_DIR)/UiaDbusCoreBridge.dll
DBUSCORE_DLL = $(BUILD_DIR)/DbusCore.dll

$(eval $(call emit-deploy-target,DBUSCORE_DLL))
$(eval $(call emit-deploy-target,DBUSCORE_DLL_MDB))
$(eval $(call emit-deploy-target,UIADBUSCOREBRIDGE_DLL))
$(eval $(call emit-deploy-target,UIADBUSCOREBRIDGE_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x dbuscorebridgetest.sh
