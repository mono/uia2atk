#!/usr/bin/env python3

from collections import namedtuple
import os
import time

import dbus
from bs4 import BeautifulSoup


max_level = 200  # or None
look_for_missed_elements = True


bus = dbus.SessionBus()

# time.sleep(3)  # One may want some delay to switch to an application windows


def main():
    uia_conn_names = look_for_uia_application()

    uia_root_elements = []
    for name in uia_conn_names:
        root_elem = get_app_root_elems(name)
        assert root_elem, f"name = '{name}' root_elem = '{root_elem}'"
        uia_root_elements.append(root_elem)

    print(f"* uia_conn_names = {uia_conn_names}")
    print(f"* uia_root_elements len = {len(uia_root_elements)}")
    print(f"* max_level = {max_level}")
    print()

    printed_elements_path = set()
    for app_elements in uia_root_elements:
        print(f"+ app_elements = {app_elements}")
        bus_name = list(set([str(root_elem.bus_name) for root_elem in app_elements]))
        assert len(bus_name) == 1, f"bus_name = {bus_name}"
        print(f"+ Application {bus_name[0]}:")
        print()
        for root_elem in app_elements:
            pep = print_tree(root_elem, indent=0, neighbors=True, max_level=max_level)
            printed_elements_path.update(pep)

    # print('org.my.name' in uia_conn_names)

    if not look_for_missed_elements:
        return

    print()
    print("Tree-missed elements:")
    at_least_one = False
    for name in uia_conn_names:
        introspect_data = introspect(name, '/org/mono/UIAutomation/Element')
        assert introspect_data, f'Connection name: [{name}]'
        introspect_soup = BeautifulSoup(introspect_data, 'lxml')
        elements = set(f"/org/mono/UIAutomation/Element/{elem_node['name']}" for elem_node in introspect_soup.node.find_all("node"))

        for elem_path in (elements - printed_elements_path):
            try:
                print(elem_path)
                print_elem_by_path(name, elem_path, indent=0, neighbors=True)
            except Exception as ex:
                print(f"elem={elem}: {ex}")
            else:
                at_least_one = True

    if not at_least_one:
        print("  <none>")


def look_for_uia_application():
    uia_conn_names = []
    for conn_name in bus.list_names():
        try:
            pid, command_line, _ = get_app_info(conn_name)
        except Exception as ex:
            print(f"look_for_uia_application: '{conn_name}'' causes '{ex}'")
            continue

        if pid == os.getpid():
            continue

        if not conn_name.startswith('org.mono.uia.guid_'):
            continue

        introspect_data = introspect(conn_name, "/org/mono/UIAutomation/Application")
        if not introspect_data:
            continue

        introspect_soup = BeautifulSoup(introspect_data, 'lxml')
        is_uia_app = bool(introspect_soup.find_all("interface", attrs={"name": "org.mono.UIAutomation.Application"}))
        if is_uia_app:
            uia_conn_names.append(conn_name)

    return uia_conn_names


def get_app_root_elems(conn_name):
    app_iface = dbus.Interface(bus.get_object(conn_name, "/org/mono/UIAutomation/Application"),
                               dbus_interface="org.mono.UIAutomation.Application")
    try:
        paths = app_iface.GetRootElementPaths()
    except dbus.exceptions.DBusException as ex:
        print(f"get_app_root_elem(conn_name={conn_name}) ERR: {ex}")
    else:
        ifaces = [get_automation_element_iface(conn_name, path) for path in paths]
        return ifaces


def print_tree(base_elem, indent=0, neighbors=True, max_level=None):
    printed_elements_path = set()

    print_elem(base_elem, indent=indent, neighbors=neighbors)
    printed_elements_path.add(str(base_elem.object_path))

    if max_level and indent >= max_level:
        return

    for child_iface in get_child_elems(base_elem):
        pep = print_tree(child_iface, indent=indent+1, neighbors=neighbors, max_level=max_level)
        printed_elements_path.update(pep)

    return printed_elements_path


def print_elem_by_path(bus_name, elem_path, indent, neighbors=False, filter=None):
    if filter and not filter(elem_path):
        return
    elem = get_automation_element_iface(bus_name, elem_path)
    print_elem(elem, indent=indent, neighbors=neighbors)


def print_elem(elem_iface, indent, neighbors=False):
    def get_short_path(path):
        return path.split("/", 4)[-1]

    def safe_str(in_str):
        return in_str  #.encode("utf-8", errors="backslashreplace")

    indent = f"l{indent: 2d}: " + "    " * indent

    print(indent + "{short_path}: Name='{name}', LocalizedControlType='{ctrltype}', AID='{aid}', RID='{rid}'".format(
        short_path=get_short_path(elem_iface.object_path),
        name=safe_str(elem_iface.get_Name()),
        ctrltype=safe_str(elem_iface.get_LocalizedControlType()),
        aid=safe_str(elem_iface.get_AutomationId()),
        rid=safe_str(elem_iface.get_RuntimeId())))

    if neighbors:
        print(" " * len(indent) + "parent={parent} | prev={prev}, next={next} | fchild={fchild}, lchild={lchild}".format(
            parent=get_short_path(elem_iface.get_ParentElementPath()),
            next=get_short_path(elem_iface.get_NextSiblingElementPath()),
            prev=get_short_path(elem_iface.get_PreviousSiblingElementPath()),
            fchild=get_short_path(elem_iface.get_FirstChildElementPath()),
            lchild=get_short_path(elem_iface.get_LastChildElementPath())))

    try:
        brect_raw = elem_iface.get_BoundingRectangle()
        brect = tuple(map(float, brect_raw))
    except:
        brect = "[ERR]"
    print(" " * len(indent) + f"BoundingRectangle={brect}")

    print()


def get_child_elems(parent_iface):
    conn_name = parent_iface.bus_name

    child_ifaces = []

    first_child_path = parent_iface.get_FirstChildElementPath()
    if not first_child_path:
        return child_ifaces
    first_child_iface = get_automation_element_iface(conn_name, first_child_path)

    child_iface = first_child_iface
    while True:
        child_ifaces.append(child_iface)

        next_sibling_path = child_iface.get_NextSiblingElementPath()
        if not next_sibling_path:
            break
        next_sibling_iface = get_automation_element_iface(conn_name, next_sibling_path)

        child_iface = next_sibling_iface

    return child_ifaces


def get_app_info(conn_name):
    iface = dbus.Interface(bus.get_object("org.freedesktop.DBus", "/"),
                           dbus_interface="org.freedesktop.DBus")
    pid = iface.GetConnectionUnixProcessID(conn_name)

    if pid > 0:
        procpath = '/proc/' + str(pid)
        with open(procpath + '/cmdline', 'r') as f:
            command_line = " ".join(f.readline().split('\0')).strip()

        try:
            bin_path = os.readlink(procpath + '/exe').strip()
        except:
            bin_path = 'ERR'
    else:
        command_line = ''
        bin_path = ''

    return pid, command_line, bin_path


def introspect(conn_name, path):
    app = dbus.Interface(bus.get_object(conn_name, path),
                         dbus_interface="org.freedesktop.DBus.Introspectable")
    try:
        #print(f"try {conn_name}...")
        return app.Introspect()
    except Exception as ex:
        #print("ERR {}: {}, {}").format(ex, conn_name, path))
        return None


def get_automation_element_iface(conn_name, uia_elem_path):
    return dbus.Interface(bus.get_object(conn_name, uia_elem_path),
                          dbus_interface="org.mono.UIAutomation.AutomationElement")


if __name__ == "__main__":
    main()

"""
D-Bus raw data example:

('<!DOCTYPE node PUBLIC "-//freedesktop//DTD D-BUS Object Introspection '
 '1.0//EN" "http://www.freedesktop.org/standards/dbus/1.0/introspect.dtd">\n'
 '<!-- NDesk.DBus 0.6.0 -->\n'
 '<node>\n'
 '  <interface name="org.freedesktop.DBus.Introspectable">\n'
 '    <method name="Introspect">\n'
 '      <arg name="data" direction="out" type="s" />\n'
 '    </method>\n'
 '  </interface>\n'
 '  <interface name="org.mono.UIAutomation.Application">\n'
 '    <method name="GetVisibleRootElementPaths">\n'
 '      <arg name="ret" direction="out" type="as" />\n'
 '    </method>\n'
 '    <method name="GetFocusedElementPath">\n'
 '      <arg name="ret" direction="out" type="s" />\n'
 '    </method>\n'
 '    <method name="GetElementPathFromHandle">\n'
 '      <arg name="handle" direction="in" type="i" />\n'
 '      <arg name="ret" direction="out" type="s" />\n'
 '    </method>\n'
 '    <method name="AddAutomationEventHandler">\n'
 '      <arg name="eventId" direction="in" type="i" />\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="AddRootElementAutomationEventHandler">\n'
 '      <arg name="eventId" direction="in" type="i" />\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="AddAutomationPropertyChangedEventHandler">\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '      <arg name="properties" direction="in" type="ai" />\n'
 '    </method>\n'
 '    <method name="AddRootElementAutomationPropertyChangedEventHandler">\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '      <arg name="properties" direction="in" type="ai" />\n'
 '    </method>\n'
 '    <method name="AddStructureChangedEventHandler">\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="AddRootElementStructureChangedEventHandler">\n'
 '      <arg name="scope" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveAutomationEventHandler">\n'
 '      <arg name="eventId" direction="in" type="i" />\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveRootElementAutomationEventHandler">\n'
 '      <arg name="eventId" direction="in" type="i" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveAutomationPropertyChangedEventHandler">\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveRootElementAutomationPropertyChangedEventHandler">\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveStructureChangedEventHandler">\n'
 '      <arg name="elementRuntimeId" direction="in" type="ai" />\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveRootElementStructureChangedEventHandler">\n'
 '      <arg name="handlerId" direction="in" type="i" />\n'
 '    </method>\n'
 '    <method name="RemoveAllEventHandlers">\n'
 '      <arg name="handlerIdMask" direction="in" type="i" />\n'
 '    </method>\n'
 '    <signal name="AutomationEvent">\n'
 '      <arg name="handlerId" direction="out" type="i" />\n'
 '      <arg name="eventId" direction="out" type="i" />\n'
 '      <arg name="providerPath" direction="out" type="s" />\n'
 '    </signal>\n'
 '    <signal name="AutomationPropertyChanged">\n'
 '      <arg name="handlerId" direction="out" type="i" />\n'
 '      <arg name="eventId" direction="out" type="i" />\n'
 '      <arg name="providerPath" direction="out" type="s" />\n'
 '      <arg name="propertyId" direction="out" type="i" />\n'
 '      <arg name="oldValue" direction="out" type="v" />\n'
 '      <arg name="newValue" direction="out" type="v" />\n'
 '    </signal>\n'
 '    <signal name="StructureChanged">\n'
 '      <arg name="handlerId" direction="out" type="i" />\n'
 '      <arg name="eventId" direction="out" type="i" />\n'
 '      <arg name="providerPath" direction="out" type="s" />\n'
 '      <arg name="changeType" direction="out" type="i" />\n'
 '    </signal>\n'
 '    <signal name="RootElementsChanged" />\n'
 '    <signal name="FocusChanged">\n'
 '      <arg name="providerPath" direction="out" type="s" />\n'
 '    </signal>\n'
 '  </interface>\n'
 '</node>')
"""
