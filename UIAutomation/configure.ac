dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([mono-uia], [1.8.90])
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

AC_ARG_ENABLE(tests,
	AC_HELP_STRING([--disable-tests],
		[Do not build unit tests [default=no]]),
		enable_tests=no, enable_tests=yes)

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

NUNIT_PATH=

PKG_CHECK_MODULES(NUNIT, mono-nunit >= 2.4, nunit_247=yes, nunit_247=no)
if test "x$nunit_247" = "xno"; then
	AC_MSG_WARN([Could not find nunit >= 2.4.7; unit tests will not be available.])
	enable_tests=no
else
	NUNIT_PATH=`pkg-config --variable=libdir mono-nunit`/mono/2.0/
fi

AC_SUBST(NUNIT_PATH)

AM_CONDITIONAL(ENABLE_TESTS, test "x$enable_tests" = "xyes")

dnl package checks, common for all configs

PKG_CHECK_MODULES([MONO], [mono >= 2.0])
if pkg-config --atleast-version=2.5 mono; then
	SHAMROCK_CHECK_MONO_2_0_GAC_ASSEMBLIES([
		WindowsBase
	])
	WINDOWSBASE_LIBS=-r:WindowsBase
	AC_SUBST(WINDOWSBASE_LIBS)
	has_mono_2_5=yes
fi
AM_CONDITIONAL(HAS_MONO_2_5, test "x$has_mono_2_5" = "xyes")

dnl for test
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0 >= 2.12.8])

AC_CONFIG_FILES([
WindowsBase/Makefile
WindowsBase/AssemblyInfo.cs
UIAutomationTypes/Makefile
UIAutomationTypes/AssemblyInfo.cs
UIAutomationBridge/Makefile
UIAutomationBridge/AssemblyInfo.cs
UIAutomationSource/Makefile
UIAutomationSource/AssemblyInfo.cs
UIAutomationClient/Makefile
UIAutomationClientTests/Makefile
UIAutomationClientTests/GtkForm/Makefile
UIAutomationClientTests/AtspiUiaClientTests/Makefile
UIAutomationClientTests/AtspiUiaClientTests/atspiuiaclienttest.sh
UIAutomationClientTests/SampleForm/Makefile
UIAutomationClientTests/UIAutomationClientTests/Makefile
UIAutomationClientTests/UIAutomationClientTests/swfclienttest.sh
UIAutomationClient/AssemblyInfo.cs
UIAutomationProvider/Makefile
UIAutomationProvider/AssemblyInfo.cs
data/Makefile
data/mono-uia.pc.in
Makefile
build/common/Consts.cs
])

AC_OUTPUT

