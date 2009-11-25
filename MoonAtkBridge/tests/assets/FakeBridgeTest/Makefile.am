XAP = FakeBridgeTest.xap

FAKEBRIDGE = MoonAtkBridge.dll

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(FAKEBRIDGE): ../../../bin/fake/MoonAtkBridge.dll
	cp ../../../bin/fake/MoonAtkBridge.dll .

../../../bin/fake/MoonAtkBridge.dll:
	$(MAKE) fake -C ../../..

$(XAP): $(FILES) $(FAKEBRIDGE)
	$(MXAP)

clean:
	$(MXAP) --clean
