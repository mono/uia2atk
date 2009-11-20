XAP = FakeBridgeTest.xap

FAKEBRIDGE = MoonAtkBridge.dll

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(FAKEBRIDGE):
	cd ../../.. && make fake
	cp ../../../bin/fake/MoonAtkBridge.dll .

$(XAP): $(FILES) $(FAKEBRIDGE)
	$(MXAP)

clean:
	$(MXAP) --clean
