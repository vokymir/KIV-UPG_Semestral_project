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
        public float Value { 
            get 
            { 
                return Value;
            }

            set 
            { 
                if (_expr == null)
                {
                    Value = value;
                }
                else
                {
                    Value = (float)_expr.Evaluate();
                }
            } 
        }
        public string Expression {
            get
            {
                return Expression;
            }
            set 
            { 
                if (_context == null)
                {
                    _context = new ExpressionContext();
                    _context.Variables["t"] = 0;
                }
                // predpokladam spravnost vstupu
                Expression = value;

                _expr = _context.CompileGeneric<double>(Expression);
                Value = (float)_expr.Evaluate();
            }
        }
        
        private ExpressionContext? _context;
        private IGenericExpression<double>? _expr;
        

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
        
        public void setValue(float time)
        {
            _context.Variables["t"] = time;
            Value = (float)(_expr.Evaluate());
        }
        public Particle() { Expression = ""; }
    }
}
