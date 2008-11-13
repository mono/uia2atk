# very incomplete
class Button(object):
    CLICK = "click"
    PRESS = "press"
    RELEASE = "release"

    actions = (CLICK,)

class TableColumnHeader(Button):
    pass

class RadioButton(Button):
    pass

class ComboBox(object):
    PRESS = "press"

class Entry(object):
    ACTIVATE = "activate"

class Expander(object):
    ACTIVATE = "activate"

class OptionMenu(object):
    PRESS = "press"

class Range(object):
    ACTIVATE = "activate"

class TreeView:
    EXPAND_OR_CONTRACT = "expand or contract"

class CheckBox(Button):
    pass

class ListItem(object):
    CLICK = "click"
    TOGGLE = "toggle"
    ACTIVATE = "activate"

    actions = (TOGGLE, CLICK, )

class List(Button):
    pass

class NumericUpDown(object):
    PRESS = "press"
    PAGE = "page"

    actions = (PRESS, PAGE)

class ComboBox(object):
    PRESS = "press"

    actions = (PRESS, )

class Menu(object):
    CLICK = "click"

    actions = (CLICK, )

class MenuItem(object):
    CLICK = "click"

    actions = (CLICK, )


