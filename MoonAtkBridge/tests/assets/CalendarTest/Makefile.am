XAP = CalendarTest.xap

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(XAP): $(FILES)
	cp -f `pkg-config --variable=libdir mono`/moonlight/2.0-redist/System.Windows.Controls.dll .
	$(MXAP) -r:System.Windows.Controls.dll --builddirhack=$(MOON_PATH)

clean:
	rm -f System.Windows.Controls.dll
	$(MXAP) --clean

