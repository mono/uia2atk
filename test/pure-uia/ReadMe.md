Directory contains some demonstration scripts.

Scripts that prints control tree of a runned .Net application:

* `tree-uia-dbus.py`:
    * *Feature*: Communicates with an application under test through D-Bus directly. It is useful to debug UIA API libraries.
    * *Requirement*: `pip install beautifulsoup4 dbus-python`

* `tree-uia-net.py`:
    * *Feature*: This script uses "client" side of UIA API libraries. It is compatible both of .Net Windows and Mono Linux cases.
    * *Requirement*: `pip install pythonnet`

* `tree-uia-pikuli.py`
    * *Feature*: Cross-platform `pikuli` module is involved to deal with UIA API. It is runnable under both of Windows and Linux too: `pythonnet` is used inside.
    * *Requirement*: `pip install pikuli`

Global requirements:
* Pyhton 3.6 or newer
