
BuildRequires:	subversion

Name:           moonlight
License:        LGPL v2.0 only
Group:          Productivity/Multimedia/Other
Summary:        Novell Moonlight
Url:            http://go-mono.com/moonlight/
Version:        1.0.1
Release:        1
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildRequires:  gtk-sharp2 mono-devel monodoc-core rsvg2-sharp
BuildRequires:  alsa-devel gcc-c++ gtk2-devel mono-nunit
BuildRequires:  libpulse-devel bc libGraphicsMagick++2 libMagick++-devel libGraphicsMagick++-devel
BuildRequires:  libexpat-devel mozilla-xulrunner190-devel

%description
Moonlight is an open source implementation of Microsoft Silverlight for
Unix systems.



%post

%postun

%files
%defattr(-, root, root)
%doc DUMMY

%prep
svn co svn://151.155.5.148/source/trunk/mono mono
svn co svn://151.155.5.148/source/trunk/mcs mcs
svn co svn://151.155.5.148/source/trunk/moon moon


%build
%{?env_options}
%{?configure_options}
cd moon
sh autogen.sh

%install
%{?env_options}
touch DUMMY
cd moon
make dist-bzip2

%clean
rm -rf ${RPM_BUILD_ROOT}

%changelog
