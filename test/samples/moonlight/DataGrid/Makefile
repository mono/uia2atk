all:
	cp `which mono | sed -e 's/bin\/mono//'`/lib/mono/moonlight/System.Windows.Controls.dll .
	cp `which mono | sed -e 's/bin\/mono//'`/lib/mono/moonlight/System.Windows.Controls.Data.dll .
	mxap --application-name=DataGridSample

clean:
	rm -f *.dll
	mxap --application-name=DateGridSample --clean


