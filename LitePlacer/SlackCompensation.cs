using System;

namespace LitePlacer {
    class SlackCompensation {
        float slack_xltor, slack_xrtol, slack_yttob, slack_ybtot;
        float slack_x, slack_y;
        double _x,_y;

        public SlackCompensation(float xltor, float xrtol, float yttob, float ybtot) {
            slack_xltor = xltor;
            slack_xrtol = xrtol;
            slack_ybtot = ybtot;
            slack_yttob = yttob;

            if (Math.Abs(slack_xltor - slack_xrtol) > .1) {
                Console.WriteLine("Warning - X slack is assymetric");
            }
            if (Math.Abs(slack_yttob - slack_ybtot) > .1) {
                Console.WriteLine("Warning - X slack is assymetric");
            }

            slack_x = (slack_xltor + slack_xrtol) / 2f;
            slack_y = (slack_yttob + slack_ybtot) / 2f;
            
            //last movement before this is turned on is a right upward movement, so no slack compensation
            //required if we move only up and to the right.  when we move left, we need to add the slack_x to the distance
            _x=slack_x;
            _y=slack_y;
        }

        public float X(float x) {
            var dx = x - _x;
            _x = x;
            if (dx < 0) return (x - slack_x);
            return x;
        }

        public float Y(float y) {
            var dy = y - _y;
            _y = y;
            if (y < 0) return (y - slack_y);
            return y;
        }
        

    }
}
