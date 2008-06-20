
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/UiaAtkBridge.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

UIAATKBRIDGE_DLL_MDB_SOURCE=bin/Debug/UiaAtkBridge.dll.mdb
UIAATKBRIDGE_DLL_MDB=$(BUILD_DIR)/UiaAtkBridge.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/UiaAtkBridge.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

UIAATKBRIDGE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(UIAATKBRIDGE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(UIAATKBRIDGE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	Monitor.cs \
	AssemblyInfo.cs \
	TopLevelRootItem.cs \
	Window.cs \
	AutomationBridge.cs \
	Button.cs \
	Adapter.cs \
	ParentAdapter.cs \
	TextLabel.cs \
	TextImplementorHelper.cs \
	CheckBox.cs \
	Pane.cs \
	ComponentExpert.cs \
	ComponentAdapter.cs \
	ComponentParentAdapter.cs \
	ComboBox.cs \
	Menu.cs \
	MenuItem.cs \
	ToggleButton.cs \
	RadioButton.cs \
	List.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	uiaatkbridge.pc.in 

REFERENCES =  \
	System \
	$(OLIVE_LIBS) \
	$(MONO_UIA_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

UIAATKBRIDGE_PC = $(BUILD_DIR)/uiaatkbridge.pc

$(eval $(call emit-deploy-wrapper,UIAATKBRIDGE_PC,uiaatkbridge.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
