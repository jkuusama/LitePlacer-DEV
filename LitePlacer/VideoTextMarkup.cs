using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LitePlacer {
    public class VideoTextMarkup {
        public Shapes.Thing location;
        public string text = "";
        public Color color = Color.Blue;
        public Font font = new Font("Tahoma",20);

        public VideoTextMarkup(Camera cam, PartLocation location, string text) {
            this.location = new Shapes.Thing(location, Shapes.PointMode.MM);
            this.location.camera = cam;
            this.text = text;
        }


        public void Draw(ref Bitmap image) {
          //  try {
                // convert location to screen location
              //  var thing = location.Clone().ToRawResolution();
             //   var startText = thing.ToPartLocation() + new PartLocation(30, 30);
              //  Graphics g = Graphics.FromImage(image);
               
                Pen pen = new Pen(Color.Red, 1);
                Graphics g = Graphics.FromImage(image);
                g.DrawLine(pen, 100, 100, 200, 200);
               


             //   var p2 = startText.ToPoint();
             //   g.DrawString(text, font, Brushes.Black, p2);
             //   g.Flush();
          //  } catch (Exception e) { 
           //     Console.WriteLine("VideoTextMarkup: Error:"+e);
          //  }
        }


    }
}
