
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/Tests.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../UIAClientTestFramework/bin/Debug/UIAClientTestFramework.dll
BUILD_DIR = bin/Debug/

UIACLIENTTESTFRAMEWORK_DLL_SOURCE=../UIAClientTestFramework/bin/Debug/UIAClientTestFramework.dll
CONFIG_XML_SOURCE=../UIAClientTestFramework/Config.xml
NOVELL_LOGO_PNG_SOURCE=../UIAClientTestFramework/Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=../UIAClientTestFramework/Resources/procedures.css
PROCEDURES_XSL_SOURCE=../UIAClientTestFramework/Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=../UIAClientTestFramework/Resources/uiaqa_logo.png
TESTS_DLL_MDB_SOURCE=bin/Debug/Tests.dll.mdb
TESTS_DLL_MDB=$(BUILD_DIR)/Tests.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/Tests.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../UIAClientTestFramework/bin/Release/UIAClientTestFramework.dll
BUILD_DIR = bin/Release/

UIACLIENTTESTFRAMEWORK_DLL_SOURCE=../UIAClientTestFramework/bin/Release/UIAClientTestFramework.dll
CONFIG_XML_SOURCE=../UIAClientTestFramework/Config.xml
NOVELL_LOGO_PNG_SOURCE=../UIAClientTestFramework/Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=../UIAClientTestFramework/Resources/procedures.css
PROCEDURES_XSL_SOURCE=../UIAClientTestFramework/Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=../UIAClientTestFramework/Resources/uiaqa_logo.png
TESTS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(UIACLIENTTESTFRAMEWORK_DLL) \
	$(CONFIG_XML) \
	$(TESTS_DLL_MDB)  

PROGRAMFILES_RESOURCES = \
	$(NOVELL_LOGO_PNG) \
	$(PROCEDURES_CSS) \
	$(PROCEDURES_XSL) \
	$(UIAQA_LOGO_PNG)  

LINUX_PKGCONFIG = \
	$(TESTS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(PROGRAMFILES_RESOURCES) $(LINUX_PKGCONFIG) 

FILES = \
	Properties/AssemblyInfo.cs \
	WinForms/DockPatternTests.cs \
	WinForms/KeePassTests.cs \
	WinForms/WindowPatternTests.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	GTK \
	Moonlight \
	tests.pc.in 

REFERENCES =  \
	nunit.framework \
	System \
	System.Core \
	System.Windows.Forms \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	UIAutomationClient \
	UIAutomationTypes

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(PROGRAMFILES_RESOURCES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

UIACLIENTTESTFRAMEWORK_DLL = $(BUILD_DIR)/UIAClientTestFramework.dll
CONFIG_XML = $(BUILD_DIR)/Config.xml
NOVELL_LOGO_PNG = $(BUILD_DIR)/Resources/novell_logo.png
PROCEDURES_CSS = $(BUILD_DIR)/Resources/procedures.css
PROCEDURES_XSL = $(BUILD_DIR)/Resources/procedures.xsl
UIAQA_LOGO_PNG = $(BUILD_DIR)/Resources/uiaqa_logo.png
TESTS_PC = $(BUILD_DIR)/tests.pc

$(eval $(call emit-deploy-target,UIACLIENTTESTFRAMEWORK_DLL))
$(eval $(call emit-deploy-target,CONFIG_XML))
$(eval $(call emit-deploy-target,NOVELL_LOGO_PNG))
$(eval $(call emit-deploy-target,PROCEDURES_CSS))
$(eval $(call emit-deploy-target,PROCEDURES_XSL))
$(eval $(call emit-deploy-target,UIAQA_LOGO_PNG))
$(eval $(call emit-deploy-wrapper,TESTS_PC,tests.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
