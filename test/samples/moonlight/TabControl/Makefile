all:
	cp `which mono | sed -e 's/bin\/mono//'`/lib/mono/moonlight/System.Windows.Controls.dll .
	mxap --application-name=TabControlSample

clean:
	rm -f *.dll
	mxap --application-name=TabControlSample --clean
