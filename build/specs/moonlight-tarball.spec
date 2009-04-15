%define with_managed yes
%define with_ffmpeg no
%define with_cairo embedded

Name:           moonlight
License:        LGPL v2.0 only
Group:          Productivity/Multimedia/Other
Summary:        Novell Moonlight
Url:            http://go-mono.com/moonlight/
Version:        1.0.1
Release:        1
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
%if %{with_managed} != no
BuildRequires:  gtk-sharp2 mono-devel monodoc-core rsvg2-sharp
%endif
%if %{with_ffmpeg} == yes
BuildRequires:  libffmpeg-devel
%endif
BuildRequires:  alsa-devel gcc-c++ gtk2-devel
%if 0%{?suse_version} >= 1100
BuildRequires:  libpulse-devel
BuildRequires:  libexpat-devel mozilla-xulrunner190-devel
%else
BuildRequires:  mozilla-xulrunner181-devel
%endif
%if 0%{?sles_version} == 10
BuildRequires:  -mozilla-xulrunner181-devel -rsvg2-sharp
BuildRequires:  gecko-sdk rsvg-sharp2
%endif
%define debug_package_requires libmoon0 = %{version}-%{release}
#### suse options ###
%if 0%{?suse_version}
%if %{suse_version} > 1100
%define with_cairo system
%endif

%description
Moonlight is an open source implementation of Microsoft Silverlight for
Unix systems.



%post

%postun

%files
%defattr(-, root, root)
%doc DUMMY

%prep

%build
%{?env_options}
%{?configure_options}
autoreconf -f -i
%configure --with-ffmpeg=%{with_ffmpeg} \
			--with-managed=%{with_managed} \
			 --with-cairo=%{with_cairo}

%install
%{?env_options}
touch DUMMY
make dist-bzip2

%clean
rm -rf ${RPM_BUILD_ROOT}

%changelog
