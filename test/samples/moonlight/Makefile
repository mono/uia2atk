SUBDIRS = \
	Button \
	#Calendar
	CheckBox \
	ComboBox \
	#DataGrid
	#DatePicker
	GridSplitter \
	HyperlinkButton \
	Image \
	ListBox \
	MediaElement \
	MultiScaleImage \
	PasswordBox \
	ProgressBar \
	RadioButton \
	RepeateButton \
	ScrollBar \
	ScrollViewer \
	Slider \
	#TabControl
	TextBlock \
	TextBox \
	Thumb \
	ToggleButton

all:
	for i in $(SUBDIRS); do (cd $$i; make); done

clean:
	for i in $(SUBDIRS); do (cd $$i; make clean); done
