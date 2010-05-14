include Makefile.include

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
	ToggleButton \

SWC_DIRS = \
	Calendar \
	DataGrid \
	DatePicker \
	TabControl

# Building tests depending on SWC/SWCD dlls when those exist.
all:
	for i in $(SUBDIRS); do (cd $$i; make); done
	@if test -e $(GLOBAL_SWC); then for i in $(SWC_DIRS); do (cd $$i; make); done fi

clean:
	for i in $(SUBDIRS); do (cd $$i; make clean); done
	for i in $(SWC_DIRS); do (cd $$i; make clean); done
