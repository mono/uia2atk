thisdir = data
SUBDIRS = 

net20dir   = $(prefix)/lib/mono/2.0
a11ydir    = $(prefix)/lib/mono/accessibility

SHELL = /bin/sh
MKINSTALLDIRS = $(SHELL) ../mkinstalldirs
INSTALL = $(SHELL) ../install-sh
INSTALL_D = $(INSTALL) -c -m 644

all: mono-uia.pc

DISTFILES = mono-uia.pc.in

REWRITE=
mono-uia.pc: mono-uia.pc.in Makefile
	sed -e "s,@net20dir@,$(net20dir),g" -e "s,0.9,$(MONO_VERSION),g" < mono-uia.pc.in > $@.tmp
	sed -e "s,@a11ydir@,$(a11ydir),g" -e "s,0.9,$(MONO_VERSION),g" < $@.tmp > $@.tmp2
	mv $@.tmp2 $@
	rm $@.tmp

install-data-local: mono-uia.pc
	$(MKINSTALLDIRS) $(DESTDIR)$(prefix)/lib/pkgconfig
	$(INSTALL_D) mono-uia.pc $(DESTDIR)$(prefix)/lib/pkgconfig

uninstall-local:
	rm -rf $(DESTDIR)$(prefix)/lib/pkgconfig/mono-uia.pc

clean-local:
	rm -rf mono-uia.pc

test-local:

dist-local: dist-default
