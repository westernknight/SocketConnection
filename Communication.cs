using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySurface
{
    [Serializable]
    public sealed class MyCustomStrokes
    {
        public MyCustomStrokes() { }

        /// <SUMMARY>
        /// The first index is for the stroke no.
        /// The second index is for the keep the 2D point of the Stroke.
        /// </SUMMARY>
        public MyPoint[][] StrokeCollection;
        public MyColor[] Color;
        public bool[] FitToCurve;
        public bool[] IgnorePressure;
        public bool[] IsHighlighter;

        public double[] Height;
        public double[] Width;


    }
    [Serializable]
    public class MyPoint
    {
        public float X;
        public float Y;
    }
    [Serializable]
    public sealed class MyColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

    }

}