XAP = ImageTest.xap

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(XAP): $(FILES)
	$(MXAP) -res=Koala.jpg

clean:
	$(MXAP) --clean
