# Current state

* Pure UIA API (no ATK) works for:
    * Standart WinForms controls;
    * Custom controls if one implements theirs Providers.
* ATK API isn't updated at this time.
* Tests aren't  updated too.


# Pure UIA case

## Simplified diagram

```

   +-------+                                                                                             +--------+
   |  App  |                                                                                             |  Test  |
   +---+---+                                                                                             +---+----+
       |                                                                                                     |
  +----+---+  +----------------------+                                                             +---------+----------+
  |  Mono  +--+ UIAutomationWinforms |                                                             | UIAutomationClient |
  |WinForms|  |       (dll)          |                                                             |       (dll)        |
  +--------+  +----------+-----------+                        +-----------+                        +---------+----------+
              +----------+-----------++---------------+       |           |       +---------------++---------+----------+
              |  UIAutomationBridge  ++ UiaDbusBridge |<----->|   D-Bus   |<----->| UiaDbusSource ++ UIAutomationSource |
              |       (dll)          ||     (dll)     |       |           |       |     (dll)     ||       (dll)        |
              +----------------------++---------------+       +-----------+       +---------------++--------------------+
 \                                                      /                       \                                         /
  \                                                    /                         \                                       /
   ----------------------------------------------------                           ---------------------------------------
               Mono process of an Application                                             Mono process of a Test
```
## How to build

### In-system build

One can build the following projects:
* `UIAutomation`
* `UIAutomationWinforms`
* `UiaDbus`

To do this visit correspondent directories and execute there:
```
cd "${ProjectDirectory}"
./autogen.sh --prefix=/usr --disable-tests
make
```

Wuth `--prefix=/usr` one can install assemblies to the local GAC. To do it just execute as superuser (Debian example):
```
cd "${ProjectDirectory}"
sudo make install
```
 
To build in *Debian 9* one need to install the following packages:
```
sudo apt install \
    mono-complete \
    intltool libtool-bin \
    libglib2.0-cil-dev libglib3.0-cil-dev \
    libgtk2.0-cil-dev libgtk3.0-cil-dev \
    libdbus-glib2.0-cil-dev \
``` 

## How to use

1. Make UIA API assemblies are available for Mono. There are some ways:

    * Install assemblies to the local GAC. See ["In-system build"](###In-system-build) section above.
    * Store assemblies in some directory `${assemblies_dir}` and update environment:
        ```
        export MONO_PATH="${assemblies_dir}:${MONO_PATH}"
        ```

2. Run your application.
    * To avoid errors related with ATK one can update environment:
        ```
        export MONO_UIA_BRIDGE="UiaDbusBridge, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812"
        ```

3. Run test. See ["Examples" section](###examples) below.
    * Update environment to avoid ATK related errors:
        ```
        export MONO_UIA_SOURCE="UiaDbusSource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f4ceacb585d99812"
        ```

### Examples

Please, refer to the [test/pure-uia/ReadMe.md](test/pure-uia/ReadMe.md). Couple Python examples are described there.

### Tune UIA API behaviour

Some UIA API setting may be changes by means of environment variables. One may refer to the `EnvironmentVaribles` [class](UIAutomation/UIAutomationHelpers/Mono.UIAutomation.Helpers/EnvironmentVaribles.cs) for more details.

Current available settings:
* `MONO_UIA_ENABLED={0,1}`: API switcher.
* `MONO_UIA_UISYNCCONTEXT={0,1}`: Thread safety setting.
* `MONO_UIA_BRIDGE`: Application side bridges to be used.
* `MONO_UIA_SOURCE`: Test side sources to be used.
* `MONO_UIA_NAVIGATION_TREE_ERR`: Usefull to debug some API errors.
* `MONO_UIA_LOG_LEVEL`
* ...

### Additional requirements

Mono`s UIA API uses [dbus-sharp](https://github.com/mono/dbus-sharp) project. Unfortunately there are some issues not fixed yet (PRs [#71](https://github.com/mono/dbus-sharp/pull/71), [#72](https://github.com/mono/dbus-sharp/pull/72), [#73](https://github.com/mono/dbus-sharp/pull/73)). To solve this one can build library from the ['axxon' branch of this fork](https://github.com/itvgroup/dbus-sharp/tree/axxon).


# Old README from the 2010s

```
Home of effort to implement UI Automation (UIA) on Linux.

Summary (from http://www.mono-project.com/Accessibility )
=========================================================

    * Project Goals
          o Make WinForms accessible
          o Make Moonlight accessible
          o Allow UI Automation based Accessibility Technologies to run on Linux 

UI Automation: The User Interface Automation (UIA) specification is an advanced
accessibility framework that Microsoft has released to the community including
an irrevocable pledge of patent rights for anyone implementing the specification.
This [project includes] work to build a Linux implementation of the UIA and an
adapter to make it work well with Linux accessibility projects. 


Directory Layout
================

AtspiUiaSource: AT-SPI2 realization of UIA D-Bus interfaces.
MoonAtkBridge: UIA providers for silverlight controls and components.
UiaAtkBridge: Bridge between UIA providers and ATK.
UiaDbus: D-Bus interface to UIA.
UIAutomation: Implementation of Microsoft UIA assemblies.
UIAutomationWinforms: UIA providers for winforms controls and components.

uia2atk.mdw: MonoDevelop solution for code in above projects.

test: Samples, tests, and other QA resources.
build: Package build scripts and resources.
```