EXPAND_OR_CONTRACT = "expand or contract"
ACTIVATE = "activate"

# very incomplete
class Button(object):
    CLICK = "click"

    actions = (CLICK,)
class ToggleButton(Button):
    pass

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

class TreeViewTableCell:
    CLICK = "click"
    actions = (CLICK, )

class CheckBox(Button):
    pass

class List(Button):
    pass

class ListItem(object):
    CLICK = "click"

    actions = (CLICK, )

class CheckedListItem(object):
    CLICK = "click"
    TOGGLE = "toggle"

    actions = (CLICK, TOGGLE,)

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

class TableColumnHeader(object):
    CLICK = "click"

    actions = (CLICK, )

class TableCell(object):
    CLICK = "click"

    actions = (CLICK, )

class TabPage(object):
    CLICK = "click"

    actions = (CLICK, )

class HyperlinkButton(object):
    JUMP = "jump"

    actions = (JUMP, )

