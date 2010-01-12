
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
NOVELL_LOGO_PNG_SOURCE=Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=Resources/procedures.css
PROCEDURES_XSL_SOURCE=Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=Resources/uiaqa_logo.png
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
NOVELL_LOGO_PNG_SOURCE=Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=Resources/procedures.css
PROCEDURES_XSL_SOURCE=Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=Resources/uiaqa_logo.png
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(CONFIG_XML) \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB)  

PROGRAMFILES_RESOURCES = \
	$(NOVELL_LOGO_PNG) \
	$(PROCEDURES_CSS) \
	$(PROCEDURES_XSL) \
	$(UIAQA_LOGO_PNG)  

LINUX_PKGCONFIG = \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(PROGRAMFILES_RESOURCES) $(LINUX_PKGCONFIG) 

FILES = \
	Application.cs \
	Button.cs \
	CheckBox.cs \
	ComboBox.cs \
	Config.cs \
	DataGrid.cs \
	Document.cs \
	Edit.cs \
	Element.cs \
	Finder.cs \
	List.cs \
	ListItem.cs \
	MenuBar.cs \
	MenuItem.cs \
	Pane.cs \
	RadioButton.cs \
	ScrollBar.cs \
	Spinner.cs \
	TabItem.cs \
	TestBase.cs \
	ProcedureLogger.cs \
	Properties/AssemblyInfo.cs \
	Text.cs \
	ToolBar.cs \
	UIAutomationMetadata.cs \
	Utils.cs \
	Window.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Config.xml \
	Resources/novell_logo.png \
	Resources/procedures.css \
	Resources/procedures.xsl \
	Resources/uiaqa_logo.png \
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
	$(MONO_UIA_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(PROGRAMFILES_RESOURCES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

CONFIG_XML = $(BUILD_DIR)/Config.xml
NOVELL_LOGO_PNG = $(BUILD_DIR)/Resources/novell_logo.png
PROCEDURES_CSS = $(BUILD_DIR)/Resources/procedures.css
PROCEDURES_XSL = $(BUILD_DIR)/Resources/procedures.xsl
UIAQA_LOGO_PNG = $(BUILD_DIR)/Resources/uiaqa_logo.png
MONO_UIAUTOMATION_TESTFRAMEWORK_PC = $(BUILD_DIR)/mono.uiautomation.testframework.pc

$(eval $(call emit-deploy-target,CONFIG_XML))
$(eval $(call emit-deploy-target,NOVELL_LOGO_PNG))
$(eval $(call emit-deploy-target,PROCEDURES_CSS))
$(eval $(call emit-deploy-target,PROCEDURES_XSL))
$(eval $(call emit-deploy-target,UIAQA_LOGO_PNG))
$(eval $(call emit-deploy-wrapper,MONO_UIAUTOMATION_TESTFRAMEWORK_PC,mono.uiautomation.testframework.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
