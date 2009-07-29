
EXTRA_DIST =  

a11ydir    = $(prefix)/lib/mono/accessibility

SHELL = /bin/sh
MKINSTALLDIRS = $(SHELL) $(top_srcdir)/mkinstalldirs
INSTALL = $(SHELL) $(top_srcdir)/install-sh
INSTALL_D = $(INSTALL) -c -m 644

all: mono-uia-dbus.pc

REWRITE=
mono-uia-dbus.pc: mono-uia-dbus.pc.in Makefile
	sed -e "s,@a11ydir@,$(a11ydir),g" < mono-uia-dbus.pc.in > $@

install-data-local: mono-uia-dbus.pc
	$(MKINSTALLDIRS) $(DESTDIR)$(libdir)/pkgconfig
	$(INSTALL_D) mono-uia-dbus.pc $(DESTDIR)$(libdir)/pkgconfig

uninstall-local:
	rm -rf $(DESTDIR)$(libdir)/pkgconfig/mono-uia-dbus.pc

clean-local:
	rm -rf mono-uia-dbus.pc

test-local:

dist-local: dist-default

FILES =  \
	mono-uia-dbus.pc.in
