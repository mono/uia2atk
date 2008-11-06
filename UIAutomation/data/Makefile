thisdir = data
SUBDIRS = 
include ../build/rules.make

ifeq ($(PLATFORM), win32)
	olivedir := $(shell cygpath -m '$(prefix)/lib/mono/3.0')
	olive35dir := $(shell cygpath -m '$(prefix)/lib/mono/3.5')
else
	olivedir   = $(prefix)/lib/mono/3.0
	olive35dir = $(prefix)/lib/mono/3.5
	silverdir  = $(prefix)/lib/mono/2.1
	net20dir   = $(prefix)/lib/mono/2.0
	a11ydir    = $(prefix)/lib/mono/accessibility
endif

all-local: mono-uia.pc

DISTFILES = mono-uia.pc.in

REWRITE=
mono-uia.pc: mono-uia.pc.in Makefile
	sed -e "s,@net20dir@,$(net20dir),g" -e "s,@VERSION@,$(MONO_VERSION),g" < mono-uia.pc.in > $@.tmp
	sed -e "s,@a11ydir@,$(a11ydir),g" -e "s,@VERSION@,$(MONO_VERSION),g" < $@.tmp > $@.tmp2
	mv $@.tmp2 $@
	rm $@.tmp

install-local: mono-uia.pc
	$(MKINSTALLDIRS) $(DESTDIR)$(prefix)/lib/pkgconfig
	$(INSTALL_DATA) mono-uia.pc $(DESTDIR)$(prefix)/lib/pkgconfig

clean-local:
	rm -rf mono-uia.pc

test-local:

dist-local: dist-default
