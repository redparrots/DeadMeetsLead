//#define LOG_INPUT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Graphics
{
    public delegate void UpdateEventHandler(object sender, UpdateEventArgs e);

    [Serializable]
    public class InputHandler
    {
        static InputHandler()
        {
            LogInit();
        }

        public void ProcessMessage(MessageType m, EventArgs args)
        {
            LogEvent(this, m);
            ProcessMessage((int)m, args);
        }
        public virtual void ProcessMessage(int m, EventArgs args)
        {
            MessageType mt = (MessageType)m;
            switch (mt)
            {
                case MessageType.MouseClick:
                    OnMouseClick((System.Windows.Forms.MouseEventArgs)args);
                    break;
                case MessageType.Click:
                    OnClick(args);
                    break;
                case MessageType.MouseDown:
                    OnMouseDown((System.Windows.Forms.MouseEventArgs)args);
                    break;
                case MessageType.MouseMove:
                    OnMouseMove((System.Windows.Forms.MouseEventArgs)args);
                    break;
                case MessageType.MouseLeave:
                    OnMouseLeave(args);
                    break;
                case MessageType.MouseEnter:
                    OnMouseEnter(args);
                    break;
                case MessageType.MouseUp:
                    OnMouseUp((System.Windows.Forms.MouseEventArgs)args);
                    break;
                case MessageType.MouseWheel:
                    OnMouseWheel((System.Windows.Forms.MouseEventArgs)args);
                    break;
                case MessageType.Update:
                    OnUpdate((UpdateEventArgs)args);
                    break;
                case MessageType.KeyDown:
                    var e = (System.Windows.Forms.KeyEventArgs)args;
                    if (PreviewKeyDown != null) PreviewKeyDown(this, e);
                    if (!e.SuppressKeyPress)
                        OnKeyDown(e); 
                    break;
                case MessageType.KeyPress:
                    OnKeyPress((System.Windows.Forms.KeyPressEventArgs)args);
                    break;
                case MessageType.KeyUp:
                    OnKeyUp((System.Windows.Forms.KeyEventArgs)args);
                    break;
                case MessageType.Resize:
                    OnResize(args);
                    break;
            }
        }

        [field:NonSerialized]
        public event MouseEventHandler MouseClick;
        [field: NonSerialized]
        public event EventHandler MouseEnter;
        [field: NonSerialized]
        public event EventHandler MouseLeave;
        [field: NonSerialized]
        public event MouseEventHandler MouseMove;
        [field: NonSerialized]
        public event MouseEventHandler MouseDown;
        [field: NonSerialized]
        public event MouseEventHandler MouseUp;
        [field: NonSerialized]
        public event MouseEventHandler MouseWheel;
        [field: NonSerialized]
        public event EventHandler Click;
        [field: NonSerialized]
        public event KeyEventHandler PreviewKeyDown;
        [field: NonSerialized]
        public event KeyEventHandler KeyDown;
        [field: NonSerialized]
        public event KeyEventHandler KeyUp;
        [field: NonSerialized]
        public event UpdateEventHandler Update;

        protected virtual void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseDown != null) MouseDown(this, e);
        }
        protected virtual void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseUp != null) MouseUp(this, e);
        }
        protected virtual void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseClick != null) MouseClick(this, e);
        }
        protected virtual void OnClick(EventArgs e)
        {
            if (Click != null) Click(this, e);
        }
        protected virtual void OnUpdate(UpdateEventArgs e)
        {
            if (Update != null) Update(this, e);
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            if (MouseEnter != null) MouseEnter(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            if (MouseLeave != null) MouseLeave(this, e);
        }
        protected virtual void OnMouseMove(System.Windows.Forms.MouseEventArgs e) 
        {
            if (MouseMove != null)
                MouseMove(this, e);
        }
        protected virtual void OnMouseWheel(System.Windows.Forms.MouseEventArgs e) 
        {
            if (MouseWheel != null)
                MouseWheel(this, e);
        }
        protected virtual void OnKeyDown(System.Windows.Forms.KeyEventArgs e) 
        {
            if (KeyDown != null) KeyDown(this, e);
        }
        protected virtual void OnKeyUp(System.Windows.Forms.KeyEventArgs e) 
        {
            if (KeyUp != null) KeyUp(this, e);
        }
        protected virtual void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e) { }
        protected virtual void OnResize(EventArgs e) { }

        public String GetInputHierarchyDescription()
        {
            StringBuilder s = new StringBuilder();
            GetInputHierarchyDescription(s, 0);
            return s.ToString();
        }
        public virtual void GetInputHierarchyDescription(StringBuilder s, int depth)
        {
            s.Append("".PadLeft(depth*5, ' ')).AppendLine(GetType().Name);
        }

        #region Log
        [System.Diagnostics.Conditional("LOG_INPUT")]
        static void LogInit()
        {
            log = new TextLogger("InputLog");
            log.WriteRaw("Date: " + Application.ProgramStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n\r\n");
            log.WriteRaw("Time        | Type         Message\r\n");
        }

        [System.Diagnostics.Conditional("LOG_INPUT")]
        static void LogEvent(InputHandler obj, MessageType message)
        {
            log.Write(obj.GetType().Name.PadRight(13) + message);
        }
        static TextLogger log;
        #endregion
    }

    public class FilteredInputHandler : InputHandler
    {
        public FilteredInputHandler() 
        {
            for (int i = 0; i < blocked.Length; i++)
                blocked[i] = false;
        }
        bool[] blocked = new bool[256];
        public void Block(MessageType type) { blocked[(int)type] = true; }
        public void Unblock(MessageType type) { blocked[(int)type] = false; }
        public InputHandler InputHandler { get; set; }
        public override void ProcessMessage(int m, EventArgs args)
        {
            base.ProcessMessage(m, args);
            if(InputHandler != null && !blocked[m])
                InputHandler.ProcessMessage(m, args);
        }
        public override void GetInputHierarchyDescription(StringBuilder s, int depth)
        {
            base.GetInputHierarchyDescription(s, depth);
            if (InputHandler != null)
                this.InputHandler.GetInputHierarchyDescription(s, depth + 1);
        }
    }


    public class ShareInputHandler : InputHandler
    {
        public ShareInputHandler() { InputHandlers = new List<InputHandler>(); }
        public List<InputHandler> InputHandlers { get; private set; }
        public override void ProcessMessage(int m, EventArgs args)
        {
            base.ProcessMessage(m, args);
            foreach (var v in InputHandlers)
                v.ProcessMessage(m, args);
        }
        public override void GetInputHierarchyDescription(StringBuilder s, int depth)
        {
            base.GetInputHierarchyDescription(s, depth);
            foreach(var v in InputHandlers)
                v.GetInputHierarchyDescription(s, depth + 1);
        }
    }
}
