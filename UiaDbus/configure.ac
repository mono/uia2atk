AC_PREREQ([2.54])
AC_INIT([UiaDbus], [2.0.2])
AM_INIT_AUTOMAKE([foreign])

# pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

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


AC_ARG_ENABLE(external-ndesk-dbus,
	AC_HELP_STRING([--enable-external-ndesk-dbus],
		[Build referencing the system-installed ndesk-dbus instead of the internal version [default=NO]]),
		enable_external_ndesk_dbus=yes, enable_external_ndesk_dbus=no)
PKG_CHECK_MODULES(NDESK_DBUS, ndesk-dbus-1.0 >= 0.6.0, has_ndesk_dbus=yes, has_ndesk_dbus=no)
if test "x$has_ndesk_dbus" = "xno"; then
	AC_MSG_WARN([Could not find ndesk-dbus-1.0 >= 0.6.0; will build with internal version.])
	enable_external_ndesk_dbus=no
fi
if test "x$enable_external_ndesk_dbus" = "xno"; then
	NDESK_DBUS_LIBS=System.Xml
	AC_SUBST(NDESK_DBUS_LIBS)
fi
AM_CONDITIONAL(ENABLE_EXTERNAL_NDESK_DBUS, test "x$enable_external_ndesk_dbus" = "xyes")


# package checks, common for all configs
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0 >= 2.12.8])
PKG_CHECK_MODULES([MONO_UIA], [mono-uia >= 2.0.2])

TOPDIR=$PWD
AC_SUBST(TOPDIR)

AC_CONFIG_FILES([
Makefile
data/Makefile
data/mono-uia-dbus.pc.in
UiaDbus/Makefile
UiaDbusBridge/Makefile
UiaDbusBridgeTests/Makefile
UiaDbusBridgeTests/dbusbridgetest.sh
UiaDbusSource/Makefile
])

AC_OUTPUT
