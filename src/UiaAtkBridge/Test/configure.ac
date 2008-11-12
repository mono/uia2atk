AC_PREREQ([2.54])
AC_INIT([Test], [109393])
AM_INIT_AUTOMAKE([foreign])

# pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_CHECK_TOOL(CC, gcc, gcc)
AC_PROG_CC
AC_PROG_INSTALL
AM_PROG_LIBTOOL

AC_PATH_PROG(MONO, mono, no)
if test "x$MONO" = "xno"; then
        AC_MSG_ERROR([mono Not found])
fi
AC_SUBST(MONO)

AC_PATH_PROG(GMCS, gmcs, no)
if test "x$GMCS" = "xno"; then
        AC_MSG_ERROR([gmcs Not found])
fi

AC_ARG_ENABLE(tests,
	AC_HELP_STRING([--disable-tests],
		[Do not build unit tests [default=no]]),
		enable_tests=no, enable_tests=yes)

NUNIT_PATH=

PKG_CHECK_MODULES(NUNIT, nunit >= 2.4.7, nunit_247=yes, nunit_247=no)
if test "x$nunit_247" = "xno"; then
	AC_MSG_WARN([Could not find nunit >= 2.4.7; unit tests will not be available.])
	enable_tests=no
else
	NUNIT_PATH=`pkg-config --variable=assemblies_dir nunit`
fi

AC_SUBST(NUNIT_PATH)

# package checks, common for all configs
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([MONO_UIA], [mono-uia])
#Bridge-glue checks
PKG_CHECK_MODULES(ATK, atk)
AC_SUBST(ATK_CFLAGS)
AC_SUBST(ATK_LIBS)

PKG_CHECK_MODULES([GOBJECT_20], [gobject-2.0])
AC_SUBST(GOBJECT_20_CFLAGS)
AC_SUBST(GOBJECT_20_LIBS)

PKG_CHECK_MODULES([GMODULE_20], [gmodule-2.0])
AC_SUBST(GMODULE_20_CFLAGS)
AC_SUBST(GMODULE_20_LIBS)

PKG_CHECK_MODULES([GLIB_20], [glib-2.0])
AC_SUBST(GLIB_CFLAGS)
AC_SUBST(GLIB_LIBS)

AC_SUBST(TOPDIR)

AC_CONFIG_FILES([
Makefile
AtkTest/Makefile
GailTest/Makefile
GailTestApp/Makefile
UiaAtkBridgeTest/Makefile
])

AC_OUTPUT
