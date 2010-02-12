AC_PREREQ([2.54])
AC_INIT([MoonAtkBridge], [1.8.94])
AM_INIT_AUTOMAKE([foreign])

# pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
	AC_MSG_ERROR([You need to install pkg-config])
fi

m4_include(expansions.m4)
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

AC_PATH_PROG(SMCS, smcs, no)
if test "x$SMCS" = "xno"; then
	AC_MSG_ERROR([smcs Not found, please install moonlight])
fi

AC_PATH_PROG(MXAP, mxap, no)
if test "x$MXAP" = "xno"; then
	AC_MSG_ERROR([mxap Not found, please install moonlight])
fi

AC_PATH_PROG(CC, gcc, no)
if test "x$CC" = "xno"; then
	AC_MSG_ERROR([gcc Not found])
fi

AC_PATH_PROG(GACUTIL, gacutil, no)
if test "x$GACUTIL" = "xno" ; then
	AC_MSG_ERROR([No gacutil tool found])
fi

AC_PATH_PROG(WGET, wget, no)
if test "x$WGET" = "xno" ; then
	AC_MSG_ERROR([No wget tool found])
fi

AC_PATH_PROG(DIFF, diff, no)
if test "x$DIFF" = "xno" ; then
	AC_MSG_ERROR([No diff tool found])
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

dnl package checks, common for all configs
PKG_CHECK_MODULES([MONO], [mono >= 2.5])

AC_ARG_VAR(MOON_PATH, "Path to built checkout of moon")
if test "x$MOON_PATH" = "x"; then
	MOON_PATH=../../moon
fi
# Convert relative path to absolute path
MOON_PATH=`(cd $MOON_PATH && pwd)`

PKG_CHECK_MODULES(CECIL, cecil)

LINKING_PATH=$MOON_PATH/class/lib/tuner

#uncomment this when we can do a proper check instead of using the monolinker.exe in lib/tuner
#AC_PATH_PROG(MONOLINKER, monolinker, no)
#if test "x$MONO" = "xno"; then
#	AC_MSG_ERROR([monolinker Not found])
#fi

if [ ! test -f "$LINKING_PATH/Mono.Tuner.dll" ]
then
	AC_MSG_ERROR([You need a built checkout of moon either at the same level of the uia2atk folder, or at a location specified by MOON_PATH])
fi
AC_SUBST(LINKING_PATH)

AC_PATH_PROG(ZIP, zip, no)
if test "x$ZIP" = "xno" ; then
	AC_MSG_ERROR([No zip tool found])
fi

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

TOPDIR=$PWD
AC_SUBST(TOPDIR)

AC_SUBST(VERSION)

AC_CONFIG_FILES([
Makefile
resources/install.rdf
tests/Makefile
tests/assets/Makefile
tests/assets/AdapterEventsTest/Makefile
tests/assets/AddRemoveAdapterTest/Makefile
tests/assets/ApplicationNullTest/Makefile
tests/assets/ButtonTest/Makefile
tests/assets/CalendarTest/Makefile
tests/assets/CheckBoxTest/Makefile
tests/assets/ComboBoxTest/Makefile
tests/assets/ExpandCollapseEventsTest/Makefile
tests/assets/FakeBridgeTest/Makefile
tests/assets/HyperlinkButtonTest/Makefile
tests/assets/ImageTest/Makefile
tests/assets/ListBoxTest/Makefile
tests/assets/MediaElementTest/Makefile
tests/assets/PasswordBoxTest/Makefile
tests/assets/ProgressBarTest/Makefile
tests/assets/RadioButtonTest/Makefile
tests/assets/RangeValueTest/Makefile
tests/assets/ReflectionTest/Makefile
tests/assets/RepeatButtonTest/Makefile
tests/assets/ScrollBarTest/Makefile
tests/assets/ScrollViewerTest/Makefile
tests/assets/SelectionTest/Makefile
tests/assets/SliderTest/Makefile
tests/assets/TableTest/Makefile
tests/assets/TextBlockTest/Makefile
tests/assets/ThumbTest/Makefile
tests/assets/ToggleButtonTest/Makefile
tests/assets/ToggleEventsTest/Makefile
tests/assets/ValueEventsTest/Makefile
tests/assets/ValueTest/Makefile
])

AC_OUTPUT
