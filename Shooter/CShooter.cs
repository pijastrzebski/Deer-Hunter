using Shooter.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shooter
{
    class CShooter: CImageBase
    {
        private Rectangle _deerHotSpot = new Rectangle();
        public CShooter() : base(Resources.deer)
        {
            _deerHotSpot.X = Left;
            _deerHotSpot.Y = Top;
            _deerHotSpot.Width = 70;
            _deerHotSpot.Height = 70;

        }

        public void Update(int X, int Y)
        {
            Left = X;
            Top = Y;
            _deerHotSpot.X = Left;
            _deerHotSpot.Y = Top;

        }

        public bool Hit (int X, int Y)
        {
            Rectangle c = new Rectangle(X, Y, 1, 1);

            if (_deerHotSpot.Contains(c))
            {
                return true;
            }

            return false;
        }

    }
}
