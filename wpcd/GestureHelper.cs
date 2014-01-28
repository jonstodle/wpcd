using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace wpcd {
    public static class GestureHelper {
        public enum Direction {
            Up, Down, Left, Right
        }

        public static Direction GetDirection(double x, double y) {
            Direction d;
            if(Math.Abs(x) >= Math.Abs(y)) {
                d = x >= 0 ? Direction.Right : Direction.Left;
            } else {
                d = y >= 0 ? Direction.Down : Direction.Up;
            }
            return d;
        }

        public static Orientation GetOrientation(double x, double y) {
            Orientation o;
            var d = GetDirection(x, y);
            if(d == Direction.Left || d == Direction.Right) {
                o = Orientation.Horizontal;
            } else {
                o = Orientation.Vertical;
            }
            return o;
        }
    }
}
