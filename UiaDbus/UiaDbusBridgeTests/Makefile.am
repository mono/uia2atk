
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:$(top_srcdir)/mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/UiaDbusBridgeTests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll \
	$(top_builddir)/UiaDbusBridge/bin/Debug/UiaDbusBridge.dll
BUILD_DIR = bin/Debug

UIADBUSBRIDGE_DLL_SOURCE=$(top_builddir)/UiaDbusBridge/bin/Debug/UiaDbusBridge.dll
UIADBUS_DLL_SOURCE=$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll
UIADBUSBRIDGETESTS_DLL_MDB_SOURCE=bin/Debug/UiaDbusBridgeTests.dll.mdb
UIADBUSBRIDGETESTS_DLL_MDB=$(BUILD_DIR)/UiaDbusBridgeTests.dll.mdb
UIADBUSBRIDGE_DLL_MDB_SOURCE=$(top_builddir)/UiaDbusBridge/bin/Debug/UiaDbusBridge.dll.mdb
UIADBUSBRIDGE_DLL_MDB=$(BUILD_DIR)/UiaDbusBridge.dll.mdb
UIADBUS_DLL_MDB_SOURCE=$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll.mdb
UIADBUS_DLL_MDB=$(BUILD_DIR)/UiaDbus.dll.mdb
UIADBUS_DLL_CONFIG_SOURCE=$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll.config
UIADBUS_DLL_CONFIG=$(BUILD_DIR)/UiaDbus.dll.config

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = mcs
ASSEMBLY_COMPILER_FLAGS = -keyfile:$(top_srcdir)/mono-uia.snk -noconfig -codepage:utf8 -warn:4 -warnaserror -optimize+
ASSEMBLY = bin/Release/UiaDbusBridgeTests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll \
	$(top_builddir)/UiaDbusBridge/bin/Release/UiaDbusBridge.dll
BUILD_DIR = bin/Release

UIADBUSBRIDGE_DLL_SOURCE=$(top_builddir)/UiaDbusBridge/bin/Release/UiaDbusBridge.dll
UIADBUS_DLL_SOURCE=$(top_builddir)/UiaDbus/bin/Release/UiaDbus.dll
UIADBUSBRIDGETESTS_DLL_MDB=
UIADBUSBRIDGE_DLL_MDB=
UIADBUS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=MonoTests.UiaDbusBridge.resources.dll

PROGRAMFILES = \
	$(UIADBUS_DLL) \
	$(UIADBUSBRIDGE_DLL) \
	$(UIADBUSBRIDGETESTS_DLL_MDB) \
	$(UIADBUS_DLL_MDB) \
	$(UIADBUS_DLL_CONFIG) \
	$(UIADBUSBRIDGE_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES)

FILES =  \
	AutomationBridgeTest.cs \
	ProviderElementWrapperTest.cs \
	TestHelper.cs \
	UiaDbusPointTest.cs \
	UiaDbusRectTest.cs 

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
	$(top_builddir)/UiaDbus/bin/Debug/UiaDbus.dll \
	$(top_builddir)/UiaDbusBridge/bin/Debug/UiaDbusBridge.dll \
	$(DBUS_SHARP_LIBS) \
	$(MONO_UIA_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) \
	bin/Debug/TestResult.xml

include $(top_srcdir)/Makefile.include

UIADBUSBRIDGE_DLL = $(BUILD_DIR)/UiaDbusBridge.dll
UIADBUS_DLL = $(BUILD_DIR)/UiaDbus.dll

$(eval $(call emit-deploy-target,UIADBUS_DLL))
$(eval $(call emit-deploy-target,UIADBUS_DLL_MDB))
$(eval $(call emit-deploy-target,UIADBUS_DLL_CONFIG))
$(eval $(call emit-deploy-target,UIADBUSBRIDGE_DLL))
$(eval $(call emit-deploy-target,UIADBUSBRIDGE_DLL_MDB))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	@mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	@chmod +x dbusbridgetest.sh
