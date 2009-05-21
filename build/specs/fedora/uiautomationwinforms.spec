%define		debug_package %{nil}

#
# spec file for package UIAutomationWinforms
#

Name:           uiautomationwinforms
Version:	1.0
Release:	1
License:        MIT
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        http://ftp.novell.com/pub/mono/sources/uiautomationwinforms/%{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-%{release}-root-%(%{__id_u} -n)
Requires:	mono-core >= 2.4 mono-data >= 2.4 gtk-sharp2 >= 2.12.8 
Requires:	mono-uia >= 1.0 mono-winfxcore >= 1.0
Requires:	uiaatkbridge >= 1.0 glib-sharp2 >= 2.12.8
BuildRequires:	mono-devel >= 2.4 mono-data >= 2.4
BuildRequires:  gtk-sharp2 >= 2.12.8 glib-sharp2 >= 2.12.8 
BuildRequires:	mono-nunit >= 2.4 mono-uia >= 1.0 mono-winfxcore >= 1.0
BuildRequires:	uiaatkbridge >= 1.0 intltool

Summary:        Implementation of UIA providers

%description
Implementation of UIA providers for Mono's Winforms controls

%prep
%setup -q

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
* Thu Apr 30 2009 Stephen Shaw <sshaw@decriptor.com> - 1.0-1
- Initial RPM
