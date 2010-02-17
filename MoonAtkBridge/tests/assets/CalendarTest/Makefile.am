XAP = CalendarTest.xap

SWC = System.Windows.Controls.dll
GLOBAL_SWC = ${MOON_LIBDIR}/${SWC}

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(SWC): $(GLOBAL_SWC)
	cp $(GLOBAL_SWC) $(SWC)

$(XAP): $(SWC) $(FILES)
	$(MXAP) -ares:$(SWC)

clean:
	rm -f $(SWC)
	$(MXAP) --clean

