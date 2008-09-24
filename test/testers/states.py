# Indicates an invalid state - probably an error condition.
INVALID = "invalid" 

# Indicates a window is currently the active window, or is an active subelement 
# within a container or table
ACTIVE = "active"

# Indicates that the object is 'armed', i.e. will be activated by if a pointer
# button-release event occurs within its bounds. Buttons often enter this state
# when a pointer click occurs within their bounds, as a precursor to
# activation.
ARMED = "armed"

# Indicates the current object is busy, i.e. onscreen representation is in the
# process of changing, or the object is temporarily unavailable for interaction
# due to activity already in progress. This state may be used by implementors
# of Document to indicate that content loading is underway. It also may
# indicate other 'pending' conditions; clients may wish to interrogate this
# object when the ATK_STATE_BUSY flag is removed.
BUSY = "busy"

# Indicates this object is currently checked, for instance a checkbox is
# 'non-empty'.
CHECKED = "checked"

# Indicates that this object no longer has a valid backing widget (for
# instance, if its peer object has been destroyed)
DEFUNCT = "defunct"

# Indicates the user can change the contents of this object
EDITABLE = "editable"

# Indicates that this object is enabled, i.e. that it currently reflects some
# application state. Objects that are "greyed out" may lack this state, and may
# lack the STATE_SENSITIVE if direct user interaction cannot cause them to
# acquire STATE_ENABLED. See also: ATK_STATE_SENSITIVE
ENABLED = "enabled"

# Indicates this object allows progressive disclosure of its children
EXPANDABLE = "expandable"

# Indicates this object its expanded - see ATK_STATE_EXPANDABLE above
EXPANDED = "expanded"

# Indicates this object can accept keyboard focus, which means all events
# resulting from typing on the keyboard will normally be passed to it when it
# has focus
FOCUSABLE = "focusable"

# Indicates this object currently has the keyboard focus
FOCUSED = "focused"

# Indicates the orientation of this object is horizontal; used, for instance,
# by objects of ATK_ROLE_SCROLL_BAR. For objects where vertical/horizontal
# orientation is especially meaningful.
HORIZONTAL = "horizontal"

# Indicates this object is minimized and is represented only by an icon
ICONIFIED = "iconified"

# Indicates something must be done with this object before the user can
# interact with an object in a different window
MODAL = "modal"

# Indicates this (text) object can contain multiple lines of text
MULTI_LINE = "multi line"

#Indicates this object allows more than one of its children to be selected at
# the same time, or in the case of text objects, that the object supports
# non-contiguous text selections.
MULTISELECTABLE = "multiselectable"

# Indicates this object paints every pixel within its rectangular region.
OPAQUE = "opaque"

# Indicates this object is currently pressed; c.f. ATK_STATE_ARMED
PRESSED = "pressed"

# Indicates the size of this object is not fixed
RESIZABLE = "resizable"

# Indicates this object is the child of an object that allows its children to
# be selected and that this child is one of those children that can be selected
SELECTABLE = "selectable"

# Indicates this object is the child of an object that allows its children to
# be selected and that this child is one of those children that has been
# selected
SELECTED = "selected"

# Indicates this object is sensitive, e.g. to user interaction. STATE_SENSITIVE
# usually accompanies STATE_ENABLED for user-actionable controls, but may be
# found in the absence of STATE_ENABLED if the current visible state of the
# control is "disconnected" from the application state. In such cases, direct
# user interaction can often result in the object gaining STATE_SENSITIVE, for
# instance if a user makes an explicit selection using an object whose current
# state is ambiguous or undefined. see STATE_ENABLED, STATE_INDETERMINATE.
SENSITIVE = "sensitive"

# Indicates this object, the object's parent, the object's parent's parent,
# and so on, are all 'shown' to the end-user, i.e. subject to "exposure" if
# blocking or obscuring objects do not interpose between this object and the
# top of the window stack.
SHOWING = "showing"

# Indicates this (text) object can contain only a single line of text
SINGLE_LINE = "single_line"

# Indicates that the information returned for this object may no longer be
# synchronized with the application state. This is implied if the object has
# STATE_TRANSIENT, and can also occur towards the end of the object peer's
# lifecycle. It can also be used to indicate that the index associated with
# this object has changed since the user accessed the object (in lieu of
# "index-in-parent-changed" events).
STALE = "stale"

# Indicates this object is transient, i.e. a snapshot which may not emit events
# when its state changes. Data from objects with ATK_STATE_TRANSIENT should not
# be cached, since there may be no notification given when the cached data
# becomes obsolete.
TRANSIENT = "transient"

# Indicates the orientation of this object is vertical
VERTICAL = "vertical"

# Indicates this object is visible, e.g. has been explicitly marked for
# exposure to the user.
VISIBLE = "visible"

# Indicates that "active-descendant-changed" event is sent when children become
# 'active' (i.e. are selected or navigated to onscreen). Used to prevent need
# to enumerate all children in very large containers, like tables. The presence
# of STATE_MANAGES_DESCENDANTS is an indication to the client. that the
# children should not, and need not, be enumerated by the client. Objects
# implementing this state are expected to provide relevant state notifications
# to listening clients, for instance notifications of visibility changes and
# activation of their contained child objects, without the client having
# previously requested references to those children.
MANAGES_DESCENDANTS = "manages_descendants"

# Indicates that a check box is in a state other than checked or not checked.
# This usually means that the boolean value reflected or controlled by the
# object does not apply consistently to the entire current context. For
# example, a checkbox for the "Bold" attribute of text may have
# STATE_INDETERMINATE if the currently selected text contains a mixture of
# weight attributes. In many cases interacting with a STATE_INDETERMINATE
# object will cause the context's corresponding boolean attribute to be
# homogenized, whereupon the object will lose STATE_INDETERMINATE and a
# corresponding state-changed event will be fired.
INDETERMINATE = "indeterminate"

# Indicates that an object is truncated, e.g. a text value in a speradsheet
# cell.
TRUNCATED = "truncated"

# Indicates that explicit user interaction with an object is required by the
# user interface, e.g. a required field in a "web-form" interface.
REQUIRED = "required"

# Indicates that the object has encountered an error condition due to failure
# of input validation. For instance, a form control may acquire this state in
# response to invalid or malformed user input.
INVALID_ENTRY = "invalid_entry"

# Indicates that the object in question implements some form of "typeahead" or
# pre-selection behavior whereby entering the first character of one or more
# sub-elements causes those elements to scroll into view or become selected.
# Subsequent character input may narrow the selection further as long as one or
# more sub-elements match the string. This state is normally only useful and
# encountered on objects that implement Selection. In some cases the typeahead
# behavior may result in full or partial "completion" of the data in the input
# field, in which case these input events may trigger text-changed events from
# the AtkText interface. This state supplants ATK_ROLE_AUTOCOMPLETE.
SUPPORTS_AUTOCOMPLETION = "supports_autocompletion"

# Indicates that the object in question supports text selection. It should only
# be exposed on objects which implement the Text interface, in order to
# distinguish this state from ATK_STATE_SELECTABLE, which infers that the
# object in question is a selectable child of an object which implements
# Selection. While similar, text selection and subelement selection are
# distinct operations.
SELECTABLE_TEXT = "selectable_text"

# Indicates that the object is the "default" active component, i.e. the object
# which is activated by an end-user press of the "Enter" or "Return" key.
# Typically a "close" or "submit" button.
DEFAULT = "default"

# Indicates that the object changes its appearance dynamically as an inherent
# part of its presentation. This state may come and go if an object is only
# temporarily animated on the way to a 'final' onscreen presentation. note some
# applications, notably content viewers, may not be able to detect all kinds of
# animated content. Therefore the absence of this state should not be taken as
# definitive evidence that the object's visual representation is static; this
# state is advisory.
ANIMATED = "animated"

# Indicates that the object (typically a hyperlink) has already been
# 'activated', and/or its backing data has already been downloaded, rendered,
# or otherwise "visited".
VISITED = "visited"


#list Button's all states
class Button(object):
    states = (ENABLED, FOCUSABLE, SENSITIVE, SHOWING, VISIBLE)

class RadioButton(Button):
    pass

class Label(object):
    states = (ENABLED, MULTI_LINE, SENSITIVE, SHOWING, VISIBLE)

class Form(object):
    states = (ENABLED, RESIZABLE, SENSITIVE, SHOWING, VISIBLE)

class PictureBox(object):
    states = (SHOWING,)

class Panel(object):
    states = (SHOWING,)

class CheckBox(object):
    states = (ENABLED, FOCUSABLE, SENSITIVE, SHOWING, VISIBLE)

class VScrollBar(object):
    states = (SHOWING, VERTICAL)

class HScrollBar(object):
    states = (SHOWING, HORIZONTAL)

class StatusBar(object):
    states = (SHOWING,)

class ListBox(object):
    states = (SHOWING, ENABLED, SELECTABLE, SENSITIVE)

class ListItem(object):
    states = (SHOWING, ENABLED, SELECTABLE, SENSITIVE)

class ProgressBar(object):
    states = (SHOWING,)

class NumericUpDown(object):
    states = (SHOWING,)

