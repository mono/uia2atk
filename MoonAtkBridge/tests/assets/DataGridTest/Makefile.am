XAP = DataGridTest.xap

SWCD = System.Windows.Controls.Data.dll
GLOBAL_SWCD = ${MOON_LIBDIR}/${SWCD}

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(SWCD): $(GLOBAL_SWCD)
	cp $(GLOBAL_SWCD) $(SWCD)

$(XAP): $(SWCD) $(FILES)
	$(MXAP) -ares:$(SWCD)

clean:
	rm -f $(SWCD)
	$(MXAP) --clean

