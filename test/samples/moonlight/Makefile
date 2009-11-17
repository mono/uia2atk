SUBDIRS = \
	Button \
	CheckBox \
	ComboBox \
	GridSplitter \
	HyperlinkButton \
	Image \
	ListBox \
	MediaElement \
	MultiScaleImage \
	PasswordBox \
	ProgressBar \
	RadioButton \
	RepeatButton \
	ScrollBar \
	ScrollViewer \
	Slider \
	TextBlock \
	TextBox \
	Thumb \
	ToggleButton
	#Calendar
	#DataGrid
	#DatePicker
	#TabControl

all:
	for i in $(SUBDIRS); do (cd $$i; make); done

clean:
	for i in $(SUBDIRS); do (cd $$i; make clean); done
