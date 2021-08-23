
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Collections.Generic;

namespace GrafPack
{
    public partial class Form1 : Form
    {
        ContextMenu transformOptions = new ContextMenu(); // the menu for a shape that appears after right click

        TextBox moveY = new TextBox(); // These will take the input from user in 'Move by Input' option
        TextBox moveX = new TextBox();
        Form InputMoveBox = new Form();

        TextBox RotateInputBox = new TextBox(); // This will take a rotation value (in degrees)

        private bool selectSquareStatus = false; //  set to true when user selected Square in Create
        private bool selectTriangleStatus = false; //  set to true when user selected Triangle in Create
        private bool selectCircleStatus = false;  //  set to true when user selected Circle in Create
        private bool selectHexagonStatus = false; //  set to true when user selected Hexagon in Create
        private bool selectLineStatus = false; //  set to true when user selected Line in Create
        private bool shapeSelection = false; // set to true when user chooses select option, is checked in keyDown
        private bool mouseHeld = false; // set true after 1st click in create, facilitates Rubber banding in mouseMove
        private int shapeIndex = 0;
        private int resizeIndex = 0;  // taking the index from list of the shape which is being re-sized
        private int moveByDragIndex = 0; // taking the index from list of the shape which is being moved by drag
        private int moveByInputIndex = 0; // taking the index from list of the shape which is being moved by input
        private int rotationIndex = 0; // taking the index from list of the shape which is selected for rotation
        private int reflectIndex = 0; // taking the index from list of the shape which is being reflected

        private int xMove; // the input taken from user in 'move by input'
        private int yMove; // the input taken from user in 'move by input'
        private int rotationValue; //  taken as input for custom rotation 
        private int diffFromCentre; // used when reflect has been selected, calculates how far shape is from centre
        private const int MID_SCREEN_X = 700; // is used for calculating reflect
        private const int MID_SCREEN_Y = 400; // is user for calculating reflect
        private bool moveByDrag = false; // set to true when 'move by mouse drag' has been clicked
        private bool moveByInput = false; // set to true when user selects 'move by input'
        private bool resizeByDrag = false; // set to true when user clicks 'resize by mouse'
        private bool rotateBy90 = false; //  set to true when 90 degree rotation option has been clicked
        private bool rotateBy180 = false; // set to true when 180 degree rotation option has been clicked
        private bool reflectXmode = false; // set to true when user selects 'Reflect X axis'
        private bool reflectYmode = false; // set to true when user selects 'Reflect Y axis'
        private bool customRotateMode = false; // set to true when 'custom rotate' has been clicked
        
        private static List<Shape> shape = new List<Shape>(); // This will store the shapes

        private int clicknumber = 0; // used for creating shapes and knowing when to draw to screen
        private Point one;  // the first click from user
        private Point two;  // the second click from user
        private Point draggedMouseLocation; // mainly used for 'resize by drag' and 'move by drag'
        private Point newPos = new Point(); // is used for 'move by drag', new position of a point.
        private Point rotatedKeyPt = new Point(); //  the new calculated key point after rotation applied
        private Point rotatedOppPt = new Point(); //  the new calculated opp point after rotation applied
        private int differenceX = 0; // used in 'move by drag', to ensure that shape maintains its dimensions
        private int differenceY = 0; // used in 'move by drag', to ensure that shape maintains its dimensions

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            TextBox moveByInputBox = new TextBox();
            moveByInputBox.Text = "0";

            // The following approach uses menu items coupled with mouse clicks. The main options
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem();
            MenuItem selectItem = new MenuItem();
            MenuItem exitProgram = new MenuItem();

            // the shape creation options
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();
            MenuItem hexagonItem = new MenuItem();
            MenuItem lineItem = new MenuItem();

            // the options for selected shape
            MenuItem moveItem = new MenuItem();
            MenuItem rotateItem = new MenuItem();
            MenuItem resizeItem = new MenuItem();
            MenuItem deleteItem = new MenuItem();
            MenuItem reflect = new MenuItem();

            // the sub options for selected shape
            MenuItem moveByDrag = new MenuItem();
            MenuItem moveByInput = new MenuItem();
            MenuItem rotate90 = new MenuItem();
            MenuItem rotate180 = new MenuItem();
            MenuItem customRotate = new MenuItem();
            MenuItem resizeByMouse = new MenuItem();
            MenuItem reflectX = new MenuItem();
            MenuItem reflectY = new MenuItem();
            
            // The text on the buttons
            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            hexagonItem.Text = "&Hexagon";
            lineItem.Text = "&Line";
            selectItem.Text = "&Select";
            exitProgram.Text = "&Exit";

            moveItem.Text = "&Move";
            rotateItem.Text = "&Rotate";
            deleteItem.Text = "&Delete";
            resizeItem.Text = "&Resize";
            reflect.Text = "&Reflect";
            rotate90.Text = "&Rotate by 90";
            rotate180.Text = "&Rotate by 180";
            customRotate.Text = "&Custom Rotate";
            reflectX.Text = "&Reflect X axis";
            reflectY.Text = "&Reflect Y axis";

            moveByDrag.Text = "&Move by mouse drag";
            moveByInput.Text = "&Move by input";
            resizeByMouse.Text = "&Resize by mouse";

            // the main options on top menu bar
            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem); 
            mainMenu.MenuItems.Add(exitProgram);

            // adding the sub-options for create 
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            createItem.MenuItems.Add(hexagonItem);
            createItem.MenuItems.Add(lineItem);

            // adding the options for context menu
            transformOptions.MenuItems.Add(moveItem);
            transformOptions.MenuItems.Add(rotateItem);
            transformOptions.MenuItems.Add(resizeItem);
            transformOptions.MenuItems.Add(deleteItem);
            transformOptions.MenuItems.Add(reflect);

            // the sub options for the context menu
            rotateItem.MenuItems.Add(rotate90);
            rotateItem.MenuItems.Add(rotate180);
            rotateItem.MenuItems.Add(customRotate);
            moveItem.MenuItems.Add(moveByDrag);
            moveItem.MenuItems.Add(moveByInput);
            resizeItem.MenuItems.Add(resizeByMouse);
            reflect.MenuItems.Add(reflectX);
            reflect.MenuItems.Add(reflectY);
        
            // Adding a click event to the buttons, which will invoke a method
            selectItem.Click += new System.EventHandler(this.selectShape);
            squareItem.Click += new System.EventHandler(this.selectSquare);
            triangleItem.Click += new System.EventHandler(this.selectTriangle);
            circleItem.Click += new System.EventHandler(this.selectCircle);
            hexagonItem.Click += new System.EventHandler(this.selectHexagon);
            lineItem.Click += new System.EventHandler(this.selectLine);
            exitProgram.Click += new System.EventHandler(this.ExitProgram);
            rotate90.Click += new System.EventHandler(this.RotateAt90);
            rotate180.Click += new System.EventHandler(this.RotateAt180);
            customRotate.Click += new System.EventHandler(this.CustomRotateBox);
            reflectX.Click += new System.EventHandler(this.ReflectX);
            reflectY.Click += new System.EventHandler(this.ReflectY);
            moveByInput.Click += new System.EventHandler(this.MoveByInput);
            moveByDrag.Click += new System.EventHandler(this.MoveByDrag);
            resizeByMouse.Click += new System.EventHandler(this.ResizeByMouse);
            deleteItem.Click += new System.EventHandler(this.deleteShape);
             
            this.Menu = mainMenu;
            this.ContextMenu = transformOptions;
            this.MouseClick += mouseClick; // This is used to create shapes and to resize and move by drag
            this.MouseDown += mouseDown; // will set mouseheld to true ( if left click held down), for rubber banding
            this.MouseUp += mouseUp;    // will set mouseheld to false once left click released
            this.MouseMove += mouseMove; // used for rubber banding and resizing and moving by drag
            this.KeyDown += keyDown; // used to scroll between the shapes in select mode
            
        } // end of Form1 constructor

        private void refreshScreen()
        {
            Invalidate();
            Refresh();
            redrawShapes();
        }

        // Generally, all methods of the form are usually private
        private void selectSquare(object sender, EventArgs e) // Triggered when Create option, 'Square' is selected.
        {
            selectSquareStatus = true;
            MessageBox.Show("Click OK and then click once each at two locations to create a square");
            selectTriangleStatus = false; // Set other options to false to ensure only 1 create option selected.
            selectCircleStatus = false;
            selectHexagonStatus = false;
            selectLineStatus = false;
        }

        private void selectTriangle(object sender, EventArgs e) // Triggered when Create option, 'Triangle' selected.
        {
            selectTriangleStatus = true;
            MessageBox.Show("Click OK and then click once at two locations to create a triangle");
            selectSquareStatus = false; // Set other options to false to ensure only 1 create option selected.
            selectCircleStatus = false;
            selectHexagonStatus = false;
            selectLineStatus = false;
        }

        private void selectCircle(object sender, EventArgs e) // Triggered when Create option, 'Circle' selected.
        {
            selectCircleStatus = true;
            MessageBox.Show("Click OK and then click a center point followed by an upper area to create a circle");
            selectSquareStatus = false; // Set other options to false to ensure only 1 create option selected at a time
            selectTriangleStatus = false;
            selectHexagonStatus = false;
            selectLineStatus = false;
        }

        private void selectHexagon(object sender, EventArgs e) // Triggered when Create option, 'Hexagon' selected.
        {
            selectHexagonStatus = true;
            MessageBox.Show("Click OK and then click once at two locations to create a hexagon");
            selectSquareStatus = false; // Set other options to false to ensure only 1 create option selected.
            selectTriangleStatus = false;
            selectCircleStatus = false;
            selectLineStatus = false;
        }

        private void selectLine(object sender, EventArgs e) // Triggered when Create option, 'Line' selected
        {
            selectLineStatus = true;
            MessageBox.Show("Click OK and then click once at two locations to create a line");
            selectSquareStatus = false; // Set other options to false to ensure only 1 create option selected.
            selectTriangleStatus = false;
            selectCircleStatus = false;
            selectHexagonStatus = false;
        }

        private void selectShape(object sender, EventArgs e) // Is triggered when Menu option 'Select' clicked
        {
            shapeSelection = true; // setting to true, this will enable user to select different shapes
            MessageBox.Show("You selected the Select option, Press the left or right key to select a shape");
            Selection(); // this will draw the currently selected shape in red
        }

        
        private void ReflectX(object sender, EventArgs e)
        {          
            Pen blackpen = new Pen(Color.Black); 
            reflectXmode = true;
            reflectIndex = shapeIndex; // getting the index of the selected shape
            shapeIdentify(shapeIndex, blackpen); // the reflection will be applied via shapeIdentify

            refreshScreen();
        }

        private void ReflectY(object sender, EventArgs e)
        {
            Pen blackpen = new Pen(Color.Black);
            reflectYmode = true;
            reflectIndex = shapeIndex; // getting the index of the selected shape
            shapeIdentify(shapeIndex, blackpen); // the reflection will be applied via shapeIdentify

            refreshScreen();
        }

        private void MoveByDrag(object sender, EventArgs e) // Is triggered when Move By Drag sub-option clicked
        {
            moveByDrag = true;
        }

        private void ExitProgram(object sender, EventArgs e) // Is triggered when the Exit menu button clicked
        {
            MessageBox.Show("Exiting program, Bye!");
            Application.Exit();
        }

        private void Selection() // This method is called when 'Select' is clicked and will draw selected shape in red
        {
            Pen selectionPen = new Pen(Color.Red); // The selected image will appear in red on the screen.

            if (shape.Count == 0) // if there are no shapes in the shape list.
            {
                MessageBox.Show("There are no shapes to choose from");
                shapeSelection = false;
            }
            else
            {
                shapeIdentify(shapeIndex, selectionPen);
            }

        }

        private Point RotatePt(float angle, Point Pt, Point center)
        { 
            float cosa = (float)Math.Cos(angle * Math.PI / 180.0);
            float sina = (float)Math.Sin(angle * Math.PI / 180.0); 
             
            // calculating new x and y values
            float X = (cosa * (Pt.X - center.X) - sina * (Pt.Y - center.Y) + center.X);
            float Y = (sina * (Pt.X - center.X) + cosa * (Pt.Y - center.Y) + center.Y);
          
            Pt = new Point((int)X, (int)Y);

            return Pt;
        }

        private void RotateAt90(object sender, EventArgs e)
        {           
            Pen blackpen = new Pen(Color.Black);
            rotateBy90 = true;
            rotationIndex = shapeIndex; // getting the index of the currently selected shape
            shapeIdentify(shapeIndex, blackpen); // rotation will be applied via shapeIdentify

            refreshScreen();
        }

        private void RotateAt180(object sender, EventArgs e)
        {
            Pen blackpen = new Pen(Color.Black);
            rotateBy180 = true;
            rotationIndex = shapeIndex; // getting the index of the currently selected shape
            shapeIdentify(shapeIndex, blackpen); // rotation will be applied via shapeIdentify

            refreshScreen();
        }
        private void CustomRotateBox(object sender, EventArgs e) // The box where user enters rotation value 
        {
            Form rotateBox = new Form();
            Pen blackpen = new Pen(Color.Black);
            Button confirmation = new Button();
            Label text = new Label();
            text.Text = "Clockwise rotation";
            text.Location = new Point(60, 30);
            confirmation.Text = "Confirm";
            confirmation.Location = new Point(75, 75);
            confirmation.BackColor = Color.White;
            
            rotateBox.Size = new Size(200, 150);
            rotateBox.Location = draggedMouseLocation;        
            RotateInputBox.Location = new Point(60, 50);
            rotateBox.Controls.Add(RotateInputBox);
            RotateInputBox.Text = "0";
            rotateBox.Controls.Add(text);  
            rotateBox.Controls.Add(confirmation);
            rotateBox.StartPosition = FormStartPosition.CenterScreen;

            if (rotateBox.IsDisposed == false || rotateBox != null)
            {
                rotateBox.Show();
                confirmation.Click += new System.EventHandler(this.CustomRotate); // invoking CustomRotate method
            }
           
        }

        private void CustomRotate(object sender, EventArgs e) // This processes rotation and checks valid input
        {
            Pen blackpen = new Pen(Color.Black);
            string rot;
            int number;
            rot = RotateInputBox.Text.ToString();
            bool isNumber =  int.TryParse(rot, out number); // used to check for numerical input
             
            if(isNumber == false) // User entered blank or a string
            {
                MessageBox.Show("You entered a string, please enter a number");               
            }
            else  // User entered a number
            {
                rotationValue = int.Parse(rot); 
                customRotateMode = true;
                rotationIndex = shapeIndex; // getting the index of the currently selected shape
                shapeIdentify(shapeIndex, blackpen); // rotation will be applied via shapeIdentify

                refreshScreen();
            }
              
        }

        private void ResizeByMouse(object sender, EventArgs e) // enables for shape to be resized in mouseMove
        {
            resizeByDrag = true;
        }

        private void MoveByInput(object sender, EventArgs e) // Appears when move by input selected
        {
            Button confirm = new Button();
            confirm.Text = "Confirm";
            confirm.Location = new Point(80, 105);
            confirm.BackColor = Color.White;
            InputMoveBox.Controls.Add(confirm);

            if (InputMoveBox.IsDisposed == true)
            {
                InputMoveBox = new Form();
                MoveByInputBox();
                InputMoveBox.Show();
            }
            else
            {
                MoveByInputBox();
                InputMoveBox.Show();
            }
       
            confirm.Click += new System.EventHandler(this.confirmMove); // will trigger confirmMove function
        
        } // End of moveByInput method

        private void MoveByInputBox()
        {
            moveX.BorderStyle = BorderStyle.Fixed3D;
            moveX.Location = new Point(100, 50);

            Label xLabel = new Label();
            xLabel.Text = "X Axis:";
            xLabel.Location = new Point(60, 55);

            moveY.Text = "0";
            moveY.Width = 50;
            moveY.Height = 10;
            moveY.BackColor = Color.White;
            moveY.BorderStyle = BorderStyle.Fixed3D;
            moveY.Location = new Point(100, 80);
            Label yLabel = new Label();
            yLabel.Text = "Y Axis:";
            yLabel.Location = new Point(60, 80);

            InputMoveBox.Size = new Size(300, 200);
            InputMoveBox.StartPosition = FormStartPosition.CenterScreen;
            InputMoveBox.Controls.Add(moveX);
            InputMoveBox.Controls.Add(xLabel);
            InputMoveBox.Controls.Add(moveY);
            InputMoveBox.Controls.Add(yLabel);
           
            InputMoveBox.Text = "Move by input";

        }


        private void confirmMove(object sender, EventArgs e)
        {

            Pen blackPen = new Pen(Color.Black);
            string inputX;
            string inputY;
            inputX = moveX.Text.ToString(); // The value entered by the user in the X field
            inputY = moveY.Text.ToString(); // The value entered by the user in the Y field
            int number;
            bool isNumberX = int.TryParse(inputX, out number); // to check to make sure input not string/blank
            bool isNumberY = int.TryParse(inputY, out number);

            if (isNumberX == false || isNumberY == false) // One or both the fields did not contain a number
            {
                MessageBox.Show("Invalid input, please enter a number");
            }
            else // user entered a number
            {
                xMove = int.Parse(inputX);  // Converting the input string value into an int
                yMove = int.Parse(inputY);  // Converting the input string value into an int
                moveByInputIndex = shapeIndex;
                moveByInput = true;
                shapeIdentify(shapeIndex, blackPen);
                moveByInput = false;
                refreshScreen();
                MessageBox.Show("Moved!");
                              
            }

        } // end of confirmMove method


        private void deleteShape(object sender, EventArgs e) // deletes shape from memory and updates the screen
        {
            shape.RemoveAt(shapeIndex);
            refreshScreen();
        }

        // this method helps to facilitate refreshing the screen, but also the rubber banding once the screen has  
        // been cleared. It will go through all the shapes in list and re-draw them by calling shapeIdentify().
        private void redrawShapes() 
        {                      
            Pen blackpen = new Pen(Color.Black);

            for (int i = 0; i < shape.Count; i++)
            {          
                shapeIdentify(i, blackpen);
            }

        }

        // This method identifies the type of shape at a given index position in shape list and will re-draw accordingly
        // taking into account the currently selected user option ( e.g. move by Input). 
        private void shapeIdentify(int index, Pen pen)
        {                                              
            Type t = shape[index].GetType();
            Graphics g = this.CreateGraphics();
       

               if(t.Equals(typeof(Square))) // the selected shape is a square
                {              
                    Square refreshShape = new Square(one, two); // creating a square ( which isn't added to memory)
                    refreshShape = (Square)shape[index]; // update square to match the position of the existing square
                    refreshShape.draw(g, pen); // this re-draws the square

                // If user has selected 'Resize by mouse' and the current list index equals shape being re-sized  
                   if( resizeByDrag == true &&  resizeIndex == index)
                    {
                    refreshShape = new Square(draggedMouseLocation, refreshShape.getOppPt());     
                    shape[shapeIndex] = refreshShape; // updating the square in memory
                    }

                // If user has selected 'Move by mouse drag' and current list index equals shape being moved
                   if(moveByDrag == true && moveByDragIndex == index)
                   {
                    // x and y differences are calculated to enable the square to maintain its dimensions
                    differenceX = refreshShape.getXDifference(differenceX); 
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Square(draggedMouseLocation, newPos);             
                    shape[shapeIndex] = refreshShape; // updating the square in memory
                   }

                // If user has selected 'Move by input' then entered a value and current list index equals moved shape
                if (moveByInput == true && moveByInputIndex == index)
                {
                    refreshShape.setKeyPt(xMove, yMove); // modifying the shape KeyPt according to user input
                    shape[shapeIndex] = refreshShape; // updating square in memory
                }
                // If user has selected Rotate by 90 and the current index equals shape being rotated
                if(rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Square(rotatedKeyPt, rotatedOppPt); // updating square with its new points
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // updating square in memory
                    rotateBy90 = false;
                }
                // If user has selected Rotate by 180 and the current index equals shape being rotated
                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Square(rotatedKeyPt, rotatedOppPt); // updating square with its new points
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // updating square in memory
                    rotateBy180 = false;
                }
                // If user has entered a custom rotate value and current index equals shape being rotated
                if (customRotateMode == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(rotationValue, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(rotationValue, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Square(rotatedKeyPt, rotatedOppPt); // updating square with its new points
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // updating square in memory
                    customRotateMode = false;
                }
                // If user has selected reflectX and index == the shape being reflected
                if(reflectXmode == true && reflectIndex == index)
                {
                    if(refreshShape.getKeyPtX() < MID_SCREEN_X) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_X- refreshShape.getKeyPtX();
                        refreshShape.setKeyPt(0, 0);
                        refreshShape.setKeyPt((MID_SCREEN_X + diffFromCentre) - refreshShape.getKeyPtX(), 0);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getKeyPtX() - MID_SCREEN_X;
                        refreshShape.setKeyPt(- refreshShape.getKeyPtX(),0); // temporarily set the X position to 0
                        refreshShape.setKeyPt(MID_SCREEN_X - diffFromCentre, 0); // set square to its new postion
                    }
                    reflectXmode = false;
                    shape[shapeIndex] = refreshShape; // update square in memory
                }

                if (reflectYmode == true && reflectIndex == index)
                {
                    if (refreshShape.getOppPtY() < MID_SCREEN_Y) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_Y - refreshShape.getOppPtY();
                        refreshShape.setKeyPt(0, MID_SCREEN_Y + diffFromCentre);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getOppPtY() - MID_SCREEN_Y;
                        refreshShape.setKeyPt(0, -(MID_SCREEN_Y + diffFromCentre));
                    }
                    reflectYmode = false;
                    shape[shapeIndex] = refreshShape; // update the square in memory
                }

            } // End of square 


            if (t.Equals(typeof(Triangle))) // the selected shape is a Triangle
            {
                Triangle refreshShape = new Triangle(one, two);
                refreshShape = (Triangle)shape[index]; // update Triangle to match position of the existing triangle
                refreshShape.draw(g, pen); // this re-draws the Triangle

                if (resizeByDrag == true && resizeIndex == index)
                {
                    refreshShape = new Triangle(draggedMouseLocation, refreshShape.getTopPt());
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                }
                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Triangle(draggedMouseLocation, newPos);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                }
                if (moveByInput == true && moveByInputIndex == index)
                {
                    refreshShape.setFirstPt(xMove, yMove);                
                    shape[shapeIndex] = refreshShape; // Updating in memory
                }

                if (rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getFirstPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getTopPt(), refreshShape.getMidPt());
                    refreshShape = new Triangle(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy90 = false;
                }
                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getFirstPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getTopPt(), refreshShape.getMidPt());
                    refreshShape = new Triangle(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy180 = false;
                }
                if (customRotateMode == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(rotationValue, refreshShape.getFirstPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(rotationValue, refreshShape.getTopPt(), refreshShape.getMidPt());

                    refreshShape = new Triangle(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    customRotateMode = false;
                }

                if (reflectXmode == true && reflectIndex == index)
                {
                    if (refreshShape.getFirstPtX() < MID_SCREEN_X) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_X - refreshShape.getFirstPtX();
                        refreshShape.setFirstPt(0, 0);
                        refreshShape.setFirstPt((MID_SCREEN_X + diffFromCentre) - refreshShape.getFirstPtX(), 0);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getFirstPtX() - MID_SCREEN_X;
                        refreshShape.setFirstPt(-refreshShape.getFirstPtX(), 0); // temporarily set the X position to 0
                        refreshShape.setFirstPt(MID_SCREEN_X - diffFromCentre, 0); // set square to its new postion
                    }
                    reflectXmode = false;
                    shape[shapeIndex] = refreshShape; // update square in memory
                }

                if (reflectYmode == true && reflectIndex == index)
                {
                    if (refreshShape.getTopPtY() < MID_SCREEN_Y) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_Y - refreshShape.getTopPtY();
                        refreshShape.setFirstPt(0, MID_SCREEN_Y + diffFromCentre);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getTopPtY() - MID_SCREEN_Y;
                        refreshShape.setFirstPt(0, -(MID_SCREEN_Y + diffFromCentre));
                    }
                    reflectYmode = false;
                    shape[shapeIndex] = refreshShape; // update the square in memory
                }


            } // End of Triangle

            
                if (t.Equals(typeof(Circle))) // the selected shape is a Circle
                  {
                    Circle refreshShape = new Circle(one, two);
                    refreshShape = (Circle)shape[index];
                    refreshShape.draw(g, pen);

                     if (resizeByDrag == true && resizeIndex == index)
                     {
                    refreshShape = new Circle(refreshShape.getKeyPoint(), draggedMouseLocation);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                     }
                   if (moveByDrag == true && moveByDragIndex == index)
                     {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Circle(draggedMouseLocation, newPos);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                     }
                if (moveByInput == true && moveByInputIndex == index)
                {
                    refreshShape.setKeyPt(xMove, yMove);
                    shape[shapeIndex] = refreshShape; // Updating in memory
                }

                if (reflectXmode == true && reflectIndex == index)
                {
                    if (refreshShape.getKeyPtX() < MID_SCREEN_X) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_X - refreshShape.getKeyPtX();
                        refreshShape.setKeyPt(0, 0);
                        refreshShape.setKeyPt((MID_SCREEN_X + diffFromCentre) - refreshShape.getKeyPtX(), 0);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getKeyPtX() - MID_SCREEN_X;
                        refreshShape.setKeyPt(-refreshShape.getKeyPtX(), 0); // temporarily set the X position to 0
                        refreshShape.setKeyPt(MID_SCREEN_X - diffFromCentre, 0); // set square to its new postion
                    }
                    reflectXmode = false;
                    shape[shapeIndex] = refreshShape; // update square in memory
                }

                if (reflectYmode == true && reflectIndex == index)
                {
                    if (refreshShape.getOppPtY() < MID_SCREEN_Y) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_Y - refreshShape.getOppPtY();
                        refreshShape.setKeyPt(0, MID_SCREEN_Y + diffFromCentre);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getOppPtY() - MID_SCREEN_Y;
                        refreshShape.setKeyPt(0, -(MID_SCREEN_Y + diffFromCentre));
                    }
                    reflectYmode = false;
                    shape[shapeIndex] = refreshShape; // update the square in memory
                }

            } // End of Circle
                 


            if (t.Equals(typeof(Hexagon))) // the selected shape is a Hexagon
            {
                Hexagon refreshShape = new Hexagon(one, two);
                refreshShape = (Hexagon)shape[index];
                refreshShape.draw(g, pen);

                if (resizeByDrag == true && resizeIndex == index)
                {
                    refreshShape = new Hexagon(draggedMouseLocation, refreshShape.getOppPt());
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                }
                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference(differenceX);
                    differenceY = refreshShape.getYDifference(differenceY);
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape = new Hexagon(draggedMouseLocation, newPos);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape; // update shape in memory
                }
                if (moveByInput == true && moveByInputIndex == index)
                {
                    refreshShape.setKeyPt(xMove, yMove);
                    shape[shapeIndex] = refreshShape; // Updating in memory
                }

                if (rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Hexagon(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy90 = false;
                }
                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Hexagon(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy180 = false;
                }
                if (customRotateMode == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(rotationValue, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(rotationValue, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape = new Hexagon(rotatedKeyPt, rotatedOppPt);
                    refreshShape.draw(g, pen);
                    shape[shapeIndex] = refreshShape;
                    customRotateMode = false;
                }

                if (reflectXmode == true && reflectIndex == index)
                {
                    if (refreshShape.getKeyPtX() < MID_SCREEN_X) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_X - refreshShape.getKeyPtX();
                        refreshShape.setKeyPt(0, 0);
                        refreshShape.setKeyPt((MID_SCREEN_X + diffFromCentre) - refreshShape.getKeyPtX(), 0);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getKeyPtX() - MID_SCREEN_X;
                        refreshShape.setKeyPt(-refreshShape.getKeyPtX(), 0); // temporarily set the X position to 0
                        refreshShape.setKeyPt(MID_SCREEN_X - diffFromCentre, 0); // set square to its new postion
                    }
                    reflectXmode = false;
                    shape[shapeIndex] = refreshShape; // update square in memory
                }

                if (reflectYmode == true && reflectIndex == index)
                {
                    if (refreshShape.getOppPtY() < MID_SCREEN_Y) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_Y - refreshShape.getOppPtY();
                        refreshShape.setKeyPt(0, MID_SCREEN_Y + diffFromCentre);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getOppPtY() - MID_SCREEN_Y;
                        refreshShape.setKeyPt(0, -(MID_SCREEN_Y + diffFromCentre));
                    }
                    reflectYmode = false;
                    shape[shapeIndex] = refreshShape; // update the square in memory
                }


            } // End of Hexagon


            if (t.Equals(typeof(Line)))
            {
                Line refreshShape = new Line();
                refreshShape = (Line)shape[index];

                refreshShape.drawLine(g, refreshShape.getKeyPt(), refreshShape.getOppPt(), pen);

                if (moveByDrag == true && moveByDragIndex == index)
                {
                    differenceX = refreshShape.getXDifference();
                    differenceY = refreshShape.getYDifference();
                    newPos = refreshShape.setNewPosition(draggedMouseLocation, differenceX, differenceY);
                    refreshShape.drawLine(g, draggedMouseLocation, newPos, pen);

                    shape[shapeIndex] = refreshShape; // update shape in memory
                }

                if (moveByInput == true && moveByInputIndex == index)
                {
                    refreshShape.setKeyPt(xMove, yMove);
                    shape[shapeIndex] = refreshShape; // Updating in memory
                }

                if (rotateBy90 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(90, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(90, refreshShape.getOppPt(), refreshShape.getMidPt());

                    refreshShape.drawLine(g, rotatedKeyPt, rotatedOppPt, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy90 = false;
                }
                if (rotateBy180 == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(180, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(180, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape.drawLine(g, rotatedKeyPt, rotatedOppPt, pen);
                    shape[shapeIndex] = refreshShape;
                    rotateBy180 = false;
                }
                if (customRotateMode == true && rotationIndex == index)
                {
                    rotatedKeyPt = RotatePt(rotationValue, refreshShape.getKeyPt(), refreshShape.getMidPt());
                    rotatedOppPt = RotatePt(rotationValue, refreshShape.getOppPt(), refreshShape.getMidPt());
                    refreshShape.drawLine(g, rotatedKeyPt, rotatedOppPt, pen);
                    shape[shapeIndex] = refreshShape;
                    customRotateMode = false;
                }

                if (reflectXmode == true && reflectIndex == index)
                {
                    if (refreshShape.getKeyPtX() < MID_SCREEN_X) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_X - refreshShape.getKeyPtX();
                        refreshShape.setKeyPt(0, 0);
                        refreshShape.setKeyPt((MID_SCREEN_X + diffFromCentre) - refreshShape.getKeyPtX(), 0);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getKeyPtX() - MID_SCREEN_X;
                        refreshShape.setKeyPt(-refreshShape.getKeyPtX(), 0); // temporarily set the X position to 0
                        refreshShape.setKeyPt(MID_SCREEN_X - diffFromCentre, 0); // set square to its new postion
                    }
                    reflectXmode = false;
                    shape[shapeIndex] = refreshShape; // update square in memory
                }

                if (reflectYmode == true && reflectIndex == index)
                {
                    if (refreshShape.getOppPtY() < MID_SCREEN_Y) // shape on left side of screen
                    {
                        diffFromCentre = MID_SCREEN_Y - refreshShape.getOppPtY();
                        refreshShape.setKeyPt(0, MID_SCREEN_Y + diffFromCentre);
                    }
                    else   // shape on right side of screen
                    {
                        diffFromCentre = refreshShape.getOppPtY() - MID_SCREEN_Y;
                        refreshShape.setKeyPt(0, -(MID_SCREEN_Y + diffFromCentre));
                    }
                    reflectYmode = false;
                    shape[shapeIndex] = refreshShape; // update the square in memory
                }

              
            }

        } // End of shapeIdentify method


        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
               
                Graphics g = this.CreateGraphics();
                Pen blackpen = new Pen(Color.Black);

                
                if (selectSquareStatus == true)  // If the square option has been selected
                {
                    if (clicknumber == 0)  // The first click
                    {
                        one = new Point(e.X, e.Y); // Point 1 is X and Y positon of the event (mouse click)
                        clicknumber = 1;
                    }
                    else
                    {                      
                        mouseHeld = true; // to implement the rubber banding
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e); // once user has let go of left mouse button mouseHeld will be set false

                        clicknumber = 0; // reset to 0
                        selectSquareStatus = false;

                        Square FinalSquare = new Square(one, two);
                        FinalSquare.draw(g, blackpen); // drawing the new square
                        shape.Add(FinalSquare);  // Adding it to memory.
                        refreshScreen();

                    }
                } // end of SelectSquareStatus 

                if (selectTriangleStatus == true) // If the triangle create option has been selected
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y); // Point 1 is the X and Y positon of the event ( mouse click)
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true; // to implement rubber banding
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e); // the user has released the mouse
                        clicknumber = 0;
                        selectTriangleStatus = false;

                        Triangle FinalTriangle = new Triangle(one, two);
                        FinalTriangle.draw(g, blackpen);
                        shape.Add(FinalTriangle); // adding triangle to the list
                        refreshScreen();
                    }

                } // end of triangle create option

                if (selectCircleStatus == true)
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y);
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e); // the user has released the mouse
                        clicknumber = 0;
                        selectCircleStatus = false;

                        Circle FinalCircle = new Circle(one, two);
                        FinalCircle.draw(g, blackpen);
                        shape.Add(FinalCircle); // adding the circle to the list
                        refreshScreen();
                    }

                }

                if (selectHexagonStatus == true)
                {
                    if (clicknumber == 0)
                    {
                        one = new Point(e.X, e.Y);
                        clicknumber = 1;
                    }
                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e);
                        clicknumber = 0;
                        selectHexagonStatus = false;

                        Hexagon FinalHexagon = new Hexagon(one, two);
                        FinalHexagon.draw(g, blackpen);
                        shape.Add(FinalHexagon); // adding Hexagon to the list
                        refreshScreen();
                    }

                }

                if (selectLineStatus == true)
                {

                    if (clicknumber == 0)  // The first click
                    {
                        one = new Point(e.X, e.Y); // Point 1 is the X and Y positon of the event ( mouse click)
                        clicknumber = 1;
                    }

                    else
                    {
                        mouseHeld = true;
                        two = new Point(e.X, e.Y);
                        mouseUp(sender, e);  // user has released mouse
                        clicknumber = 0;
                        selectLineStatus = false;

                        Line finalLine = new Line();
                        finalLine.drawLine(g, one, two, blackpen);
                        shape.Add(finalLine); 
                    }

                }


                if (moveByDrag == true) // Invoked when user is in 'Move by drag'  and has clicked to drop 
                {
                    shapeIdentify(shapeIndex, blackpen);
                    moveByDrag = false;                  
                              
                }

                if (resizeByDrag == true) // When you let go of the mouse when resizing
                {
                    shapeIdentify(shapeIndex, blackpen);
                    resizeByDrag = false;            
                }

            }  // End of left mouse click event

        } // End of mouseClick function

        private void mouseDown(object sender, MouseEventArgs e) // when mouse pressed down
        {
            if (e.Button == MouseButtons.Left && clicknumber == 1)
            {
                mouseHeld = true; // in order to implement rubber banding
            }      
        }

        // mouseMove deals with rubber banding (when creating shape), resize and move by drag
        private void mouseMove(object sender, MouseEventArgs e)
        {
            
            Graphics g = this.CreateGraphics();
            Pen blackpen = new Pen(Color.Red);
            Pen eraser = new Pen(Color.White);
            Square aSquare = new Square(one, draggedMouseLocation);
            Triangle aTriangle = new Triangle(one, draggedMouseLocation);
            Circle aCircle = new Circle(one, draggedMouseLocation);
            Hexagon aHexagon = new Hexagon(one, draggedMouseLocation);

            if (mouseHeld == true) // Rubber banding
            {
                Invalidate();
                Update();
                redrawShapes();

                draggedMouseLocation = e.Location;

                if (selectSquareStatus == true)
                {
                    aSquare.draw(g, blackpen);
                }
                if (selectTriangleStatus == true)
                {
                    aTriangle.draw(g, blackpen);
                }
                if (selectCircleStatus == true)
                {
                    aCircle.draw(g, blackpen);
                }
                if (selectHexagonStatus == true)
                {
                    aHexagon.draw(g, blackpen);
                }
                if (selectLineStatus == true)
                {
                    Line aLine = new Line();
                    aLine.drawLine(g, one, draggedMouseLocation, blackpen);
                }

            }

            if (moveByDrag == true) // This is triggered when the moveByDrag selected and the user is moving the mouse
            {
                draggedMouseLocation = e.Location;
                moveByDragIndex = shapeIndex;
                shapeIdentify(shapeIndex, blackpen);
                refreshScreen();
            }

            if (resizeByDrag == true) // The user is resizing the shape via the mouse
            {
                draggedMouseLocation = e.Location;
                resizeIndex = shapeIndex;
                shapeIdentify(shapeIndex, blackpen);
                refreshScreen();  
            }


        } // End of mouseMove method.

        private void mouseUp(object sender, MouseEventArgs e)
        {
            mouseHeld = false;

        }

        // This method deals with key presses, for the left/right key for selecting shapes.
        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right && shapeSelection) // user chooses right, to move to next item
            {
                if (shapeIndex < shape.Count - 1) // if the index has not yet reached last shape in the list
                {
                    shapeIndex++;  // moving to next item in list                
                    refreshScreen();
                    redrawShapes();
                    Selection(); // this recalls Selection, so the currently selected shape is drawn in red
                }
                else  // the index is currently at last index in the list
                {
                    shapeIndex = 0; // returning to the start of the list
                    refreshScreen();
                    redrawShapes();
                    Selection(); // this recalls Selection, so the currently selected shape is drawn in red
                }

            }

            if (e.KeyCode == Keys.Left && shapeSelection) // user chooses to move to preceding item
            {

                if (shapeIndex > 0) // if the index is not at the first position
                {
                    shapeIndex--;
                    refreshScreen();
                    redrawShapes();
                    Selection();
                }
                else // the index is at the first item of the list
                {
                    shapeIndex = shape.Count - 1; // move to the last item of the list
                    refreshScreen();
                    redrawShapes();
                    Selection();
                }
            }

        } // end of keyDown method

        private void Form1_Load(object sender, EventArgs e)
        {

        }
       
    } //  End of Form1 class

    abstract class Shape
    {
        // This is the base class for Shapes in the application. It should allow an array or LL
        // to be created containing different kinds of shapes.
        public Shape()   // constructor
        {
        }
    }

    class Square : Shape   // This has Main embedded
    {
        //This class contains the specific details for a square defined in terms of opposite corners
        Point keyPt, oppPt;      // these points identify opposite corners of the square
        Point midPt;
        Point topRightPt;
        Point bottomLeftPt;
        int xMovement, yMovement;
        Line line = new Line();

        public Square(Point keyPt, Point oppPt)   // constructor
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }

        // You will need a different draw method for each kind of shape. Note the square is drawn
        // from first principles. All other shapes should similarly be drawn from first principles. 
        // Ideally no C# standard library class or method should be used to create, draw or transform a shape
        // and instead should utilse user-developed code.
        public void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            double xDiff, yDiff, xMid, yMid;   // range and mid points of x & y  
           
            // calculate ranges and mid points
            xDiff = oppPt.X - keyPt.X;
            yDiff = oppPt.Y - keyPt.Y;
            xMid = (oppPt.X + keyPt.X) / 2;
            yMid = (oppPt.Y + keyPt.Y) / 2;
            midPt = new Point((int)xMid, (int)yMid);
            topRightPt = new Point((int)(xMid + yDiff / 2), (int)(yMid - xDiff / 2));
            bottomLeftPt = new Point((int)(xMid - yDiff / 2), (int)(yMid + xDiff / 2));

               line.drawLine(g, keyPt, topRightPt, blackPen);
               line.drawLine(g, oppPt, topRightPt, blackPen);
               line.drawLine(g, bottomLeftPt, oppPt, blackPen);
               line.drawLine(g, bottomLeftPt, keyPt, blackPen);
        }

        public Point getKeyPt() // Is used to process rotation
        {
            return keyPt;
        }

        public int getKeyPtX() // Is used for reflection on X axis
        {
            return keyPt.X;
        }
        public int getKeyPtY()
        {
            return keyPt.Y;
        }

        public Point getOppPt() // is used to process rotation and resize
        {
            return oppPt;
        }

        public Point getMidPt() // is used to process rotation calculations
        {
            return midPt;
        }

        public Point setKeyPt(int x, int y) // used for move by input, also will adjust oppPt
        {
            xMovement = x;
            yMovement = y;
            keyPt.X = keyPt.X + xMovement;
            oppPt.X = oppPt.X + xMovement; // moving the oppPt in order to maintain the dimensions
            keyPt.Y = keyPt.Y + yMovement;
            oppPt.Y = oppPt.Y + yMovement;
            return keyPt;
        }

        public Point updateKeyPt(Point p)
        {
            keyPt = p;
            return keyPt;
        }

        public Point setOppPt(int x, int y)
        {
            oppPt.X = keyPt.X + x;
            oppPt.Y = keyPt.Y + y;

            return oppPt;
        }

        public int getOppPtX()
        {
            return oppPt.X;
        }
        public int getOppPtY() // Used to reflect on Y axis
        {
            return oppPt.Y;
        }
     
        public int getXDifference(int xDiff) // Used to process move by drag
        {
            xDiff = oppPt.X - keyPt.X;      
            return xDiff;
        }

        public int getYDifference(int yDiff) // Used to process move by drag
        {
            yDiff = oppPt.Y - keyPt.Y;
            return yDiff;

        }

        public Point setNewPosition(Point p, int x, int y) // Used to process move by drag
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y + y;
            return newPosition;
        }

        
        public static void Main()
        {
            Application.Run(new Form1());
        }

    } // End of Square class

    class Triangle : Shape
    {
        Point firstPt, topPt, midPt, adjPt;      // these points identify opposite corners of the square          
        int xMovement, yMovement;

        public Triangle(Point firstPt, Point topPt)   // constructor
        {
            this.firstPt = firstPt;
            this.topPt = topPt;     
        }

        public Point getTopPt()
        {
            return topPt;
        }

        public Point getMidPt()
        {
            return midPt;
        }

        public Point getFirstPt()
        {
            return firstPt;
        }
        public int getTopPtY()
        {
            return topPt.Y;
        }

        public int getFirstPtX()
        {
            return firstPt.X;
        }

        public int getXDifference(int xDiff)
        {
            xDiff = topPt.X - firstPt.X;
            return xDiff;
        }

        public Point setFirstPt(int x, int y) // used for move by input, also will adjust oppPt
        {
            xMovement = x;
            yMovement = y;
            firstPt.X = firstPt.X + xMovement;
            topPt.X = topPt.X + xMovement; // moving the oppPt in order to maintain the dimensions
            firstPt.Y = firstPt.Y + yMovement;
            topPt.Y = topPt.Y + yMovement;
            return firstPt;
        }

        public int getYDifference(int yDiff)
        {
            yDiff = firstPt.Y - topPt.Y;
            return yDiff;
        }

     
        public Point setNewPosition(Point p, int x, int y) // Used to process move by drag
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y - y; // firstPt y - topPt y
            return newPosition;
        }

        public void draw(Graphics g, Pen blackPen)
        {
            // This method draws the square by calculating the positions of the other 2 corners
            Point point2;
            point2 = new Point(0, 0);
            double difference;
          
            if (firstPt.Y > topPt.Y)
            {
                point2.Y = firstPt.Y;
            }
            else
            {
                point2.Y = topPt.Y;
            }

            if (firstPt.X > topPt.X)
            {              
                difference = firstPt.X - topPt.X;             
                point2.X = firstPt.X + (int)difference;
            }
            else
            {
                difference = topPt.X - firstPt.X;
                point2.X = topPt.X + (int)difference;
            }          

            adjPt.X = topPt.X + (int)difference;
            adjPt.Y = firstPt.Y;
            midPt.X = topPt.X;
            midPt.Y = (firstPt.Y + topPt.Y)/2 ;

            Line line = new Line();
            line.drawLine(g, firstPt, topPt, blackPen);
            line.drawLine(g, topPt, adjPt, blackPen);
            line.drawLine(g, firstPt, adjPt, blackPen);
                      
        }

    }

    class Circle : Shape
    {
        Point keyPt, oppPt;
        Point[] points = new Point[4];   
       
        Pen blackPen = new Pen(Color.Black, 1);

        public Circle(Point keyPt, Point oppPt)   // constructor
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }

        public Point getKeyPoint()
        {
            return keyPt;
        }

        public int getKeyPtX() // Is used to reflect on X axis
        {
            return keyPt.X;
        }

        public int getOppPtY() // Used to reflect on Y axis
        {
            return oppPt.Y;
        }

        public void draw(Graphics g, Pen p)
        {
            CurveLine curve = new CurveLine();
            curve.drawCircle(g, keyPt, oppPt,p);

        }

        public int getXDifference(int xDiff)
        {
            xDiff = oppPt.X - keyPt.X;
            //  yDiff = oppPt.Y - keyPt.Y;
            return xDiff;
        }

        public int getYDifference(int yDiff)
        {
            yDiff = keyPt.Y - oppPt.Y;
            return yDiff;

        }

        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y - y; // firstPt y - topPt y
            return newPosition;
        }

        public Point setKeyPt(int x, int y)
        {
            
            keyPt.X = keyPt.X + x;
            oppPt.X = oppPt.X + x;
            keyPt.Y = keyPt.Y + y;
            oppPt.Y = oppPt.Y + y;
            return keyPt;
        }


        // Draw the circle.
    }

    class Hexagon : Shape
    {
        Point keyPt, oppPt, topPt1, topPt2, bottomPt1, bottomPt2, midPt;
        int xDiff, yDiff, xMid, yMid;

        public Hexagon(Point keyPt, Point oppPt)   // constructor
        {
            this.keyPt = keyPt;
            this.oppPt = oppPt;
        }

        public int getXDifference(int xDiff)
        {
            xDiff = oppPt.X - keyPt.X;
            return xDiff;
        }

        public int getYDifference(int yDiff)
        {
            yDiff = oppPt.Y - keyPt.Y;
            return yDiff;
        }

        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y + y;
            return newPosition;
        }

        public Point setKeyPt(int x, int y)
        {
            keyPt.X = keyPt.X + x;
            oppPt.X = oppPt.X + x;
            keyPt.Y = keyPt.Y + y;
            oppPt.Y = oppPt.Y + y;
            return keyPt;
        }

        public Point getMidPt()
        {
            return midPt;

        }
        public Point getKeyPt()
        {
            return keyPt;
        }

        public int getKeyPtX()
        {
            return keyPt.X;
        }

        public Point getOppPt()
        {
            return oppPt;
        }
        public int getOppPtY()
        {
            return oppPt.Y;
        }

        public void draw(Graphics g, Pen pen)
        {

            Line line = new Line();
            xMid = (oppPt.X + keyPt.X) / 2;
            yMid = (oppPt.Y + keyPt.Y) / 2;
            midPt = new Point(xMid, yMid);

            topPt1.X = (xMid + keyPt.X) / 2;
            topPt1.Y = keyPt.Y - (xMid - keyPt.X);
            bottomPt1.X = topPt1.X;
            bottomPt1.Y = keyPt.Y + (xMid - keyPt.X);
            topPt2.X = (xMid + oppPt.X) / 2;
            topPt2.Y = topPt1.Y;
            bottomPt2.X = topPt2.X;
            bottomPt2.Y = bottomPt1.Y;

            line.drawLine(g, topPt1, topPt2, pen);
            line.drawLine(g, keyPt, topPt1, pen);           
            line.drawLine(g, topPt2, oppPt, pen);
            line.drawLine(g, oppPt, bottomPt2, pen);            
            line.drawLine(g, bottomPt1, keyPt, pen);
            
            line.drawLine(g, bottomPt1, bottomPt2, pen);
            
        }
    }
  
    class Line : Shape
    {
        int xDiff, yDiff;
        int steps;    
        float xInc, yInc, x , y;
        Point KeyPt, OppPt, MidPt;   

        public Line()   // constructor
        {
           
        }

        public void drawLine(Graphics g, Point startPt, Point endPt, Pen pen)
        {
            KeyPt = startPt;
            OppPt = endPt;

            if (startPt.X > endPt.X) // first x point greater than the second
            {
                xDiff = startPt.X - endPt.X;
                x = endPt.X;
                

            }
            else // the second point x is greater than the first point
            {
                xDiff = endPt.X - startPt.X;
                x = startPt.X; // x is set here just incase the y values are equal ( i.e the four ifs for finding out
                                 // top left, top right click are never triggered)
            }

            if (startPt.Y > endPt.Y)
            {
                yDiff = startPt.Y - endPt.Y;
                y = endPt.Y;
            }
            else
            {
                yDiff = endPt.Y - startPt.Y;
                y = startPt.Y;
            }
            if(startPt.Y == endPt.Y) // This maybe required for drawing a hexagon ( where y values are equal)
            {                        // otherwise y may never be set ( and be 0 by default)
                y = startPt.Y;
            }

            MidPt.X = (int)((xDiff / 2) + x);
            MidPt.Y = (int)((yDiff / 2) + y);

            if (Math.Abs(xDiff) > Math.Abs(yDiff)) // more steps need to be taken along x axis
            {
                steps = Math.Abs(xDiff);
            }
            else                               // more steps need to be taken along y axis
            {
                steps = Math.Abs(yDiff);
            }

            xInc = xDiff / (float)steps;
            yInc = yDiff / (float)steps;

            // The following 4 ifs work out where the two clicks had occured
            if (startPt.X > endPt.X && startPt.Y < endPt.Y) // first click top right, click 2 lower left
            {
                x = endPt.X;
                y = endPt.Y;
                yInc = -yInc; // in order to move downwards, from top left to bottom right
            }

            if(startPt.X < endPt.X && startPt.Y > endPt.Y) // first click lower left, then click top right
            {
                x = startPt.X;
                y = startPt.Y;
                yInc = -yInc; // in order to move upwards, from bottom left to top right
            }

            if(startPt.X > endPt.X && startPt.Y > endPt.Y) // first click lower right, then click top left
            {
                x = endPt.X;
                y = endPt.Y;
            }

            if (startPt.X < endPt.X && startPt.Y < endPt.Y) // first click top left, then click lower right
            {
                x = startPt.X;
                y = startPt.Y;
            }

            // drawing the line
            for (int i = 0; i < steps; i++)
            {
                x += xInc;
                y += yInc;
                putPixel(g, (int)Math.Round(x), (int)Math.Round(y), pen);
            }

        }

        public void putPixel(Graphics g, int x, int y, Pen p)
        {
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(p.Color);
            g.FillRectangle(brush, x, y, 1, 1);
        }

        public Point getMidPt()
        {
            return MidPt;
        }

        public Point getKeyPt()
        {
            return KeyPt;
        }
        public Point getOppPt()
        {
            return OppPt;
        }

        public int getKeyPtX()
        {
            return KeyPt.X;
        }
        public int getOppPtY()
        {
            return OppPt.Y;
        }

        

        public Point setKeyPt(int x, int y)
        {
            KeyPt.X = KeyPt.X + x;
            OppPt.X = OppPt.X + x;
            KeyPt.Y = KeyPt.Y + y;
            OppPt.Y = OppPt.Y + y;
            return KeyPt;
        }

        public int getXDifference()
        {
            return xDiff;
        }

        public int getYDifference()
        {            
            return yDiff;
        }

        public Point setNewPosition(Point p, int x, int y)
        {
            Point newPosition = new Point();
            newPosition.X = p.X + x;
            newPosition.Y = p.Y + y;
            return newPosition;
        }

    }


     class CurveLine : Shape
    {
        
        public void drawCircle(Graphics g,  Point center, Point oppPt, Pen p)
        {
            int x, y;
            int radius = center.Y - oppPt.Y;
            int decision = 3 - 2 * radius;
            y = radius;
            x = 0;

            draw(g, center.X, center.Y, x, y,p);

            while (y >= x)
            {
                x++;

                if (decision > 0)
                {
                    y--;
                    decision = decision + 4 * (x - y) + 10;
                }
                else
                    decision = decision + 4 * x + 6;
                    draw(g, center.X, center.Y, x, y, p);
              
            }

        }
     
        public void draw(Graphics g, int xCentre, int yCentre, int x, int y, Pen p)
        {
            putPixel(g,xCentre + x, yCentre + y, p);
            putPixel(g,xCentre - x, yCentre + y, p);
            putPixel(g,xCentre + x, yCentre - y, p);
            putPixel(g,xCentre - x, yCentre - y, p);
            putPixel(g,xCentre + y, yCentre + x, p);
            putPixel(g,xCentre - y, yCentre + x, p);
            putPixel(g,xCentre + y, yCentre - x, p);
            putPixel(g,xCentre - y, yCentre - x, p);
        }
      
        public void putPixel(Graphics g, int x, int y, Pen p)
        {        
            Pen pen = new Pen(Color.Red);
            System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(p.Color);
            g.FillRectangle(brush, x, y, 1, 1);
        }


    }  



}















    

