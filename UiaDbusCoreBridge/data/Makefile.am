
EXTRA_DIST =  

a11ydir    = $(prefix)/lib/mono/accessibility

SHELL = /bin/sh
MKINSTALLDIRS = $(SHELL) $(top_srcdir)/mkinstalldirs
INSTALL = $(SHELL) $(top_srcdir)/install-sh
INSTALL_D = $(INSTALL) -c -m 644

all: mono-uia-dbuscore.pc

REWRITE=
mono-uia-dbuscore.pc: mono-uia-dbuscore.pc.in Makefile
	sed -e "s,@a11ydir@,$(a11ydir),g" < mono-uia-dbuscore.pc.in > $@

install-data-local: mono-uia-dbuscore.pc
	$(MKINSTALLDIRS) $(DESTDIR)$(libdir)/pkgconfig
	$(INSTALL_D) mono-uia-dbuscore.pc $(DESTDIR)$(libdir)/pkgconfig

uninstall-local:
	rm -rf $(DESTDIR)$(libdir)/pkgconfig/mono-uia-dbuscore.pc

clean-local:
	rm -rf mono-uia-dbuscore.pc

test-local:

dist-local: dist-default

FILES =  \
	mono-uia-dbuscore.pc.in
