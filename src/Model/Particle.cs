using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Flee.PublicTypes;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// For storing all information about one Particle.
    /// </summary>
    public class Particle
    {
        // now I know I could use Vector2, but instead I have these two attributes X,Y
        public float X {  get; set; }
        public float Y { get; set; }
        public float Value {  get; set; }
        public string Expression {  get; set; }
        
        private ExpressionContext? _context;
        private IGenericExpression<double>? _expr;

        // for loading from Scenario purposes
        public Particle() { Expression = ""; }

        // for manual creation
        public Particle(float x, float y, float val, string exp )
        {
            X = x;
            Y = y;
            Expression = exp;
            Value = val;
            
            _context = new ExpressionContext();
            _context.Variables["t"] = 0;
            _expr = _context.CompileGeneric<double>(exp);
        }
        
        // for frame-based calculations of value
        public void setValue(float time)
        {
            if (_context == null)
            {
                _context = new ExpressionContext();
            }
            _context.Variables["t"] = time;
            
            // predpoklad OK vstupu do Expression
            if (_expr == null)
            {
                _expr = _context.CompileGeneric<double>(Expression);
            }

            Value = (float)(_expr.Evaluate());
        }

        public void trueInit()
        {
            if (Expression == "")
            {
                Expression = Value.ToString();
            }

            initContext();
            initExpr();
            setValue(0f);
        }

        public void initContext()
        {
            if (_context == null)
            {
                _context = new ExpressionContext();
            }

            _context.Variables["t"] = 0;
        }

        // predpoklada OK vstup & existenci _contextu
        public void initExpr()
        {
            if (_expr == null)
            {
                _expr = _context.CompileGeneric<double>(Expression);
            }
        }
    }
}
