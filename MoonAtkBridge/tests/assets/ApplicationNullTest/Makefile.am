# Notice this XAP name doesn't match the name defined in App.xaml
#
# Error output: "Message: Could not find the startup type ApplicationNullTest.App on the ApplicationNullTest"
XAP = ApplicationNull.xap

FILES = \
	App.xaml \
	App.xaml.cs \
	Page.xaml \
	Page.xaml.cs

all: $(XAP)

$(XAP): $(FILES)
	$(MXAP)

clean:
	$(MXAP) --clean
