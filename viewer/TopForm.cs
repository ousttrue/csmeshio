using System;
using System.Windows.Forms;
using System.Drawing;

namespace SlimDXViewer
{

    public class TopForm : Form
    {
        DXControl _dx;

        private int _mouseX;
        private int _mouseY;
        private bool _mouseLeftDown=false;
        private bool _mouseMiddleDown=false;
        private bool _mouseRightDown=false;

        public TopForm(DXControl dx)
        {
            _dx=dx;
            _dx.Parent=this;
            _dx.Dock = DockStyle.Fill;

            Text = "SlimDXViewer";
            Size = new Size(800, 600);

            // add menu
            var ms = new MenuStrip();
            ms.Parent = this;
            MainMenuStrip = ms;

            var file = new ToolStripMenuItem("&File");          
            ms.Items.Add(file);

            var open = new ToolStripMenuItem("&Open", 
                    null, new EventHandler(OnOpen));
            file.DropDownItems.Add(open);

            var exit = new ToolStripMenuItem("&Exit", 
                    null, new EventHandler(OnExit));          
            exit.ShortcutKeys = Keys.Control | Keys.X;
            file.DropDownItems.Add(exit);

            // mouse event
            _dx.MouseDown += new MouseEventHandler(OnMouseDown);
            _dx.MouseUp += new MouseEventHandler(OnMouseUp);
            _dx.MouseMove += new MouseEventHandler(OnMouseMove);
            _dx.MouseWheel += new MouseEventHandler(OnMouseWheel);

            // keyboard event
            KeyPreview = true;
            KeyDown += new KeyEventHandler(OnKeyDown);

            //CenterToScreen();
        }

        void OnOpen(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();

            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter =
                "mmd model(*.pmx;*.pmd)|*.pmx;*.pmd"
                +"|mmd motion(*.vmd)|*.vmd"
                +"|すべてのファイル(*.*)|*.*"
                ;
            ofd.FilterIndex = 1;
            ofd.Title = "open model file";
            ofd.RestoreDirectory = true;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

			var path=ofd.FileName;
			if(path.ToLower().EndsWith(".pmx")){
				var pmx=meshio.Pmx.Loader.loadFromPath(path);
				_dx.Scene.Load(pmx);            
			}
			else if(path.ToLower().EndsWith(".vmd")){
				var vmd=meshio.Vmd.Loader.loadFromPath(path);
			}
			else{
				MessageBox.Show(
						String.Format("unknown file: {0}", path),
						"エラー",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
			}
        }

        void OnExit(object sender, EventArgs e) {
            Close();
        }

        private void OnMouseDown(object sender, MouseEventArgs e) 
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    _mouseLeftDown=true;
                    break;

                case MouseButtons.Middle:
                    _mouseMiddleDown=true;
                    break;

                case MouseButtons.Right:
                    _mouseRightDown=true;
                    break;

                default:
                    Text=string.Format("unknown mouse button: {0}", e.Button);
                    break;
            }
            _mouseX=e.X;
            _mouseY=e.Y;
        }

        private void OnMouseMove(object sender, MouseEventArgs e) 
        {
            int dx=_mouseX-e.X;
            int dy=_mouseY-e.Y;
            var view=_dx.Scene.View;
            if(_mouseLeftDown){
                view.Dolly(dy);
            }
            if(_mouseMiddleDown){
                view.ShiftX((float)-dx);
                view.ShiftY((float)dy);
            }
            if(_mouseRightDown){
                view.Head((float)dx);
                view.Pitch((float)dy);
            }
            _mouseX=e.X;
            _mouseY=e.Y;
        }

        private void OnMouseUp(object sender, MouseEventArgs e) 
        {
            switch(e.Button)
            {
                case MouseButtons.Left:
                    _mouseLeftDown=false;
                    break;

                case MouseButtons.Middle:
                    _mouseMiddleDown=false;
                    break;

                case MouseButtons.Right:
                    _mouseRightDown=false;
                    break;

                default:
                    Text=string.Format("unknown mouse button: {0}", e.Button);
                    break;
            }
            _mouseX=e.X;
            _mouseY=e.Y;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            _dx.Scene.View.Dolly(e.Delta);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Q:
                case Keys.Escape:
                    Close();
                    break;

                default:
                    Text=string.Format("{0}[{0:d}]", e.KeyCode);
                    break;
            }
        }

    }
}

