#
# spec file for package UIAutomationWinforms
#

Name:           uiautomationwinforms
Version:	1.0
Release:	1
License:        MIT/X11
Group:          System/Libraries
URL:		http://www.mono-project.com/Accessibility
Source0:        %{name}-%{version}.tar.bz2
BuildRoot:	%{_tmppath}/%{name}-%{version}-build
Requires:	mono-core mono-data gtk-sharp2 
Requires:	mono-uia mono-winfxcore uiaatkbridge glib-sharp2
BuildRequires:	mono-devel mono-data gtk-sharp2 glib-sharp2 
BuildRequires:	mono-nunit mono-uia mono-winfxcore uiaatkbridge intltool

Summary:        Implementation of UIA providers

%description
Implementation of UIA providers for Mono's Winforms controls

%prep
%setup -q

%build
%configure --disable-tests
make

%install
make DESTDIR=%{buildroot} install
%find_lang UIAutomationWinforms

%clean
rm -rf %{buildroot}

%files -f UIAutomationWinforms.lang
%defattr(-,root,root)
%doc COPYING README NEWS 
%dir %_libdir/uiautomationwinforms
%_libdir/uiautomationwinforms/UIAutomationWinforms.dll*
%_prefix/lib/mono/gac/UIAutomationWinforms

%changelog
