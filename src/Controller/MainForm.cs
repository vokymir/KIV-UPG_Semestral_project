using ElectricFieldVis.Model;
using ElectricFieldVis.View;
using System.Drawing;
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

        /// <summary>
        /// Init MainForms components - Model, View, Controller and WinForm itself.
        /// </summary>
        /// <param name="scenarioName">Name of the desired scenario.</param>
        public MainForm(string scenarioName = "0")
        {
            InitializeModel(scenarioName);
            InitializeView();
            InitializeComponent();
            InitializeController();

            this.Size = new Size(800, 600);
            MinimumSize = new Size(100, 100 + SystemInformation.CaptionHeight);
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
        }

        /// <summary>
        /// Init View by creating Renderer with all particles and probe.
        /// </summary>
        private void InitializeView()
        {
            _renderer = new Renderer(_particles, _probe, this.ClientSize);
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
    }
}
