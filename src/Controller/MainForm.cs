using ElectricFieldVis.Model;
using ElectricFieldVis.View;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace ElectricFieldVis.Controller
{
    public partial class MainForm : Form
    {
        private Renderer _renderer;
        private List<Particle> _particles;
        private Probe _probe;
        private System.Windows.Forms.Timer _timer;
        private float _timeElapsed = 0f;
        private float _lastUpdateTime = 0f;
        private int _fps = 30;
        private StatsForm _statsForm;
        private MenuStrip _menuStrip;

        /// <summary>
        /// Init MainForms components - Model, View, Controller and WinForm itself.
        /// </summary>
        /// <param name="scenarioName">Name of the desired scenario.</param>
        public MainForm(string scenarioName, int grid_w, int grid_h)
        {
            this.Size = new Size(800, 600);

            InitializeModel(scenarioName);
            InitializeView(grid_w, grid_h);
            InitializeComponent();
            InitializeController();

            InitializeOtherWindows();

            MinimumSize = new Size(100, 100 + SystemInformation.CaptionHeight + _menuStrip.Size.Height);
            this.Location = new Point(300, 0);
            this.StartPosition = FormStartPosition.Manual;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            this.drawingPanel.MouseDown += MainForm_MouseDown;
            this.drawingPanel.MouseMove += MainForm_MouseMove;
            this.drawingPanel.MouseUp += MainForm_MouseUp;
            this.drawingPanel.MouseWheel += MainForm_MouseWheel;
        }


        #region init

        /// <summary>
        /// Init Model by loading scenario and creating new main Probe.
        /// </summary>
        /// <param name="scenarioName">Name of the desired scenario.</param>
        private void InitializeModel(string scenarioName)
        {
            Scenario scenario = Scenario.LoadScenario(scenarioName);
            _particles = scenario.particles;
            _probe = new Probe();
        }

        /// <summary>
        /// Init View by creating Renderer with all particles and probe.
        /// </summary>
        private void InitializeView(int grid_w, int grid_h)
        {
            _renderer = new Renderer(_particles, _probe, this.ClientSize, grid_w, grid_h);
        }

        /// <summary>
        /// Init Controller by starting timer.
        /// </summary>
        private void InitializeController()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000 / _fps;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void InitializeOtherWindows()
        {
            CreateMenu();
        }

        #endregion init

        #region menu

        private void ShowStatsForm()
        {
            if (_statsForm == null || _statsForm.IsDisposed)
            {
                _statsForm = new StatsForm();

                _statsForm.Show();
            }
        }

        private void CreateMenu()
        {
            _menuStrip = new MenuStrip();
            this.MainMenuStrip = _menuStrip;
            this.Controls.Add(_menuStrip);

            // TOP-level
            ToolStripMenuItem stats = new ToolStripMenuItem("Stats");
            ToolStripMenuItem customizer = new ToolStripMenuItem("Customize");
            ToolStripMenuItem help = new ToolStripMenuItem("Helper");

            // HELP-submenu
            ToolStripMenuItem center = new ToolStripMenuItem("Center");
            ToolStripMenuItem scale_to_fit = new ToolStripMenuItem("Scale To Fit");
            help.DropDownItems.AddRange([center, scale_to_fit]);


            stats.Click += Click_stats;
            customizer.Click += Click_custom;

            center.Click += Click_center;
            scale_to_fit.Click += Click_scale_to_fit;

            _menuStrip.Items.Add(stats);
            _menuStrip.Items.Add(customizer);
            _menuStrip.Items.Add(help);
            
        }

        private void Click_scale_to_fit(object? sender, EventArgs e)
        {
            _renderer.ScaleToFill();
        }

        private void Click_center(object? sender, EventArgs e)
        {
            _renderer.CenterOrigin();
        }

        private void Click_custom(object? sender, EventArgs e)
        {
            _renderer.ShowCustomizerForm();
        }

        private void Click_stats(object? sender, EventArgs e)
        {
            ShowStatsForm();
        }

        private void UpdateProbeColor(Color newColor)
        {
            _probe.color = newColor;
            Invalidate(); // Redraw the arrow with the new color
        }

        public void UpdateStatsForm()
        {
            if (_statsForm == null)
            {
                return;
            }
            Vector2 probeDir = FieldCalculator.CalculateFieldDirection(_probe.position, _particles);

            _statsForm.UpdateProbeCoords(_probe.position);
            _statsForm.UpdateProbeDirection(probeDir);
            _statsForm.UpdateOriginPos(_renderer.Origin);
            _statsForm.UpdateZoom(_renderer.Scale);
        }

        #endregion menu

        #region time

        /// <summary>
        /// Update the frame every time it is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object? sender, EventArgs e)
        {
            float currentTime = Environment.TickCount / 1000f; // time in seconds
            float deltaTime = currentTime - _lastUpdateTime;
            _lastUpdateTime = currentTime;

            _timeElapsed += deltaTime;

            _probe.UpdatePosition(_timeElapsed);
            drawingPanel.Invalidate();
            
            UpdateStatsForm();
        }

        /// <summary>
        /// Paint and render.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            _renderer.Render(e.Graphics,this.ClientSize);
            
        }

        #endregion time

        #region interactivity

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this, e);
            Utils.HandleKeyboard(this,_renderer,e);
        }

        private Vector2 _map_position_before = Vector2.Zero;
        private bool _moving_map = false;
        private void MainForm_MouseDown(object? sender, MouseEventArgs e)
        {
            // if nothing else is hit TODO
            _moving_map = true;
            _map_position_before = new Vector2(e.X, e.Y);
        }
        private void MainForm_MouseUp(object? sender, MouseEventArgs e)
        {
            _moving_map = false;
        }

        private void MainForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (_moving_map)
            {
                Vector2 map_position_now = new Vector2(e.X, e.Y);
                Vector2 difference =  map_position_now - _map_position_before;
                _renderer.Origin = _renderer.Origin + difference;
                _map_position_before = map_position_now;
            }
        }
        private bool _zooming = false;
        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (!_zooming && e.Delta > 0)
            {
                _zooming = true;
                _renderer.Scale = _renderer.ZoomingFactor;
                _zooming = false;
            }else if (!_zooming && e.Delta < 0)
            {
                _zooming = true;
                _renderer.Scale = - 1 * _renderer.ZoomingFactor;
                _zooming = false;
            }
        }

        #endregion interactivity
    }
}
