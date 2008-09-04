#
# spec file for package at-spi (Version 1.9.0 /w dbus )
#
# Copyright (c) 2008 SUSE LINUX Products GmbH, Nuernberg, Germany.
# This file and all modifications and additions to the pristine
# package are under the same license as the package itself.
#
# Please submit bugfixes or comments via http://bugs.opensuse.org/
#

# norootforbuild


Name:           at-spi
BuildRequires:  fdupes gail-devel gnome-common gtk-doc intltool libidl-devel perl-XML-Parser python sgml-skel update-desktop-files
License:        GPL v2 or later; LGPL v2.1 or later
Group:          Development/Libraries/GNOME
AutoReqProv:    on
Version:        1.9.0
Release:        1
Requires:       %{name}-lang = %{version} 
Summary:        Assistive Technology Service Provider Interface - dbus
Source:         %{name}-%{version}.tar.bz2
Url:            http://www.gnome.org/
BuildRoot:      %{_tmppath}/%{name}-%{version}-build

%description
This library, based on ATK, is a general interface for applications to
make use of the accessibility toolkit.  This version is based on dbus.



Authors:
--------
    Bill Haneman <bill.haneman@sun.com>
    Marc Mulcahy <marc.mulchay@sun.com>
    Michael Meeks <micheal@ximian.com>
    Mark McLoughlin <mark@skynet.ie>
    Stephen Shaw <sshaw@decriptor.com>
    Ray Wany <rawang@novell.com>

%package devel
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Include Files and Libraries mandatory for Development.
Group:          Development/Libraries/GNOME
Requires:       %{name} = %{version} libbonobo-devel atk-devel gtk2-devel

%description devel
This package contains all necessary include files and libraries needed
to develop applications that require these.



%package doc
License:        GPL v2 or later; LGPL v2.1 or later
Summary:        Additional Package Documentation.
Group:          Development/Libraries/GNOME
Requires:       %{name} = %{version}

%description doc
This package contains optional documentation provided in addition to
this package's base documentation.



Authors:
--------
    Bill Haneman <bill.haneman@sun.com>
    Marc Mulcahy <marc.mulchay@sun.com>
    Michael Meeks <micheal@ximian.com>
    Mark McLoughlin <mark@skynet.ie>
    Stephen Shaw <sshaw@decriptor.com>
    Ray Wany <rawang@novell.com>

%lang_package
%prep

%setup -q

%build
%configure\
	--libexecdir=%{_prefix}/lib/at-spi
make %{?jobs:-j%jobs} referencetopdir=%{_docdir}/%{name}/reference

%install
make install referencetopdir=%{_docdir}/%{name}/reference DESTDIR=$RPM_BUILD_ROOT
cp AUTHORS COPYING ChangeLog NEWS README $RPM_BUILD_ROOT%{_docdir}/%{name}
%find_lang %{name}
rm $RPM_BUILD_ROOT%{_libdir}/*/*.*a $RPM_BUILD_ROOT%{_libdir}/*/*/*.*a
%fdupes $RPM_BUILD_ROOT

%clean
rm -rf $RPM_BUILD_ROOT

%post -p /sbin/ldconfig

%postun -p /sbin/ldconfig

%files 
%defattr (-, root, root)
%doc %dir %{_docdir}/%{name}
%doc %{_docdir}/%{name}/AUTHORS
%doc %{_docdir}/%{name}/COPYING
%doc %{_docdir}/%{name}/ChangeLog
%doc %{_docdir}/%{name}/NEWS
%doc %{_docdir}/%{name}/README
%{_libdir}/*.so.*
%{_libdir}/gtk-2.0/modules/*.so
#%{_prefix}/lib/at-spi
# FIXME: split these off into a separate -python package
%dir %{py_sitedir}/pyatspi/
%{py_sitedir}/pyatspi/*
%_prefix/share/at-spi/dbus/*

%files lang -f %{name}.lang

%files devel
%defattr (-, root, root)
%{_includedir}/at-spi-1.0
%{_includedir}/dbind-0.1
%{_libdir}/*.so
%{_libdir}/*.*a
%{_libdir}/pkgconfig/*.pc
#%{_datadir}/idl/at-spi-1.0

#%_prefix/libexec/at-spi-registryd

%files doc
%defattr (-, root, root)
%{_datadir}/gtk-doc/html/at-spi-cspi
%doc %{_docdir}/%{name}/reference

%changelog
* Mon Jun 09 2008 maw@suse.de
- Update to version 1.23.3:
  + Bugs fixed: bgo#532887, bgo#498668, bgo#431053, bgo#451553,
  bgo#520490, bgo#521667, and bgo#522356.
* Thu Apr 10 2008 maw@suse.de
- Update to version 1.22.1:
  + Bug fixed: bgo#520490
  + Updated translations.
* Fri Mar 14 2008 maw@suse.de
- Update to version 1.22.0:
  + Bug fixed: bgo#496232
  + Updated translations.
* Mon Mar 03 2008 maw@suse.de
- Update to version 1.21.92:
  + Bugs fixed: bgo#508147, bgo#517761, bgo#517250, and bgo#512702.
* Wed Jan 23 2008 maw@suse.de
- Update to version 1.21.5:
  + Bugs (bugzilla.gnome.org) fixed: #474480 and #503091.
* Tue Dec 18 2007 maw@suse.de
- Update to version 1.21.3:
  + Bugs (bugzilla.gnome.org) fixed: #446277, #493547, #326516,
  [#491805], #492469, and #490202.
* Sat Nov 24 2007 maw@suse.de
- Update to version 1.21.1:
  + Bugs (bugzilla.gnome.org) fixed: #490205, #490202, and #489273.
* Mon Sep 17 2007 sbrabec@suse.cz
- Updated to version 1.20.0:
  * bug fixes
* Thu Aug 30 2007 maw@suse.de
- Remove unnecessary autoreconf call.
* Mon Aug 06 2007 maw@suse.de
- Split out a -lang subpackage
- s#%%run_ldconfig#/sbin/ldconfig# in %%post and %%postun.
* Tue Jul 31 2007 maw@suse.de
- Update to version 1.19.5
- bugzilla.gnome.org bugs fixed: #446277, #450897, and #433802
- Use %%fdupes.
* Thu Jul 05 2007 maw@suse.de
- Update to version 1.19.3
- New python binding: pyatspi
- So buildrequire python
- Fixes for bugzilla.gnome.org #430938, #427836, #428007, #405774,
  [#407600], #329454, #439057, and #439436/
* Wed Apr 11 2007 maw@suse.de
- Update to version 1.18.1, which has a fix for bugzilla.gnome.org
  [#329454].
* Wed Mar 21 2007 maw@suse.de
- Update to version 1.18.0
- Bug fixes, including bugzilla.gnome.org 404584, 375319, 401299,
  412286, 329454.
* Wed Mar 21 2007 jhargadon@suse.de
- moved *.idl files to the devel package (#254444)
* Fri Feb 16 2007 maw@suse.de
- Update to version 1.17.0
- Remove at-spi-implicit-definition.patch
* Wed Jan 03 2007 sbrabec@suse.cz
- Spec file cleanup.
* Wed Dec 13 2006 maw@suse.de
- Move to /usr.
* Mon Oct 02 2006 jhargadon@suse.de
- update to version 1.7.12
- Modified parameter names in some IDL to avoid class name collisions
- Bugfixes: #353226, #356688 (Neo Liu), #350552 (Ginn Chen), #350958
* Wed Aug 30 2006 jhargadon@suse.de
- update to version 1.7.11
- Docs fixes
- Fixed getAttributes APIs
- Export Hyperlink interface for AtkHyperlinkImpl peers
- Aggregate Document interface
* Tue Aug 01 2006 gekker@suse.de
- Update to version 1.7.10
  * New method Selection::deselectChild.  RFE #326535.
* Fri Jul 21 2006 gekker@suse.de
- Update to version 1.7.9
- Remove upstreamed patch
  * I18n uses po/LINGUAS now.
  * Table enhancement: new method Table::getRowColumnExtentsAtIndex
  RFE #326536.  Assistance from Ariel Rios.
  * Text enhancement: Text::getAttributeRun, Text::getDefaultAttributeSet
  RFE #326540.  Assistance from Ariel Rios.
  * New interface, Document.  RFE #326520.
  * New roles, LINK, REDUNDANT_OBJECT, INPUT_METHOD_WINDOW, FORM.
  * Meaningful implementation and bridges for StreamableContent.
  RFE #326532.
  * Added STATE_VISITED, and relations DESCRIBED_BY, DESCRIPTION_FOR,
  and PARENT_WINDOW_OF.
  * Fixed dist to include Accessibility_Selector.idl (missing from dist though
  in cvs since 1.7.3).
  * Bugfixes to EventDetails event support, and fixed event emission for
  implementors of SPI_REMOTE_OBJECT.
  * Added idl/Accessibility_Selector.idl to the repository (missing in 1.7.0
  through 1.7.2, added in 1.7.3)
  * DOCS:
  All the IDL is now documented with doxygen-compatible docs.
  Just run 'doxygen oxyfile' in the IDL directory.  (This will
  be automated in a future version).
  * NEW API:
  idl:
  Accessibility::Accessible:getAttributes (name/value pair annotation
  for all objects)
  Accessibility::Accessible:getApplication (retrieves ref to host app)
  Accessibility::Component:getAlpha (get transparency/opacity value of comonent)
  Accessibility::Image:getLocale (get POSIX locale for image and
  imagedesc)
  Accessibility::Text:getAttributeValue (retrieve a single named
  attribute value)
  Accessibility::Relation:RELATION_PARENT_WINDOW_OF
  Accessibility::Role:ROLE_ENTRY, ROLE_CHART, ROLE_CAPTION,
  ROLE_DOCUMENT_FRAME, ROLE_HEADING, ROLE_PAGE, ROLE_SECTION (New roles
  for complex docs and forms.)
  Accessibility::State:STATE_REQUIRED, STATE_TRUNCATED, STATE_ANIMATED,
  STATE_INVALID_ENTRY, STATE_SUPPORTS_AUTOCOMPLETION,
  STATE_SELECTABLE_TEXT, STATE_IS_DEFAULT (New states, for complex forms)
  Accessibility::EventDetails (new, more detailed info marshalled with events)
  cspi:
  (AccessibleEvent_getSourceName):
  (AccessibleEvent_getSourceRole):
  (AccessibleEvent_getSourceApplication):
  (AccessibleEvent_getSourceDetails):
  New methods for interrogating/demarshalling event details i.e. source's
  accessible name, role, and host app.
  (Accessible_getAttributes): New, retrieve annotations/attributes
  on objects.
  (Accessible_getHostApplication): New, retrieve enclosing Application
  instance for an object.
  (AccessibleImage_getImageLocale): New, retrieve Locale info for an
  image.
  (AccessibleComponent_getAlpha): New, retrieve alpha value for
  an AccessibleComponent (see discussion in idl section above).
  * BUGFIXES:
  Some compiler fixes from Kjartan Marass.
  Some thread-related fixes from Michael Meeks.
  Don't allow non-preemptive listeners to pre-empt events! (Bill Haneman).
* Tue Mar 14 2006 gekker@suse.de
- fix a threading deadlock in event emission (#157561)
* Wed Jan 25 2006 mls@suse.de
- converted neededforbuild to BuildRequires
* Tue Sep 06 2005 gekker@suse.de
- Update to version 1.6.6
* Mon Sep 05 2005 gekker@suse.de
- Update to version 1.6.5 (GNOME 2.12)
- Add docbook-xml-packages to nfb
* Tue Jun 07 2005 gekker@suse.de
- Update to version 1.6.4.
* Wed Jun 01 2005 sbrabec@suse.cz
- Fixed devel requirements.
* Tue Mar 08 2005 gekker@suse.de
- Update to version 1.6.3 (GNOME 2.10)
* Fri Feb 04 2005 meissner@suse.de
- fixed sentinel warning.
* Thu Dec 16 2004 ro@suse.de
- fix compiler warning
* Wed Dec 15 2004 gekker@suse.de
- Update version to 1.6.2.
* Fri Nov 19 2004 ro@suse.de
- removed extra aclocal include directive
* Tue Oct 12 2004 sbrabec@suse.cz
- Removed openssl from build requirements.
* Tue Oct 12 2004 sbrabec@suse.cz
- Fixed libexecdir for bi-arch (#47050).
* Tue May 04 2004 sbrabec@suse.cz
- Moved *.pc to at-spi-devel.
* Wed Apr 21 2004 sbrabec@suse.cz
- Updated to version 1.4.2 (GNOME 2.6).
* Thu Feb 19 2004 sbrabec@suse.cz
- Updated to version 1.3.12.
* Mon Feb 09 2004 hhetter@suse.de
- updated to version 1.3.8 [GNOME2.4.2]
* Sat Jan 10 2004 adrian@suse.de
- add %%run_ldconfig and %%defattr
* Thu Oct 09 2003 sbrabec@suse.cz
- Updated to version 1.3.7 (GNOME 2.4).
* Fri Aug 15 2003 sbrabec@suse.cz
- Updated to version 1.2.1.
* Tue Jul 15 2003 sbrabec@suse.cz
- Prefix really changed to /opt/gnome.
* Tue Jul 15 2003 sbrabec@suse.cz
- GNOME prefix change to /opt/gnome.
* Mon Jul 14 2003 hhetter@suse.de
- %%_lib fixes
* Tue Jun 24 2003 sbrabec@suse.cz
- Fixed neededforbuild.
- Branched at-spi-devel and at-spi-doc.
- Prefix clash fixes - gtk-doc, orbit2 modules and gtk modules.
- Standard docs moved from yelp directory.
* Tue May 27 2003 ro@suse.de
- add bonobo-server to filelist
- remove unpackaged/moved files from buildroot
* Wed Feb 26 2003 sbrabec@suse.cz
- FHS fix.
* Fri Feb 07 2003 hhetter@suse.de
- updated to version 1.1.8 [GNOME 2.2.0]
* Tue Nov 12 2002 ro@suse.de
- changed neededforbuild <xf86 xdevel> to <x-devel-packages>
* Fri Sep 27 2002 ro@suse.de
- Added alsa alsa-devel to neededforbuild (esound)
* Thu Jun 06 2002 ro@suse.de
- build on lib64 platforms
* Wed Jun 05 2002 hhetter@suse.de
- initial SuSE Release
