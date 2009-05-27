PATCH_STAMP = patch.stamp

GTK_SHARP_PATCHES = \
	patches/001_gtk-sharp_target-smcs.diff

all: gtk-sharp patch

gtk-sharp:
	svn export http://anonsvn.mono-project.com/source/trunk/gtk-sharp

patch: gtk-sharp
	if [ -f $(PATCH_STAMP) ]; then \
		for p in $(GTK_SHARP_PATCHES); do \
			if [ $(PATCH_STAMP) -ot $$p ]; then \
				rm -f $(PATCH_STAMP); \
				$(MAKE) $(AM_MAKEFLAGS) $(PATCH_STAMP); \
				break; \
			fi; \
		done; \
	else \
		$(MAKE) $(AM_MAKEFLAGS) $(PATCH_STAMP); \
	fi

$(PATCH_STAMP): $(GTK_SHARP_PATCHES)
	rm -f patch.tmp
	touch patch.tmp
	$(foreach p,$^, patch -p0 -N -d gtk-sharp < $p)
	mv -f patch.tmp $@

clean:
	rm -f $(PATCH_STAMP)
	rm -rf gtk-sharp/
