using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RJCodeUI_M1.RJControls
{
    public class RJDragControl : Component
    {
        /// <summary>
        /// This class inherits from the component class, Component is the base class for all components
        /// in the common language runtime that marshal by reference. In this case, it is basically to 
        /// convert this class to a control without design (component)
        /// 
        /// The <ReleaseCapture()/> method is an external function of the user.32 library, this method Releases
        /// the mouse capture from a window in the current thread and restores normal mouse input processing.
        /// 
        /// The <SendMessage(....)/> method is an external function of the user.32 library, this method Sends the specified 
        /// message to a window or windows. The SendMessage function calls the window procedure for the specified 
        /// window and does not return until the window procedure has processed the message.
        /// <param name="hWnd">A handle to the window whose window procedure will receive the message</param>
        /// <param name="wMsg">The message to be sent</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// 
        /// for these 2 previous methods is necessary to import the <user32.DLL/> library of unmanaged code through
        /// Interop services <see cref="using System.Runtime.InteropServices;"/>
        /// <see cref="DllImportAttribute"/>([DllImport("dll name")]), Indicates that the attributed  
        /// method is exposed by an unmanaged dynamic-link library (DLL) as a static entry point
        /// </summary>
        /// 

        #region -> Fields

        private Control dragControl;//Gets or Sets the control responsible for dragging the form
        private Form targetForm;//Gets or Sets the target form to drag
        #endregion

        #region -> Constructors

        public RJDragControl()
        {
            //Constructor without parameters  
        }
        public RJDragControl(IContainer container) //This constructor is invoked automatically in the form designer when the control is dragged from the toolbox onto the form.
        {
            //Initializes a new instance and associates it with the specified container.
            //This constructor ensures that the object is removed correctly, since it is not a child of the form.
            container.Add((IComponent)this);           
        }
        public RJDragControl(Control _dragControl, Form _targetForm)
        {
            targetForm = _targetForm;
            dragControl = _dragControl;
            dragControl.MouseDown += new MouseEventHandler(Control_MouseDown);//Associate the MouseDown event: Occurs when mouse button is held down on the drag control
        }
        public RJDragControl(Control _dragControl, Form _targetForm, IContainer container)
        {   //Initializes a new instance and associates it with the specified container.
            //This constructor ensures that the object is removed correctly, since it is not a child of the form.
            container.Add((IComponent)this); 
           
            targetForm = _targetForm;
            dragControl = _dragControl;
            dragControl.MouseDown += new MouseEventHandler(Control_MouseDown);//Associate the MouseDown event: Occurs when mouse button is held down on the drag control
        }
      
        #endregion

        #region -> Properties

        [Category("RJ Code Advance")]
        [Description("Set the control responsible for dragging the form")]
        public Control DragControl
        {
            get { return dragControl; }
            set
            {
                dragControl = value;//Set drag control
                if (dragControl != null)
                {
                    dragControl.HandleCreated += new EventHandler(Control_Created);//Associate the HandleCreate event: Occurs when the control is first displayed in its container(Form,panel, etc)
                    dragControl.MouseDown += new MouseEventHandler(Control_MouseDown);//Associate the MouseDown event: Occurs when mouse button is held down on the drag control
                }
            }
        }
        #endregion

        #region -> Import Extern Methods

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
        #endregion

        #region -> Event Methods 

        private void Control_Created(object sender, EventArgs e)
        {
            //Occurs when the control is first displayed in its container(Form,panel, etc)
            if (!this.DesignMode)
                targetForm = dragControl.FindForm();//Find the form to which the control belongs and thus set the target form to drag
        }
        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            //Occurs when mouse button is held down on the control and when moving it will drag the form

            ReleaseCapture();//Releases the mouse capture from a window            
            SendMessage(targetForm.Handle, 0x112, 0xf012, 0);//Send message to a window
            /// <param value="targetForm.Handle">send the target form handle to drag</param>
            /// <param value="0x112">Send WM_SYSCOMMAND as message</param>
            /// <param value="0xf012">Send a fake title bar click(Click on the title bar generates 0xF012)</param>
        }
        #endregion

    }
}
