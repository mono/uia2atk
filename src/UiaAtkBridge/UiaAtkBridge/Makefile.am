
EXTRA_DIST =  UiaAtkBridge.dll.config.in

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG" -keyfile:../mono-uia.snk

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
ASSEMBLY_COMPILER_FLAGS = -lib:@expanded_libdir@/mono/3.0 -lib:@expanded_libdir@/mono/accessibility -noconfig -codepage:utf8 -warn:4 -optimize+ -keyfile:../mono-uia.snk
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


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(ASSEMBLY).config

FILES =  \
	ActionImplementorHelper.cs \
	Adapter.cs \
	AmbiDictionary.cs \
	AssemblyInfo.cs \
	AutomationBridge.cs \
	Button.cs \
	CheckBoxButton.cs \
	ComboBox.cs \
	ComponentAdapter.cs \
	ComponentImplementorHelper.cs \
	ComponentParentAdapter.cs \
	Hyperlink.cs \
	Image.cs \
	List.cs \
	ListItem.cs \
	MenuItem.cs \
	Monitor.cs \
	Pane.cs \
	ParentAdapter.cs \
	ParentMenu.cs \
	ProgressBar.cs \
	RadioButton.cs \
	ScrollBar.cs \
	SelectionProviderUserHelper.cs \
	Spinner.cs \
	StatusBar.cs \
	TableImplementorHelper.cs \
	TextBoxEntryView.cs \
	TextImplementorHelper.cs \
	TextLabel.cs \
	ToolTip.cs \
	TopLevelRootItem.cs \
	Window.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System  \
	UIAutomationTypes \
	UIAutomationProvider \
	UIAutomationBridge \
	WindowsBase \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES)

install-data-local: gac-install

uninstall-local: gac-uninstall

include $(top_srcdir)/Makefile.include


$(ASSEMBLY).config: $(srcdir)/UiaAtkBridge.dll.config.in Makefile
	sed -e "s|\@expanded_lib_dir\@|@expanded_libdir@|" \
	    < $< > $@


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
