SWC = System.Windows.Controls

XAP = TableTest.xap

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

$(SWC).dll:
	cp $(MOON_PATH)/class/lib/moonlight/$(SWC).dll .

all: $(XAP)

$(XAP): $(FILES) $(SWC).dll
	$(MXAP)

clean:
	$(MXAP) --clean
