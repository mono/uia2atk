dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([UIAutomation], [0.9])
AM_INIT_AUTOMAKE([foreign 1.9 tar-ustar])

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_PROG_INSTALL

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

AC_PATH_PROG(SN, sn, no)
if test "x$SN" = "xno" ; then
	AC_MSG_ERROR([No sn tool found])
fi


#AC_ARG_ENABLE(tests,
#	AC_HELP_STRING([--disable-tests],
#		[Do not build unit tests [default=no]]),
#		enable_tests=no, enable_tests=yes)
#
#AC_ARG_ENABLE(debug,
#	AC_HELP_STRING([--enable-debug],
#		[Use 'DEBUG' Configuration [default=YES]]),
#		enable_debug=yes, enable_debug=no)


NUNIT_PATH=

PKG_CHECK_MODULES(NUNIT, nunit >= 2.4.7, nunit_247=yes, nunit_247=no)
if test "x$nunit_247" = "xno"; then
	AC_MSG_WARN([Could not find nunit >= 2.4.7; unit tests will not be available.])
	enable_tests=no
else
	NUNIT_PATH=`pkg-config --variable=assemblies_dir nunit`
fi

AC_SUBST(NUNIT_PATH)

AM_CONDITIONAL(ENABLE_TESTS, test "x$enable_tests" = "xyes")

dnl package checks, common for all configs

PKG_CHECK_MODULES([MONO], [mono >= 2.0])

AC_CONFIG_FILES([
WindowsBase/Makefile
UIAutomationTypes/Makefile
UIAutomationBridge/Makefile
UIAutomationClient/Makefile
UIAutomationProvider/Makefile
data/Makefile
data/mono-uia.pc.in
Makefile
build/common/Consts.cs
])

AC_OUTPUT

