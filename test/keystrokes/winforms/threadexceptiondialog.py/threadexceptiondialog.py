#!/usr/bin/python

from macaroon.playback import *
import utils

sequence = MacroSequence()

sequence.append(utils.StartRecordingAction())
sequence.append(WaitForWindowActivate("ThreadExceptionDialog control",None))
sequence.append(utils.AssertPresentationAction(
    "app active",
    ["BRAILLE LINE:  'ThreadExceptionDialog control Frame'",
     "     VISIBLE:  'ThreadExceptionDialog control Frame', cursor=1",
     "BRAILLE LINE:  'Raise an Exception Button'",
     "     VISIBLE:  'Raise an Exception Button', cursor=1",
     "SPEECH OUTPUT: 'ThreadExceptionDialog control frame'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Raise an Exception button'"]))


sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Return"))
sequence.append(utils.AssertPresentationAction(
    "click raise button",
    ["BRAILLE LINE:  'Show Details Button'",
     "     VISIBLE:  'Show Details Button', cursor=1",
     "BRAILLE LINE:  'ThreadExceptionDialog control Dialog'",
     "     VISIBLE:  'ThreadExceptionDialog control Di', cursor=1",
     "BRAILLE LINE:  'Show Details Button'",
     "     VISIBLE:  'Show Details Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Show Details button'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'ThreadExceptionDialog control An unhandled exception has occurred in you application. If you click Ignore the application will ignore this error and attempt to continue. If you click Abort, the application will quit immediately. Division by zero'",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Show Details button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next control",
    ["BRAILLE LINE:  'Ignore Button'",
     "     VISIBLE:  'Ignore Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Ignore button'"]))
     
sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("Tab"))
sequence.append(utils.AssertPresentationAction(
    "jump to next control",
    ["BRAILLE LINE:  'Abort Button'",
     "     VISIBLE:  'Abort Button', cursor=1",
     "SPEECH OUTPUT: ''",
     "SPEECH OUTPUT: 'Abort button'"]))

sequence.append(utils.StartRecordingAction())
sequence.append(KeyComboAction("KP_Subtract"))
sequence.append(utils.AssertPresentationAction(
    "flat-review (kp_substract) ",
    ["BRAILLE LINE:  'Show Details Ignore Abort'",
     "     VISIBLE:  'Show Details Ignore Abort', cursor=21",
     "SPEECH OUTPUT: 'Abort'"]))


sequence.append(utils.AssertionSummaryAction())

sequence.start()
