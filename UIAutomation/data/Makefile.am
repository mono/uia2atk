
EXTRA_DIST =  

a11ydir    = $(prefix)/lib/mono/accessibility

windowsbase = WindowsBase

SHELL = /bin/sh
MKINSTALLDIRS = $(SHELL) $(top_srcdir)/mkinstalldirs
INSTALL = $(SHELL) $(top_srcdir)/install-sh
INSTALL_D = $(INSTALL) -c -m 644

all: mono-uia.pc

REWRITE=
mono-uia.pc: mono-uia.pc.in Makefile
	sed -e "s,@windowsbase@,$(windowsbase),g" < mono-uia.pc.in > $@.tmp
	sed -e "s,@a11ydir@,$(a11ydir),g" < $@.tmp > $@.tmp2
	mv $@.tmp2 $@
	rm $@.tmp

install-data-local: mono-uia.pc
	$(MKINSTALLDIRS) $(DESTDIR)$(libdir)/pkgconfig
	$(INSTALL_D) mono-uia.pc $(DESTDIR)$(libdir)/pkgconfig

uninstall-local:
	rm -rf $(DESTDIR)$(libdir)/pkgconfig/mono-uia.pc

clean-local:
	rm -rf mono-uia.pc

test-local:

dist-local: dist-default

FILES =  \
	mono-uia.pc.in
