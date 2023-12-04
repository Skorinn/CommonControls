//*********************************************************************************************************************
// File Name:      ToggleButton.cs
// Description:    Implementation of the Toggle Button control
//
// Copyright (C) 2023 Mike Pullen. All Rights Reserved.
// Confidential and Proprietary
//
// Revision History: 
//====================================================================================================================
// 2023/12/02 - Mike Pullen - Original implementation.
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
    public partial class ToggleButton: CheckBox
    {
        #region Type Definitions

        // Drawing styles for the control
        public enum DrawingStyles
        {
            Solid,
            Hallow,
        }

        #endregion
        #region Contructors and Destructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ToggleButton()
        {
            // Minimum size required to ensure it is drawn correctly
            this.MinimumSize = new Size(50, 25);
        }

        #endregion
        #region Event Handlers

        /// <summary>
        /// Override for the paint event
        /// </summary>
        /// <param name="pevent">IN - The paint event arguments</param>
        protected override void OnPaint(PaintEventArgs paintEvent)
        {
            // Determine the height of the control
            int iHeight = this.Height - 5;

            // Set the smoothing mode and clear the control area
            paintEvent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            paintEvent.Graphics.Clear(this.Parent.BackColor);

            // Set the colors based on the state of the toggle
            SolidBrush backgroundBrush = null;
            SolidBrush toggleBrush = null;

            // If the control is disabled
            if (false == this.Enabled)
            {
                backgroundBrush = new SolidBrush(m_DisabledBackground);
                toggleBrush = new SolidBrush(m_DisabledToggle);
            }
            // Otherwise, if the toggle is ON
            else if (this.Checked)
            {
                // Use the ON colors
                backgroundBrush = new SolidBrush(m_OnBackground);
                toggleBrush = new SolidBrush(m_OnToggle);

            }
            // Otherwise, the toggle is OFF
            else
            {
                // Use the OFF colora
                backgroundBrush = new SolidBrush(m_OffBackground);
                toggleBrush = new SolidBrush(m_OffToggle);
            }

            // Get the paths for the background and toggle
            GraphicsPath backgroundPath = GetBackgroundPath();
            Rectangle toggleRectangle = GetToggleRectangle();

            // Draw the background based on the selected style
            if (DrawingStyles.Solid == m_Style)
            {
                // Fill the background
                paintEvent.Graphics.FillPath(backgroundBrush, backgroundPath);
            }
            else
            {
                // Only draw the outline for the background
                Pen backgroundPen = new Pen(backgroundBrush, 1);
                paintEvent.Graphics.DrawPath(backgroundPen, backgroundPath);
            }
            
            // Draw the toggle
            paintEvent.Graphics.FillEllipse(toggleBrush, toggleRectangle);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Creates the path for the control
        /// </summary>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// Background color when the toggle is in the ON state
        /// </summary>
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
        /// Toggle color when the toggle is in the ON state
        /// </summary>
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
