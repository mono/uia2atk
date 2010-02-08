
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE"
ASSEMBLY = bin/Debug/MonoTests.Mono.UIAutomation.UIAClientAPI.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Mono.UIAutomation.TestFramework/bin/Debug/Mono.UIAutomation.TestFramework.dll
BUILD_DIR = bin/Debug/

NOVELL_LOGO_PNG_SOURCE=Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=Resources/procedures.css
PROCEDURES_XSL_SOURCE=Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=Resources/uiaqa_logo.png
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_SOURCE=../Mono.UIAutomation.TestFramework/bin/Debug/Mono.UIAutomation.TestFramework.dll
CONFIG_XML_SOURCE=../Mono.UIAutomation.TestFramework/Config.xml
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB_SOURCE=../Mono.UIAutomation.TestFramework/bin/Debug/Mono.UIAutomation.TestFramework.dll.mdb
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB=$(BUILD_DIR)/Mono.UIAutomation.TestFramework.dll.mdb
MONOTESTS_MONO_UIAUTOMATION_UIACLIENTAPI_DLL_MDB_SOURCE=bin/Debug/MonoTests.Mono.UIAutomation.UIAClientAPI.dll.mdb
MONOTESTS_MONO_UIAUTOMATION_UIACLIENTAPI_DLL_MDB=$(BUILD_DIR)/MonoTests.Mono.UIAutomation.UIAClientAPI.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE"
ASSEMBLY = bin/Release/MonoTests.Mono.UIAutomation.UIAClientAPI.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../Mono.UIAutomation.TestFramework/bin/Release/Mono.UIAutomation.TestFramework.dll
BUILD_DIR = bin/Release/

NOVELL_LOGO_PNG_SOURCE=Resources/novell_logo.png
PROCEDURES_CSS_SOURCE=Resources/procedures.css
PROCEDURES_XSL_SOURCE=Resources/procedures.xsl
UIAQA_LOGO_PNG_SOURCE=Resources/uiaqa_logo.png
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_SOURCE=../Mono.UIAutomation.TestFramework/bin/Release/Mono.UIAutomation.TestFramework.dll
CONFIG_XML_SOURCE=../Mono.UIAutomation.TestFramework/Config.xml
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB=
MONOTESTS_MONO_UIAUTOMATION_UIACLIENTAPI_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES_RESOURCES = \
	$(NOVELL_LOGO_PNG) \
	$(PROCEDURES_CSS) \
	$(PROCEDURES_XSL) \
	$(UIAQA_LOGO_PNG)  

PROGRAMFILES = \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_DLL) \
	$(CONFIG_XML) \
	$(MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB) \
	$(MONOTESTS_MONO_UIAUTOMATION_UIACLIENTAPI_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(TESTS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES_RESOURCES) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	Moonlight/DiggSearchTests.cs \
	Moonlight/PuzzleGameTests.cs \
	Moonlight/SL2WithPrismTests.cs \
	Winforms/DockPatternTests.cs \
	Winforms/KeePassTests.cs \
	Winforms/WindowPatternTests.cs \
	AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	GTK \
	Moonlight \
	Resources/novell_logo.png \
	Resources/procedures.css \
	Resources/procedures.xsl \
	Resources/uiaqa_logo.png \
	tests.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Windows.Forms \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	$(MONO_UIA_LIBS) \
	$(NUNIT_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES_RESOURCES) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

NOVELL_LOGO_PNG = $(BUILD_DIR)/Resources/novell_logo.png
PROCEDURES_CSS = $(BUILD_DIR)/Resources/procedures.css
PROCEDURES_XSL = $(BUILD_DIR)/Resources/procedures.xsl
UIAQA_LOGO_PNG = $(BUILD_DIR)/Resources/uiaqa_logo.png
MONO_UIAUTOMATION_TESTFRAMEWORK_DLL = $(BUILD_DIR)/Mono.UIAutomation.TestFramework.dll
CONFIG_XML = $(BUILD_DIR)/Config.xml
TESTS_PC = $(BUILD_DIR)/tests.pc

$(eval $(call emit-deploy-target,NOVELL_LOGO_PNG))
$(eval $(call emit-deploy-target,PROCEDURES_CSS))
$(eval $(call emit-deploy-target,PROCEDURES_XSL))
$(eval $(call emit-deploy-target,UIAQA_LOGO_PNG))
$(eval $(call emit-deploy-target,MONO_UIAUTOMATION_TESTFRAMEWORK_DLL))
$(eval $(call emit-deploy-target,CONFIG_XML))
$(eval $(call emit-deploy-target,MONO_UIAUTOMATION_TESTFRAMEWORK_DLL_MDB))
$(eval $(call emit-deploy-wrapper,TESTS_PC,tests.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
