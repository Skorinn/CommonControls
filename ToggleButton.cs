//*********************************************************************************************************************
// File Name:      ToggleButton.cs
// Description:    Implementation of the Toggle Button control
//
// Copyright (C) 2023 Mike Pullen.
// Licensed under the MIT License. See the LICENSE file in the repository root.
//
// Revision History: 
//====================================================================================================================
// 2023/12/02 - Mike Pullen - Original implementation.
// 2026/08/31 - Mike Pullen - Corrected spelling of the DrawingStyles.Hollow enumerator.
// 2026/08/31 - Mike Pullen - Released under the MIT License.
// 2026/08/31 - Mike Pullen - Disposed of the GDI+ drawing resources, opted in to resize repainting and
//                            double buffering, and added designer metadata to the properties.
//*********************************************************************************************************************
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CommonControls
{
    /// <summary>
    /// Toggle Button control
    /// </summary>
    public class ToggleButton: CheckBox
    {
        #region Type Definitions

        /// <summary>
        /// Drawing styles for the control
        /// </summary>
        public enum DrawingStyles
        {
            /// <summary>
            /// The background of the control is filled
            /// </summary>
            Solid,

            /// <summary>
            /// Only the outline of the background is drawn
            /// </summary>
            Hollow,
        }

        #endregion
        #region Contructors and Destructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ToggleButton()
        {
            // Repaint the whole surface on resize, and draw off screen, so resizing leaves no artifacts and does not flicker
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Minimum size required to ensure it is drawn correctly
            this.MinimumSize = new Size(50, 25);
        }

        #endregion
        #region Event Handlers

        /// <summary>
        /// Override for the paint event
        /// </summary>
        /// <param name="paintEvent">IN - The paint event arguments</param>
        protected override void OnPaint(PaintEventArgs paintEvent)
        {
            // Set the smoothing mode and clear the control area
            paintEvent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            paintEvent.Graphics.Clear(this.Parent.BackColor);

            // Set the colors based on the state of the toggle
            Color backgroundColor;
            Color toggleColor;

            // If the control is disabled
            if (false == this.Enabled)
            {
                // Use the disabled colors
                backgroundColor = m_DisabledBackground;
                toggleColor = m_DisabledToggle;
            }
            // Otherwise, if the toggle is ON
            else if (this.Checked)
            {
                // Use the ON colors
                backgroundColor = m_OnBackground;
                toggleColor = m_OnToggle;
            }
            // Otherwise, the toggle is OFF
            else
            {
                // Use the OFF colors
                backgroundColor = m_OffBackground;
                toggleColor = m_OffToggle;
            }

            // Get the rectangle for the toggle
            Rectangle toggleRectangle = GetToggleRectangle();

            // Create the drawing resources, disposing of them once the control has been drawn
            using (GraphicsPath backgroundPath = GetBackgroundPath())
            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            using (SolidBrush toggleBrush = new SolidBrush(toggleColor))
            {
                // Draw the background based on the selected style
                if (DrawingStyles.Solid == m_Style)
                {
                    // Fill the background
                    paintEvent.Graphics.FillPath(backgroundBrush, backgroundPath);
                }
                else
                {
                    // Only draw the outline for the background
                    using (Pen backgroundPen = new Pen(backgroundBrush, 1))
                    {
                        paintEvent.Graphics.DrawPath(backgroundPen, backgroundPath);
                    }
                }

                // Draw the toggle
                paintEvent.Graphics.FillEllipse(toggleBrush, toggleRectangle);
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Creates the path for the control
        /// </summary>
        /// <returns>The rounded path used to draw the background</returns>
        private GraphicsPath GetBackgroundPath()
        {
            // Determine what will be the radius of the toggle
            int iToggleRadius = this.Height - 1;

            // Create the left arc
            Point LeftArcStart = new Point(0, 0);
            Size LeftArcSize = new Size(iToggleRadius, iToggleRadius);
            Rectangle LeftArc = new Rectangle(LeftArcStart, LeftArcSize);

            // Create the right arc
            Point RightArcStart = new Point(this.Width-iToggleRadius-2, 0); // -2 = 1 for each side
            Size RightArcSize = LeftArcSize;
            Rectangle RightArc = new Rectangle(RightArcStart, RightArcSize);

            // Create the path for the toggle
            GraphicsPath TogglePath = new GraphicsPath();
            TogglePath.StartFigure();
            TogglePath.AddArc(LeftArc, 90, 180);
            TogglePath.AddArc(RightArc, 270, 180);
            TogglePath.CloseFigure();

            return TogglePath;
        }

        /// <summary>
        /// Gets the rectangle for the toggle
        /// </summary>
        /// <returns>The bounding rectangle of the toggle for the current state</returns>
        private Rectangle GetToggleRectangle()
        {
            // Set the position based on the toggle state
            Point toggleStart = new Point(0, 2);
            
            // If the toggle is enabled
            if (this.Checked)
            {
                // Set the position to the right
                toggleStart.X = this.Width - this.Height + 1;
            }
            // Otherwise, the toggle is disabled
            else
            {
                // Set the position to the left
                toggleStart.X = 2;
            }

            // Set the size of the toggle
            int iToggleDiameter = this.Height - 5;
            Size toggleSize = new Size(iToggleDiameter, iToggleDiameter);

            // Create and return the rectangle
            Rectangle toggleRectangle = new Rectangle(toggleStart, toggleSize);
            return toggleRectangle;

        }

        #endregion
        #region Properties

        /// <summary>
        /// Background color when the toggle is in the OFF state
        /// </summary>
        [Category("Appearance")]
        [Description("Background color when the toggle is in the OFF state")]
        [DefaultValue(typeof(Color), "Black")]
        public Color OffBackground
        {
            get => m_OffBackground;
            set
            {
                m_OffBackground = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Toggle color when the toggle is in the OFF state
        /// </summary>
        [Category("Appearance")]
        [Description("Toggle color when the toggle is in the OFF state")]
        [DefaultValue(typeof(Color), "White")]
        public Color OffToggle
        {
            get => m_OffToggle;
            set
            {
                m_OffToggle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Background color when the toggle is in the ON state
        /// </summary>
        [Category("Appearance")]
        [Description("Background color when the toggle is in the ON state")]
        [DefaultValue(typeof(Color), "Black")]
        public Color OnBackground
        {
            get => m_OnBackground;
            set
            {
                m_OnBackground = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Toggle color when the toggle is in the ON state
        /// </summary>
        [Category("Appearance")]
        [Description("Toggle color when the toggle is in the ON state")]
        [DefaultValue(typeof(Color), "White")]
        public Color OnToggle
        {
            get => m_OnToggle;
            set
            {
                m_OnToggle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Background color when the control is disabled
        /// </summary>
        [Category("Appearance")]
        [Description("Background color when the control is disabled")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color DisabledBackground
        {
            get => m_DisabledBackground;
            set
            {
                m_DisabledBackground = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Toggle color when the control is disabled
        /// </summary>
        [Category("Appearance")]
        [Description("Toggle color when the control is disabled")]
        [DefaultValue(typeof(Color), "LightGray")]
        public Color DisabledToggle
        {
            get => m_DisabledToggle;
            set
            {
                m_DisabledToggle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Drawing style for the control
        /// </summary>
        [Category("Appearance")]
        [Description("Drawing style for the control")]
        [DefaultValue(DrawingStyles.Solid)]
        public DrawingStyles Style
        {
            get => m_Style;
            set
            {
                m_Style = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Override for the Text property to remove set
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text { get => base.Text; }

        #endregion
        #region Data Members

        // Button colors
        private Color m_OffBackground = Color.Black;
        private Color m_OffToggle = Color.White;
        private Color m_OnBackground = Color.Black;
        private Color m_OnToggle = Color.White;
        private Color m_DisabledBackground = Color.Gray;
        private Color m_DisabledToggle = Color.LightGray;

        // Drawing style
        private DrawingStyles m_Style = DrawingStyles.Solid;

        #endregion
    }
}
