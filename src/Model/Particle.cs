using System;
using System.Collections.Generic;
using System.Linq;
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

        public float _time = 0;
        private ExpressionContext _context;
        private IGenericExpression<double>? _expr;

        private string EXPRESSION = "1";

        private float _value = 0;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_expr == null)
                {
                    _expr = _context.CompileGeneric<double>(EXPRESSION);
                }
                _time = value;
                _context.Variables["t"] = _time;
                _value = (float)_expr.Evaluate();
            }
        }


        public Particle(float x, float y, string expression)
        {
            X = x;
            Y = y;
            EXPRESSION = expression;
            _context = new ExpressionContext();
            _expr = _context.CompileGeneric<double>(expression);
            
        }
        public Particle()
        {
            _context = new ExpressionContext();
        }
    }
}
