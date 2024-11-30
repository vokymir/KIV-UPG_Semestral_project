using ElectricFieldVis.Model;
using ElectricFieldVis.View;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace ElectricFieldVis.Controller
{
    public partial class MainForm : Form
    {
        #region declarations

        private readonly float _timeflow_speed_LOWER_BOUND = 0.1f;
        private readonly float _timeflow_speed_UPPER_BOUND = 10f;

        private List<Particle> _particles;
        private Renderer _renderer;
        private StatsForm _statsForm;
        private LegendForm? _legendForm;
        private MenuStrip _menuStrip;
        private Particle? _moving_particle = null;
        private Probe _probe;
        private Probe? _moving_probe = null;
        private System.Windows.Forms.Timer _timer;
        private Vector2 _map_position_before = Vector2.Zero;
        private bool _funMode = false;
        private bool _moving_map = false;
        private bool _zooming = false;
        private float _lastUpdateTime = 0f;
        private float _timeElapsed = 0f;
        private float _timeflow_speed = 1.0f;
        private int _fps = 30;
        private bool[] static_probe_ids = new bool[360];
        public OtherProbesForm? opf;
        public string _scenarioName = "";

        public event Action OtherProbesChanged;

        #endregion declarations

        #region init

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

            // subscribe
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);
            this.drawingPanel.MouseDown += MainForm_MouseDown;
            this.drawingPanel.MouseMove += MainForm_MouseMove;
            this.drawingPanel.MouseUp += MainForm_MouseUp;
            this.drawingPanel.MouseWheel += MainForm_MouseWheel;
        }

        /// <summary>
        /// Init Model by loading scenario and creating new main Probe.
        /// </summary>
        /// <param name="scenarioName">Name of the desired scenario.</param>
        private void InitializeModel(string scenarioName)
        {
            Scenario scenario = Scenario.LoadScenario(scenarioName);
            _particles = scenario.particles;
            _probe = new Probe();
            _scenarioName = scenario.isDefault ? "0" : scenarioName;
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
            _lastUpdateTime = Environment.TickCount / 1000f;
            _timer.Start();
        }

        // Make other windows discoverable
        private void InitializeOtherWindows()
        {
            CreateMenu();
        }

        #endregion init

        #region menu

        private void CreateMenu()
        {
            _menuStrip = new MenuStrip();
            this.MainMenuStrip = _menuStrip;
            this.Controls.Add(_menuStrip);

            // TOP-level
            ToolStripMenuItem stats = new ToolStripMenuItem("Stats");
            ToolStripMenuItem customizer = new ToolStripMenuItem("Customize");
            ToolStripMenuItem help = new ToolStripMenuItem("Helper");
            ToolStripMenuItem scenario = new ToolStripMenuItem("Scenario");
            ToolStripMenuItem legend = new ToolStripMenuItem("Legend");
            ToolStripMenuItem time = new ToolStripMenuItem("Time");
            ToolStripMenuItem other_probes = new ToolStripMenuItem("Added probes");
            ToolStripMenuItem funMode_toggle = new ToolStripMenuItem("Fun Mode OFF");
            funMode_toggle.Name = "fun";

            // add visually to menu
            _menuStrip.Items.AddRange([/*stats, */customizer, help, scenario, legend, time, other_probes, funMode_toggle]);
            /* stats are only DEV info */

            // HELP-submenu
            ToolStripMenuItem center = new ToolStripMenuItem("Center");
            ToolStripMenuItem scale_to_fit = new ToolStripMenuItem("Scale To Fit");
            help.DropDownItems.AddRange([center, scale_to_fit]);

            // SCENARIO-submenu
            ToolStripMenuItem load = new ToolStripMenuItem("Load");
            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            scenario.DropDownItems.AddRange([load, save]);

            // TIME-submenu
            ToolStripMenuItem slowerTime = new ToolStripMenuItem("Slower");
            ToolStripMenuItem resetTime = new ToolStripMenuItem("Reset");
            ToolStripMenuItem fasterTime = new ToolStripMenuItem("Faster");
            ToolStripMenuItem customTime = new ToolStripMenuItem("Custom");
            time.DropDownItems.AddRange([slowerTime, resetTime, fasterTime, customTime]);

            // main menu
            stats.Click += Click_stats;
            customizer.Click += Click_customizer;
            legend.Click += Click_legend;
            other_probes.Click += Click_other_probes;
            funMode_toggle.Click += Click_funMode;

            // help submenu
            center.Click += Click_center;
            scale_to_fit.Click += Click_scale_to_fit;

            // scenario submenu
            load.Click += Click_load;
            save.Click += Click_save;

            // time submenu
            slowerTime.Click += Click_slowerTime;
            resetTime.Click += Click_resetTime;
            fasterTime.Click += Click_fasterTime;
            customTime.Click += Click_customTime;
        }

        #region click on menu items
        private void Click_funMode(object? sender, EventArgs e)
        {
            _funMode = !_funMode;
            //sender.Text = $"Fun Mode {(_funMode ? "ON" : "OFF")}";
            this._menuStrip.Items.Find("fun", false)[0].Text = $"Fun Mode {(_funMode ? "ON" : "OFF")}";
            //FunModeClicked?.Invoke(_funMode);
            this._renderer.funMode = _funMode;
        }

        private void Click_other_probes(object? sender, EventArgs e)
        {
            if (opf != null)
            {
                opf.Focus();
                opf.Activate();
                return;
            }
            opf = new OtherProbesForm(this, _renderer);
            opf.Show();
            OtherProbesChanged?.Invoke();
        }

        private void Click_customTime(object? sender, EventArgs e)
        {
            Point pt = new Point(this.Location.X + this.Width / 2 - 100, this.Location.Y + this.Height / 2);
            string input = InputBox.Show("Desired Timeflow Speed", pt);
            if (input != "")
            {
                SetTimeflowSpeed(input);
            }
        }

        private void Click_fasterTime(object? sender, EventArgs e)
        {
            SetTimeflowSpeed(2.0f);
        }

        private void Click_resetTime(object? sender, EventArgs e)
        {
            SetTimeflowSpeed("reset");
        }

        private void Click_slowerTime(object? sender, EventArgs e)
        {
            SetTimeflowSpeed(0.5f);
        }

        private void Click_legend(object? sender, EventArgs e)
        {
            if (_legendForm == null || _legendForm.IsDisposed)
            {
                _legendForm = new LegendForm(_renderer);

                _legendForm.StartPosition = FormStartPosition.CenterParent;
                _legendForm.Show();
            }

            _legendForm.Activate();
            _legendForm.Focus();
        }

        private void Click_load(object? sender, EventArgs e)
        { 
            var wid = 200;
            var hgt = 10;
            Point here = new Point(
                this.Left + (this.Width - wid) / 2,
                this.Top + (this.Height - hgt) / 2
            );
            var newScen = InputBox.Show("Which Scenario Should Load",here,_scenarioName,wid,hgt);
            
            // empty or esc
            if (newScen == "")
            {
                return;
            }

            InitializeModel(newScen);
            _renderer.particles = this._particles;
            _renderer.mainProbe = this._probe;
        }

        private void Click_save(object? sender, EventArgs e)
        {
            Scenario sc = new Scenario();
            sc.particles = this._particles;

            var wid = 200;
            var hgt = 10;
            Point here = new Point(
                this.Left + (this.Width - wid) / 2,
                this.Top + (this.Height - hgt) / 2
            );
            string name = InputBox.Show("Name this scenario", here);

            // empty or esc
            if (name == "")
            {
                return ;
            }

            bool res = Scenario.SaveScenario(sc, name);
            if (res)
            {
                _scenarioName = name;
            }
        }

        private void Click_scale_to_fit(object? sender, EventArgs e)
        {
            _renderer.ScaleToFill();
        }

        private void Click_center(object? sender, EventArgs e)
        {
            _renderer.CenterOrigin();
        }

        private void Click_customizer(object? sender, EventArgs e)
        {
            Point where = new Point(
                this.Left + 300,
                this.Top + 60
            );
            _renderer.ShowCustomizerForm(where);
        }

        private void Click_stats(object? sender, EventArgs e)
        {
            ShowStatsForm();
        }

        #endregion click on menu items

        private void ShowStatsForm()
        {
            if (_statsForm == null || _statsForm.IsDisposed)
            {
                _statsForm = new StatsForm();

                Point where = new Point(
                    this.Left,
                    this.Top + 60
                );
                _statsForm.StartPosition = FormStartPosition.Manual;
                _statsForm.Location = where;
                _statsForm.Show();
            }
            _statsForm.Activate();
            _statsForm.Focus();
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
        /// Update the frame every tick.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimerTick(object? sender, EventArgs e)
        {
            float currentTime = Environment.TickCount / 1000f; // time in seconds
            float deltaTime = (currentTime - _lastUpdateTime) * _timeflow_speed; // timeflow_speed for changing speed
            _lastUpdateTime = currentTime;

            _timeElapsed += deltaTime;

            _probe.UpdatePosition(_timeElapsed);
            UpdateParticleValues(_timeElapsed);
            drawingPanel.Invalidate();
            
            UpdateStatsForm();

            // Only update graph each milisecond, to have consistent X axis value.
            // miliseconds are the best, dont change it unless change graph X ax label in GraphForm.cs
            int second_divisor = 10;
            if (Math.Floor(currentTime * second_divisor) - Math.Floor(second_divisor * (currentTime - deltaTime)) >= 1)
            {
                foreach( (Probe, GraphForm) probeGraph in _renderer.otherProbes)
                {
                    probeGraph.Item2.UpdateGraph();
                }
            }
        }

        private void UpdateParticleValues(float timeElapsed)
        {
            foreach (Particle particle in _particles)
            {
                particle.setValue(timeElapsed);
            }
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

        // multiply speed by multiplier
        private void SetTimeflowSpeed(float multiplier)
        {
            if (multiplier == 0.0f)
            {
                SetTimeflowSpeed("reset");
                return;
            }

            float val = _timeflow_speed * multiplier;
            SetTimeflowSpeed(val, true);
        }

        // set custom timeflow
        private void SetTimeflowSpeed(string str)
        {
            if (str == "")
            {
                return;
            }
            if (str == "reset")
            {
                SetTimeflowSpeed(1, true);
                return;
            }

            bool parseRes = false;
            int num;
            float num2;
            parseRes = int.TryParse(str, out num);
            if (parseRes)
            {
                SetTimeflowSpeed((float)num, true);
                return;
            }
            parseRes = float.TryParse(str, out num2);
            if (parseRes)
            {
                SetTimeflowSpeed(num2, true);
                return;
            }
        }

        // I am sorry, but i already got the function
        private void SetTimeflowSpeed(float value, bool I_WANT_TO_SET_THIS_VALUE_not_multiply)
        {
            if (value <= _timeflow_speed_UPPER_BOUND
                && value >= _timeflow_speed_LOWER_BOUND)
            {
                _timeflow_speed = value;
            }
        }

        #endregion time

        #region interactivity

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Utils.HandleCtrlW(this, e);
        }
        
        private void MainForm_MouseDown(object? sender, MouseEventArgs e)
        {
            // right buttom is only for map moving
            if (e.Button == MouseButtons.Right)
            {
                _moving_map = true;
                _map_position_before = new Vector2(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Left)
            {
                // left button is multi-useful
                Particle? the_one = WasParticleClicked(e);
                Probe? the_probe = WasProbeClicked(e);
                if (the_probe != null)
                { // either click on probe (has prevalence)
                    HandleProbeOnClick(e, the_probe);
                }
                else if (the_one != null)
                { // or on particle
                    HandleParticleOnClick(e, the_one); 
                }
                else
                { // or elsewhere
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        CreateNewParticle(e);
                    }
                    else
                    {
                        CreateStaticProbe(e);
                    }
                }
            }

        }

        private void MainForm_MouseMove(object? sender, MouseEventArgs e)
        {
            // moving map when right button clicked
            if (_moving_map && e.Button == MouseButtons.Right)
            {
                Vector2 map_position_now = new Vector2(e.X, e.Y);
                Vector2 difference = map_position_now - _map_position_before;
                _renderer.Origin = _renderer.Origin + difference;
                _map_position_before = map_position_now;
            }
            // move particle if should
            if (_moving_particle != null)
            {
                if (e.X <= this.drawingPanel.Left || e.Y <= this.drawingPanel.Top ||
                    e.X >= this.drawingPanel.Right || e.Y >= this.drawingPanel.Bottom)
                {
                    return;
                }
                Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));
                _moving_particle.X = click.X;
                _moving_particle.Y = click.Y;
            }
            // move probe if should
            if (_moving_probe != null)
            {
                if (e.X <= this.drawingPanel.Left || e.Y <= this.drawingPanel.Top ||
                    e.X >= this.drawingPanel.Right || e.Y >= this.drawingPanel.Bottom)
                {
                    return;
                }
                Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));
                _moving_probe.position = click;
            }
        }

        // reset moving status to false
        private void MainForm_MouseUp(object? sender, MouseEventArgs e)
        {
            _moving_map = false;
            _moving_particle = null;
            _moving_probe = null;
        }

        // zooming
        private void MainForm_MouseWheel(object? sender, MouseEventArgs e)
        {
            // zooming with protection of speed-zooming
            if (!_zooming && e.Delta > 0)
            {
                _zooming = true;
                _renderer.Scale = _renderer.ZoomingFactor;
                _zooming = false;
            }
            else if (!_zooming && e.Delta < 0)
            {
                _zooming = true;
                _renderer.Scale = -1 * _renderer.ZoomingFactor;
                _zooming = false;
            }
        }

        // return clicked particle or null
        private Particle? WasParticleClicked(MouseEventArgs e)
        {
            Particle? the_clicked_one = null;
            Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));

            for (int i = 0; i < _particles.Count; i++)
            {

                Particle particle = _particles[i];
                float particle_radius = _renderer.CalculateParticleRadius(particle);

                if (click.X <= particle.X + particle_radius &&
                    click.X >= particle.X - particle_radius &&
                    click.Y <= particle.Y + particle_radius &&
                    click.Y >= particle.Y - particle_radius
                    )
                {
                    the_clicked_one = particle;
                    break;
                }
            }

            return the_clicked_one;
        }

        private Probe? WasProbeClicked(MouseEventArgs e)
        {
            Probe? the_clicked_one = null;
            Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));

            foreach ((Probe, GraphForm) pg in _renderer.otherProbes)
            {
                Probe probe = pg.Item1 as Probe;
                float probe_radius = 0.075f;

                if (click.X <= probe.position.X + probe_radius &&
                    click.X >= probe.position.X - probe_radius &&
                    click.Y <= probe.position.Y + probe_radius &&
                    click.Y >= probe.position.Y - probe_radius
                    )
                {
                    the_clicked_one = probe;
                    break;
                }
            }

            return the_clicked_one;
        }
        
        // return true if done something
        private bool HandleParticleOnClick(MouseEventArgs e, Particle the_clicked_one)
        {
            if (Form.ModifierKeys == Keys.Control)
            { // if intention to change value of particle
                Point here = this.PointToScreen(e.Location);
                string input = InputBox.Show("", here, the_clicked_one.Expression);

                // esc inputbox
                if (input == "")
                {
                    return true;
                }
                if (input.Contains('x'))
                {
                    DestroyParticle(the_clicked_one);
                    return true;
                }

                the_clicked_one.setExpression(input);
                return true;
            }

            // if intention to move particle
            _moving_particle = the_clicked_one;

            return true;
        }

        private void HandleProbeOnClick(MouseEventArgs e, Probe the_probe)
        {
            if (Form.ModifierKeys == Keys.Control)
            { // remove probe from _renderer and make color slot available
                _renderer.otherProbes.RemoveWhere(x =>
                {
                    if (x.Item1.ID == the_probe.ID)
                    {
                        x.Item2.Dispose(); // dispose GraphForm
                        static_probe_ids[x.Item1.ID] = false; // make color slot available
                        return true;       // remove
                    }
                    return false;          // keep 
                });

                OtherProbesChanged?.Invoke();
                return;
            }

            _moving_probe = the_probe;
        }

        private void CreateNewParticle(MouseEventArgs e)
        {
            Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));
            Point here = this.PointToScreen(e.Location);
            string input = InputBox.Show("Enter Particle Value:",here);

            // esc inputbox
            if (input == "")
            {
                return;
            }
            // destroy particle
            if (input.Contains('x'))
            {
                return;
            }

            Particle new_particle = new Particle();
            new_particle.trueInit();
            new_particle.X = click.X;
            new_particle.Y = click.Y;
            new_particle.setExpression(input);

            if (new_particle.Expression == "0")
            {
                return ;
            }

            this._particles.Add(new_particle);
            this._renderer.particles.Add(new_particle);
        }

        private void CreateStaticProbe(MouseEventArgs e)
        {
            Vector2 click = _renderer.GetRealWorldCoords(new Vector2(e.X, e.Y));
            int id = Array.FindIndex(static_probe_ids, val => !val);
            int color_count = 360;

            if (id >= color_count || id == -1)
            {
                return;
            }
            static_probe_ids[id] = true;

            double h = (id * 37) % color_count;
            double s = 1.0;
            double v = 1.0;

            Color clr = Utils.ColorFromHSV(h, s, v);

            Probe probe = new Probe(click, 0, clr);
            probe.ID = id;
            GraphForm graph = new GraphForm(probe, _renderer);

            _renderer.otherProbes.Add((probe, graph));
            OtherProbesChanged?.Invoke();
        }
        
        private void DestroyParticle(Particle the_clicked_one)
        {
            this._particles.Remove(the_clicked_one);
            this._renderer.particles.Remove(the_clicked_one);
        }

        #endregion interactivity
    }
}
