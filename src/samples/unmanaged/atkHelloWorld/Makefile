CFLAGS:=-g $(shell pkg-config --cflags atk gmodule-2.0 gobject-2.0)
LDFLAGS:=$(shell pkg-config --libs atk gmodule-2.0 gobject-2.0) -lgthread-2.0

hello: hello.o hellochild.o hellotoplevel.o helloutil.o mytk.o mytkwidget.o
