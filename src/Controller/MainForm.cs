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
        private int _fps = 30;

        public MainForm(int scenarioNumber = 0)
        {
            initializeModel(scenarioNumber);
            initializeView();
            InitializeComponent();
            initializeController();
        }

        private void initializeModel(int scenarioNumber)
        {
            _particles = Scenario.LoadScenario(scenarioNumber);
            _probe = new Probe();
        }

        private void initializeView()
        {
            _renderer = new Renderer(_particles, _probe);
        }

        private void initializeController()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000 / _fps;
            _timer.Tick += onTimerTick;
            _timer.Start();
        }

        private void onTimerTick(object sender, EventArgs e)
        {
            float deltaTime = 1f / _fps;
            _timeElapsed += deltaTime;

            _probe.UpdatePosition(_timeElapsed);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _renderer.Render(e.Graphics,this.ClientSize);
        }
    }
}
