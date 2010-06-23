
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/Mono.UIAutomation.TestFramework.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

CONFIG_XML_SOURCE=Config.xml
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB_SOURCE=bin/Debug/Mono.UIAutomation.TestFramework.dll.mdb
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB=$(BUILD_DIR)/Mono.UIAutomation.TestFramework.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/Mono.UIAutomation.TestFramework.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

CONFIG_XML_SOURCE=Config.xml
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(CONFIG_XML) \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	Application.cs \
	AssemblyInfo.cs \
	Button.cs \
	Calendar.cs \
	CheckBox.cs \
	ComboBox.cs \
	Config.cs \
	DataGrid.cs \
	DataItem.cs \
	Direction.cs \
	Document.cs \
	Edit.cs \
	Element.cs \
	Finder.cs \
	Group.cs \
	Header.cs \
	HeaderItem.cs \
	Helper.cs \
	Hyperlink.cs \
	Image.cs \
	List.cs \
	ListItem.cs \
	MenuBar.cs \
	Menu.cs \
	MenuItem.cs \
	Pane.cs \
	ProcedureLogger.cs \
	RadioButton.cs \
	ScrollBar.cs \
	Separator.cs \
	Slider.cs \
	Spinner.cs \
	SplitButton.cs \
	StatusBar.cs \
	Tab.cs \
	TabItem.cs \
	Table.cs \
	TestBase.cs \
	Text.cs \
	Thumb.cs \
	TitileBar.cs \
	ToolBar.cs \
	ToolTip.cs \
	Tree.cs \
	TreeItem.cs \
	UIAutomationMetadata.cs \
	Utils.cs \
	Window.cs

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Config.xml \
	mono.uiautomation.testframework.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Drawing \
	System.Windows.Forms \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	$(MONO_UIA_LIBS) \
	$(NUNIT_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

CONFIG_XML = $(BUILD_DIR)/Config.xml
MONO_UIAUTOMATION_TESTFRAMEWORK_PC = $(BUILD_DIR)/mono.uiautomation.testframework.pc

$(eval $(call emit-deploy-target,CONFIG_XML))
$(eval $(call emit-deploy-wrapper,MONO_UIAUTOMATION_TESTFRAMEWORK_PC,mono.uiautomation.testframework.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
