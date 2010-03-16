bridgegluedir = $(libdir)/uiaatkbridge
bridgeglue_LTLIBRARIES = libbridge-glue.la

libbridge_glue_la_CFLAGS = -Wall -DGTK_MODULES_DIR=\"$(GTK_MODULES_DIR)\"

libbridge_glue_la_LDFLAGS = -module -avoid-version -no-undefined 
libbridge_glue_la_LIBADD = $(ATK_LIBS)

libbridge_glue_la_SOURCES = \
	main.c \
	main.h

INCLUDES = $(ATK_CFLAGS) $(GOBJECT_20_CFLAGS) $(GMODULE_20_CFLAGS) $(GLIB_CFLAGS)

EXTRA = Changelog
