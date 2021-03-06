﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;


namespace MORT
{


    public partial class screenForm : Form
    {
        #region:::::::::::::::::::::::::::::::::::::::::::Form level declarations:::::::::::::::::::::::::::::::::::::::::::

        public static screenForm instance;
        public bool isMakeQuick = false;

        public bool LeftButtonDown = false;
        public bool RectangleDrawn = false;
        public bool ReadyToDrag = false;


        public Point ClickPoint = new Point();
        public Point CurrentTopLeft = new Point();
        public Point CurrentBottomRight = new Point();
        public Point DragClickRelative = new Point();

        public int RectangleHeight = new int();
        public int RectangleWidth = new int();

        Graphics g;
        Pen MyPen = new Pen(Color.Black, 1);
        SolidBrush TransparentBrush = new SolidBrush(Color.White);
        Pen EraserPen = new Pen(Color.FromArgb(255, 255, 192), 1);
        SolidBrush eraserBrush = new SolidBrush(Color.FromArgb(255, 255, 192));

        #endregion

        #region:::::::::::::::::::::::::::::::::::::::::::Mouse Event Handlers & Drawing Initialization:::::::::::::::::::::::::::::::::::::::::::
        public screenForm()
        {

            instance = this;
            InitializeComponent();
            this.MouseDown += new MouseEventHandler(mouse_Click);

            this.MouseMove += new MouseEventHandler(mouse_Move);

            this.Location = SystemInformation.VirtualScreen.Location;
            this.Size = SystemInformation.VirtualScreen.Size;
            g = this.CreateGraphics();

        }

        public screenForm(bool isQuick)
        {

            instance = this;
            InitializeComponent();
            this.MouseDown += new MouseEventHandler(mouse_Click);

            this.MouseMove += new MouseEventHandler(mouse_Move);

            this.Location = SystemInformation.VirtualScreen.Location;
            this.Size = SystemInformation.VirtualScreen.Size;
            g = this.CreateGraphics();

            isMakeQuick = isQuick;

        }
        #endregion


        public static void MakeScreenForm(bool isQuick)
        {
            if (screenForm.instance == null)
            {
                screenForm form = new screenForm(isQuick);
                form.Show();
            }
        }

        static public void makeOcrAreaForm(int newX, int newY, int newX2, int newY2, bool isShowFlag)
        {
            
            if (newY < 20)
            {
                newY = 20;
            }
            OcrAreaForm searchOptionForm = new OcrAreaForm();


            int BorderWidth = SystemInformation.FrameBorderSize.Width;
            int TitlebarHeight = SystemInformation.CaptionHeight + BorderWidth;

            searchOptionForm.StartPosition = FormStartPosition.Manual;
            searchOptionForm.Location = new Point(newX - BorderWidth, newY - TitlebarHeight);
            searchOptionForm.Size = new Size(newX2 + BorderWidth * 2, newY2 + TitlebarHeight + BorderWidth);
            searchOptionForm.Show();

            FormManager.Instace.AddOcrAreaForm(searchOptionForm);

            if (isShowFlag == false)
            {
                searchOptionForm.Opacity = 0;
            }

        }

        static public void MakeQuickOcrAreaForm(int newX, int newY, int newX2, int newY2)
        {
            if (newY < 20)
            {
                newY = 20;
            }

            OcrAreaForm searchOptionForm = null;
            if(FormManager.Instace.quickOcrAreaForm == null)
            {
                 searchOptionForm = new OcrAreaForm(true);
            }
            else
            {
                searchOptionForm = FormManager.Instace.quickOcrAreaForm;
            }



            int BorderWidth = SystemInformation.FrameBorderSize.Width;
            int TitlebarHeight = SystemInformation.CaptionHeight + BorderWidth;


            searchOptionForm.StartPosition = FormStartPosition.Manual;
            searchOptionForm.Location = new Point(newX - BorderWidth, newY - TitlebarHeight);
            searchOptionForm.Size = new Size(newX2 + BorderWidth * 2, newY2 + TitlebarHeight + BorderWidth);
            searchOptionForm.Show();

            FormManager.Instace.MakeQuickOcrAreaForm(searchOptionForm);

            searchOptionForm.Opacity = 0;
            FormManager.Instace.MyMainForm.setCaptureArea(); 

        }

        
        #region:::::::::::::::::::::::::::::::::::::::::::Mouse Buttons:::::::::::::::::::::::::::::::::::::::::::



        private void mouse_Up(object sender, MouseEventArgs e)
        {


            if (e.Button == MouseButtons.Left)
            {
                RectangleDrawn = true;
                LeftButtonDown = false;
                Point curPos = new Point(Cursor.Position.X, Cursor.Position.Y);

                if (ClickPoint.X > curPos.X)
                {
                    int temp = ClickPoint.X;
                    ClickPoint.X = curPos.X;
                    curPos.X = temp;
                }
                if (ClickPoint.Y > curPos.Y)
                {
                    int temp = ClickPoint.Y;
                    ClickPoint.Y = curPos.Y;
                    curPos.Y = temp;
                }
                if (curPos.X == ClickPoint.X)
                {
                    curPos.X++;
                }
                if (curPos.Y == ClickPoint.Y)
                {
                    curPos.Y++;
                }
                if(!isMakeQuick)
                {
                    makeOcrAreaForm(ClickPoint.X, ClickPoint.Y, curPos.X - ClickPoint.X, curPos.Y - ClickPoint.Y, true);
                }
                else
                {
                    MakeQuickOcrAreaForm(ClickPoint.X, ClickPoint.Y, curPos.X - ClickPoint.X, curPos.Y - ClickPoint.Y);
                }
               


            }

            this.Close();
        }
        private void mouse_Click(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                LeftButtonDown = true;
                ClickPoint = new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);

                if (RectangleDrawn)
                {

                    RectangleHeight = CurrentBottomRight.Y - CurrentTopLeft.Y;
                    RectangleWidth = CurrentBottomRight.X - CurrentTopLeft.X;
                    DragClickRelative.X = Cursor.Position.X - CurrentTopLeft.X;
                    DragClickRelative.Y = Cursor.Position.Y - CurrentTopLeft.Y;

                }
            }
        }


        #endregion



        private void mouse_Move(object sender, MouseEventArgs e)
        {
            if (LeftButtonDown && !RectangleDrawn)
            {
                DrawSelection();
            }
        }

        private void DrawSelection()
        {
            this.Cursor = Cursors.Arrow;

            //Erase the previous rectangle
            g.DrawRectangle(EraserPen, CurrentTopLeft.X - this.Location.X, CurrentTopLeft.Y -this.Location.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);

            //Calculate X Coordinates
            if (Cursor.Position.X < ClickPoint.X)
            {

                CurrentTopLeft.X = Cursor.Position.X;
                CurrentBottomRight.X = ClickPoint.X;

            }
            else
            {

                CurrentTopLeft.X = ClickPoint.X;
                CurrentBottomRight.X = Cursor.Position.X;

            }

            //Calculate Y Coordinates
            if (Cursor.Position.Y < ClickPoint.Y)
            {

                CurrentTopLeft.Y = Cursor.Position.Y;
                CurrentBottomRight.Y = ClickPoint.Y;

            }
            else
            {
                CurrentTopLeft.Y = ClickPoint.Y;
                CurrentBottomRight.Y = Cursor.Position.Y;
            }
          
            //Draw a new rectangle
            g.DrawRectangle(MyPen, CurrentTopLeft.X - this.Location.X, CurrentTopLeft.Y - this.Location.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);

        }


        #region ::::::::: 백업 :::::::::
        /*

        private void DragSelection()
        {
            //Ensure that the rectangle stays within the bounds of the screen

            //Erase the previous rectangle
            g.DrawRectangle(EraserPen, CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);

            if (Cursor.Position.X - DragClickRelative.X > 0 && Cursor.Position.X - DragClickRelative.X + RectangleWidth < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)
            {

                CurrentTopLeft.X = Cursor.Position.X - DragClickRelative.X;
                CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

            }
            else
                //Selection area has reached the right side of the screen
                if (Cursor.Position.X - DragClickRelative.X > 0)
                {

                    CurrentTopLeft.X = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - RectangleWidth;
                    CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

                }
                //Selection area has reached the left side of the screen
                else
                {

                    CurrentTopLeft.X = 0;
                    CurrentBottomRight.X = CurrentTopLeft.X + RectangleWidth;

                }

            if (Cursor.Position.Y - DragClickRelative.Y > 0 && Cursor.Position.Y - DragClickRelative.Y + RectangleHeight < System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
            {

                CurrentTopLeft.Y = Cursor.Position.Y - DragClickRelative.Y;
                CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

            }
            else
                //Selection area has reached the bottom of the screen
                if (Cursor.Position.Y - DragClickRelative.Y > 0)
                {

                    CurrentTopLeft.Y = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - RectangleHeight;
                    CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

                }
                //Selection area has reached the top of the screen
                else
                {

                    CurrentTopLeft.Y = 0;
                    CurrentBottomRight.Y = CurrentTopLeft.Y + RectangleHeight;

                }

            //Draw a new rectangle
            g.DrawRectangle(MyPen, CurrentTopLeft.X, CurrentTopLeft.Y, RectangleWidth, RectangleHeight);
        }

        private void DrawSelection()
        {

            this.Cursor = Cursors.Arrow;

            //Erase the previous rectangle
            g.DrawRectangle(EraserPen, CurrentTopLeft.X, CurrentTopLeft.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);

            //Calculate X Coordinates
            if (Cursor.Position.X < ClickPoint.X)
            {

                CurrentTopLeft.X = Cursor.Position.X;
                CurrentBottomRight.X = ClickPoint.X;

            }
            else
            {

                CurrentTopLeft.X = ClickPoint.X;
                CurrentBottomRight.X = Cursor.Position.X;

            }

            //Calculate Y Coordinates
            if (Cursor.Position.Y < ClickPoint.Y)
            {

                CurrentTopLeft.Y = Cursor.Position.Y;
                CurrentBottomRight.Y = ClickPoint.Y;

            }
            else
            {

                CurrentTopLeft.Y = ClickPoint.Y;
                CurrentBottomRight.Y = Cursor.Position.Y;

            }

            //Draw a new rectangle
            g.DrawRectangle(MyPen, CurrentTopLeft.X, CurrentTopLeft.Y, CurrentBottomRight.X - CurrentTopLeft.X, CurrentBottomRight.Y - CurrentTopLeft.Y);

        }
        */
        #endregion



        private void screenForm_Load(object sender, EventArgs e)
        {

        }

        private void screenForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            instance = null;
        }

    }
}