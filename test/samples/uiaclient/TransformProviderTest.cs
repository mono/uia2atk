using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace TransformProviderTest
{
    public class TransformProviderForm : Form
    {
        public TransformProviderForm()
        {
            var t = new TransformProviderControl();
            t.Name = "TransformProviderControl1";
            t.Width = 100;
            t.Height = 100;
            t.Top = 25;
            t.Left = 25;
            Controls.Add(t);
            Text = "Transform Test";
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TransformProviderForm());
        }
    }

    public class TransformProviderControl : Control, IRawElementProviderSimple, ITransformProvider
    {
        public TransformProviderControl()
            : base()
        {
            BackColor = Color.Black;
            RotateDegree = 0.0;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_GETOBJECT
                            && (int)m.LParam == AutomationInteropProvider.RootObjectId)
            {
                m.Result = AutomationInteropProvider.ReturnRawElementProvider(
                    this.Handle, m.WParam, m.LParam,
                    (IRawElementProviderSimple)this);
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            int centerX = Width / 2;
            int centerY = Height / 2;
            const int CenterRadius = 4;
            e.Graphics.FillEllipse(Brushes.Red, centerX - CenterRadius, centerY - CenterRadius,
                CenterRadius * 2, CenterRadius * 2);
            e.Graphics.DrawLine(new Pen(Color.Red), centerX, centerY,
                centerX + (float)(Math.Cos(RotateDegree / 180.0 * Math.PI) * centerX),
                centerY - (float)(Math.Sin(RotateDegree / 180.0 * Math.PI) * centerY));
        }

        public double RotateDegree { get; set; }

        #region IRawElementProviderSimple Members
        public object GetPatternProvider(int patternId)
        {
            if (patternId == TransformPattern.Pattern.Id)
                return this;

            return null;
        }

        public object GetPropertyValue(int propertyId)
        {
            if (propertyId == TransformPattern.CanMoveProperty.Id)
                return CanMove;
            else if (propertyId == TransformPattern.CanResizeProperty.Id)
                return CanResize;
            else if (propertyId == TransformPattern.CanRotateProperty.Id)
                return CanRotate;
            else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
                return string.Format ("{0}, r:{1}", Name, RotateDegree);

            return null;
        }

        public IRawElementProviderSimple HostRawElementProvider
        {
            get { return this; }
        }

        public ProviderOptions ProviderOptions
        {
            get { return ProviderOptions.ServerSideProvider; }
        }
        #endregion

        #region ITransformProvider Members

        public bool CanMove
        {
            get { return true; }
        }

        public bool CanResize
        {
            get { return true; }
        }

        public bool CanRotate
        {
            get { return true; }
        }

        public new void Move(double x, double y)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                this.Left = (int)x;
                this.Top = (int)y;
                this.Invalidate();
            });
        }

        public new void Resize(double width, double height)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                this.Width = (int)width;
                this.Height = (int)height;
                this.Invalidate();
            });
        }

        public void Rotate(double degrees)
        {
            this.RotateDegree = degrees;
            this.Invoke((MethodInvoker)delegate()
            {
                this.Invalidate();
            });
        }

        #endregion

        #region Private Fields
        private const int WM_GETOBJECT = 0x003D;
        #endregion
    }
}
