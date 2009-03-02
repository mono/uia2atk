using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace CustomControl
{
    public partial class MyCustomControl : UserControl
    {
        public MyCustomControl()
        {
            InitializeComponent();
        }

        public StackPanel StackPanel
        {
            get { return stackPanel; }
        }

        public void AddNewItem()
        {
            TextBox newItem = new TextBox ();
            newItem.Text = string.Format ("Item #{0}", stackPanel.Children.Count + 1);
            stackPanel.Children.Add(newItem);

            AutomationPeer peer;
            if ((peer = FrameworkElementAutomationPeer.FromElement(this)) != null)
                peer.RaiseAutomationEvent(AutomationEvents.StructureChanged);
        }

        protected override AutomationPeer OnCreateAutomationPeer ()
        {
            return new MyCustomControlPeer(this);
        }
    }

    class MyCustomControlPeer : FrameworkElementAutomationPeer
    {
        public MyCustomControlPeer(MyCustomControl control)
            : base(control)
        {
            myCustomControl = control;
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> list = new List<AutomationPeer>();
            foreach (UIElement element in myCustomControl.stackPanel.Children)
                list.Add(FrameworkElementAutomationPeer.CreatePeerForElement(element));
            return list;
        }

        protected override bool IsKeyboardFocusableCore()
        {
            return false;
        }

        MyCustomControl myCustomControl;
    }
}
