####Panel
####Label
####CheckBox
####RadioButton
import clr
clr.AddReference('System.Windows.Forms')
clr.AddReference('System.Drawing')

from System.Drawing import *
from System.Windows.Forms import *

class ChecksAndRadiosForm(Form):
    def __init__(self):
        self.Text = "Panel&Label&CheckBox&RadioButton"

        self.Width = 400
        self.Height = 400

        self.setupCheckButtons()
        self.setupRadioButtons()

        self.Controls.Add(self.checkPanel)
        self.Controls.Add(self.radioPanel)

        self.mainLabel1 = Label()
        self.mainLabel1.Text = "Examples for: Panel, Label, CheckBox, RadioButton"
        self.mainLabel1.Location = Point(10,10)
        self.mainLabel1.AutoSize = True
        self.Controls.Add(self.mainLabel1)

    def newPanel(self, x, y):
        panel = Panel()
        panel.Width = 400
        panel.Height = 150
        panel.Location = Point(x, y)
        panel.BorderStyle = BorderStyle.Fixed3D
        return panel

    def setupCheckButtons(self):
        self.checkPanel = self.newPanel(0, 50)

        self.checkLabel = Label()
        self.checkLabel.Text = "multi-choose:"
        self.checkLabel.Location = Point(25, 25)
        self.checkLabel.AutoSize = True

        self.check1 = CheckBox()
        self.check1.Text = "Bananas"
        self.check1.Location = Point(25, 50)
        self.check1.Width = 90

        self.check2 = CheckBox()
        self.check2.Text = "Chicken"
        self.check2.Location = Point(125, 50)
        self.check2.Width = 110

        self.check3 = CheckBox()
        self.check3.Text = "Stuffed Peppers"
        self.check3.Location = Point(240, 50)
        self.check3.Width = 120
        self.check3.Checked = True

        self.checkPanel.Controls.Add(self.checkLabel)
        self.checkPanel.Controls.Add(self.check1)
        self.checkPanel.Controls.Add(self.check2)
        self.checkPanel.Controls.Add(self.check3)

    def setupRadioButtons(self):
        self.radioPanel = self.newPanel(0, 200)

        self.radioLabel1 = Label()
        self.radioLabel1.Text = "Tell Me Your Gender:"
        self.radioLabel1.Location = Point(25, 25)
        self.radioLabel1.AutoSize = True

        self.radio1 = RadioButton()
        self.radio1.Text = "Male"
        self.radio1.Location = Point(25, 50)
        self.radio1.Checked = True
        self.radio1.CheckedChanged += self.checkedChanged

        self.radio2 = RadioButton()
        self.radio2.Text = "Female"
        self.radio2.Location = Point(150, 50)
        self.radio2.CheckedChanged += self.checkedChanged

        self.radioLabel2 = Label()
        self.radioLabel2.Text = "Go On:____"
        self.radioLabel2.Location = Point(25, 80)
        self.radioLabel2.AutoSize = True
        self.radioLabel2.Font = Font("Arial", 15, FontStyle.Bold)
        self.radioLabel2.ForeColor = Color.Red

        self.radioPanel.Controls.Add(self.radioLabel1)
        self.radioPanel.Controls.Add(self.radioLabel2)
        self.radioPanel.Controls.Add(self.radio1)
        self.radioPanel.Controls.Add(self.radio2)


    def checkedChanged(self, sender, args):
        if not sender.Checked:
            return
        if sender.Text == "Female":
            self.radioLabel2.Text = "You are %s" % self.radio2.Text 
        else:
            self.radioLabel2.Text = "You are %s" % self.radio1.Text

form = ChecksAndRadiosForm()
Application.Run(form)
