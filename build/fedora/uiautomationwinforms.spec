%define		debug_package %{nil}

#
# spec file for package UIAutomationWinforms
#

Name:           uiautomationwinforms
Version:        1.8.93
Release:        1
License:        MIT
Group:          System/Libraries
URL:            http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/uiautomationwinforms/%{name}-%{version}.tar.bz2
Patch0:         uiautomationwinforms-libdir.patch
BuildRoot:      %{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Requires:       gtk-sharp2 >= 2.12.8
Requires:       mono-core >= 2.6
Requires:       mono-data >= 2.6
Requires:       mono-uia >= 1.8.93
Requires:       uiaatkbridge >= 1.8.93
BuildRequires:  gtk-sharp2-devel >= 2.12.8
BuildRequires:	mono-devel >= 2.6
BuildRequires:  mono-data >= 2.6
BuildRequires:	mono-nunit >= 2.6
BuildRequires:  mono-uia >= 1.8.93
BuildRequires:  mono-uia-devel >= 1.8.93
BuildRequires:  intltool

Summary:        Implementation of UIA providers

%description
Implementation of UIA providers for Mono's Winforms controls

%prep
%setup -q
%patch0 -p1

%build
%configure --disable-tests
make %{?_smp_mflags}

%install
rm -rf %{buildroot}
make DESTDIR=%{buildroot} install
%find_lang UIAutomationWinforms

%clean
rm -rf %{buildroot}

%files -f UIAutomationWinforms.lang
%defattr(-,root,root,-)
%doc COPYING README NEWS 
%dir %_libdir/uiautomationwinforms
%_libdir/uiautomationwinforms/UIAutomationWinforms.dll*
%_libdir/mono/gac/UIAutomationWinforms

%changelog
* Mon Nov 30 2009 Stephen Shaw <sshaw@decriptor.com> - 1.8.90-1
- Updates for the 2.0 release
* Thu Apr 30 2009 Stephen Shaw <sshaw@decriptor.com> - 1.0-1
- Initial RPM
