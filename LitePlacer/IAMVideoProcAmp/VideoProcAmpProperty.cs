// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2013
// contacts@aforgenet.com
//

namespace AForge.Video.DirectShow
{
    using System;

    /// <summary>
    /// The enumeration specifies a setting on a camera.
    /// </summary>
    public enum VideoProcAmpProperty
    {
        /// <summary>
        /// Brightness control.
        /// </summary>
        Brightness = 0,

        /// <summary>
        /// Contrast control.
        /// </summary>
        Contrast,

        /// <summary>
        /// Hue control.
        /// </summary>
        Hue,

        /// <summary>
        /// Saturation control.
        /// </summary>
        Saturation,

        /// <summary>
        /// Sharpness control.
        /// </summary>
        Sharpness,

        /// <summary>
        /// Gamma control.
        /// </summary>
        Gamma,

        /// <summary>
        /// ColorEnable control.
        /// </summary>
        ColorEnable,

        /// <summary>
        /// WhiteBalance control.
        /// </summary>
        WhiteBalance,

        /// <summary>
        /// BacklightCompensation control.
        /// </summary>
        BacklightCompensation,

        /// <summary>
        /// Gain control.
        /// </summary>
        Gain
    }

    /// <summary>
    /// The enumeration defines whether a camera setting is controlled manually or automatically.
    /// </summary>
    [Flags]
    public enum VideoProcAmpFlags
    {
        /// <summary>
        /// No control flag.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Auto control Flag.
        /// </summary>
        Auto = 0x0001,

        /// <summary>
        /// Manual control Flag.
        /// </summary>
        Manual = 0x0002
    }
}