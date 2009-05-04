#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("Button_Label_LinkLabel controls",None))
sequence.append(utils.AssertPresentationAction(
    "Button_Label_LinkLabel controls frame active",
    ["BRAILLE LINE:  'Button_Label_LinkLabel controls Frame'",
     "     VISIBLE:  'Button_Label_LinkLabel controls ', cursor=1",
     "BRAILLE LINE:  'openSUSE:www.opensuse.org '",
     "     VISIBLE:  'openSUSE:www.opensuse.org ', cursor=1",
     "SPEECH OUTPUT: 'Button_Label_LinkLabel controls frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'openSUSE:www.opensuse.org \n \n webmail:gmail.novell.com label'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Down"))
sequence.append(utils.AssertPresentationAction(
    "next entry",
    ["BRAILLE LINE:  'button1 Button'",
     "     VISIBLE:  'button1 Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'button1 button'"]))

sequence.append(utils.AssertionSummaryAction())

sequence.start()
