AC_PREREQ([2.54])
AC_INIT([UiaAtkBridge], [1.8.91])
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

AC_PATH_PROG(CC, gcc, no)
if test "x$CC" = "xno"; then
        AC_MSG_ERROR([gcc Not found])
fi

AC_PATH_PROG(GACUTIL, gacutil, no)
if test "x$GACUTIL" = "xno" ; then
	AC_MSG_ERROR([No gacutil tool found])
fi

AC_ARG_ENABLE(tests,
	AC_HELP_STRING([--disable-tests],
		[Do not build unit tests [default=no]]),
		enable_tests=no, enable_tests=yes)

NUNIT_PATH=

PKG_CHECK_MODULES(NUNIT, mono-nunit >= 2.4, nunit_24=yes, nunit_24=no)
if test "x$nunit_24" = "xno"; then
	AC_MSG_WARN([Could not find mono-nunit >= 2.4; unit tests will not be available.])
	enable_tests=no
else
	NUNIT_PATH=`pkg-config --variable=libdir mono-nunit`/mono/2.0/
fi

AC_SUBST(NUNIT_PATH)

# Require Gtk+ devel package so that we can get access at the pc file to
# construct a path to the modules directory
PKG_CHECK_MODULES([GTK20], [gtk+-2.0 >= 2.12])

GTK_MODULES_DIR=`pkg-config --variable=libdir gtk+-2.0`/gtk-2.0/modules
AC_SUBST(GTK_MODULES_DIR)

AM_CONDITIONAL(ENABLE_TESTS, test "x$enable_tests" = "xyes")

AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'DEBUG' Configuration [default=YES]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release,
	AC_HELP_STRING([--enable-release],
		[Use 'RELEASE' Configuration [default=NO]]),
		enable_release=yes, enable_release=no)
AM_CONDITIONAL(ENABLE_RELEASE, test x$enable_release = xyes)
if test "x$enable_release" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_DEBUG, true)
	enable_debug=yes
fi


# package checks, common for all configs
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0 >= 2.12.8])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0 >= 2.12.8])

PKG_CHECK_MODULES(GTK_SHARP_2_12_10, gtk-sharp-2.0 >= 2.12.10, [last_gtk_sharp=yes], [last_gtk_sharp=no])
AM_CONDITIONAL(HAVE_OLD_ATK_SHARP, test x$last_gtk_sharp = xno)

PKG_CHECK_MODULES([MONO_UIA], [mono-uia >= 1.0])

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

TOPDIR=$PWD
AC_SUBST(TOPDIR)

AC_CONFIG_FILES([
Makefile
UiaAtkBridge/Makefile
bridge-glue/Makefile
Test/Makefile
Test/AtkTest/Makefile
Test/GailTestApp/Makefile
Test/GailTest/Makefile
Test/GailTest/gailtest.sh
Test/UiaAtkBridgeTest/Makefile
Test/UiaAtkBridgeTest/bridgetest.sh
])

AC_OUTPUT
