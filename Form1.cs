using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

//Namespace of the 2d drawing application.
namespace Extending_a_2D_drawing_application
{
    //Main form class of the application interface. 
    public partial class MainForm : Form
    {
        private List<Shape> shapes = new List<Shape>(); //storing  created shapes. 
        private Shape selectedShape = null;   //store the shape that is selcted. 
        private string currentMode = "Create"; // Interaction of the current mode. 
        private string shapeType = "Square";//The  shape which gone create.
        private Point startPoint;// Store the starting point of the drawing. 
        private bool isDragging = false; // dragging the shape.
        private Point dragStart; // stores the current position of the dragging. 

        public MainForm()
        {
            InitializeComponent(); 
            this.DoubleBuffered = true;// Addding double buffering to minimize the flickering. 
            InitializeMenu(); //Initilalize the application menu.
        }

        private void InitializeMenu()
        {
            MenuStrip menuStrip = new MenuStrip(); //Creating this to hold menu items
            ToolStripMenuItem createMenu = new ToolStripMenuItem("Create"); //Making the create menu item to hold shape options.
            //Making the square options which is under create 
            ToolStripMenuItem squareItem = new ToolStripMenuItem("Square", null, (s, e) => shapeType = "Square"); //shape will be square when clicked.
            ToolStripMenuItem circleItem = new ToolStripMenuItem("Circle", null, (s, e) => shapeType = "Circle"); //Shape will be circle when clicked.
            ToolStripMenuItem triangleItem = new ToolStripMenuItem("Triangle", null, (s, e) => shapeType = "Triangle");//Shape will be triangle when clicked.

            ToolStripMenuItem clearAllShape = new ToolStripMenuItem("Delete All Shape ", null, (s, e) => // The menu item which clear all shape
            {
                shapes.Clear(); // Removing all the shape 
                selectedShape = null; // deselcteing the shape
                Invalidate(); // form to repaint and reflect the changes. 
            });
            menuStrip.Items.Add(clearAllShape);


            createMenu.DropDownItems.AddRange(new ToolStripItem[] { squareItem, circleItem, triangleItem });

            ToolStripMenuItem selectMenu = new ToolStripMenuItem("Select", null, (s, e) => currentMode = "Select"); // Helps to select the shape
            ToolStripMenuItem deleteMenu = new ToolStripMenuItem("Delete", null, (s, e) => DeleteShape()); // Delete the current shape 

            
            ToolStripMenuItem transformMenu = new ToolStripMenuItem("Transform");//Transform menu which has sub option called move , rotate , resize
            ToolStripMenuItem moveMenuItem = new ToolStripMenuItem("Move the shape", null, (s, e) => StartDragging()); // Move menu 

            //Rotate has submenu called Left and right rotation 
            ToolStripMenuItem rotateMenuItem = new ToolStripMenuItem("Rotate");
            ToolStripMenuItem rotateLeftItem = new ToolStripMenuItem("Rotate the shape left (-10°)", null, (s, e) => RotateSelectedShape(-10));
            ToolStripMenuItem rotateRightItem = new ToolStripMenuItem("Rotate the shape right (+10°)", null, (s, e) => RotateSelectedShape(10));

            //  Trandorm menu will get move and roatate option 
            rotateMenuItem.DropDownItems.Add(rotateLeftItem);
            rotateMenuItem.DropDownItems.Add(rotateRightItem);

            // Increase and decrease option in resize 
            transformMenu.DropDownItems.Add(moveMenuItem);
            transformMenu.DropDownItems.Add(rotateMenuItem);

            // Resize Submenu
            ToolStripMenuItem resizeMenuItem = new ToolStripMenuItem("Resize");
            ToolStripMenuItem increaseSizeItem = new ToolStripMenuItem("Increase Size (+10%)", null, (s, e) => ResizeSelectedShape(1.1f));
            ToolStripMenuItem decreaseSizeItem = new ToolStripMenuItem("Decrease Size (-10%)", null, (s, e) => ResizeSelectedShape(0.9f));

            resizeMenuItem.DropDownItems.Add(increaseSizeItem);
            resizeMenuItem.DropDownItems.Add(decreaseSizeItem);

            // Adding Resize to Transform Menu
            transformMenu.DropDownItems.Add(resizeMenuItem);

            //Exit menu 
            ToolStripMenuItem exitMenu = new ToolStripMenuItem("Exit", null, (s, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to exit?", "Confirm Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.OK)
                {
                    Application.Exit(); //Close the form 
                }
            });

            //Adding the menu strips to controls
            menuStrip.Items.Add(exitMenu);
            ;

            menuStrip.Items.Add(createMenu); // Adding Create menu 
            menuStrip.Items.Add(selectMenu); // Adding select menu 
            menuStrip.Items.Add(deleteMenu); // Adding delete menu
            menuStrip.Items.Add(transformMenu); // Adding Transform menu
            menuStrip.Items.Add(exitMenu); //Adding exit menu 

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void RotateSelectedShape(float angle) //Rotating the selcted shape with given angle. 
        {
            if (selectedShape != null)
            {
                selectedShape.Rotate(angle); //Roating the shape  
                Invalidate();  //Showing  the new rotation in the form 
            }
        }


        protected override void OnMouseDown(MouseEventArgs e) //Used for creating , selecting and drag the shapes
        {
            startPoint = e.Location; // location is storing where the mouse was clicked

            if (currentMode == "Create")
            {
                switch (shapeType) //At the clicked place, create a shape of the chosen type.
                {
                    case "Square":
                        shapes.Add(new Square(startPoint, 50));
                        break;
                    case "Circle":
                        shapes.Add(new Circle(startPoint, 50));
                        break;
                    case "Triangle":
                        shapes.Add(new Triangle(startPoint, 50));
                        break;
                }
                Invalidate(); //Reenetr the shape 
            }
            else if (currentMode == "Select")
            {
                selectedShape = null;     //Shape that is under mouse pointer
                foreach (var shape in shapes)
                {
                    if (shape.Contains(e.Location))
                    {
                        selectedShape = shape; // Get a shape which under pointer
                        break;
                    }
                }
                Invalidate();

                //  Reseting  mode to "Create" after selection
                currentMode = "Create";
            }
            else if (currentMode == "Move" && selectedShape != null)
            {
                isDragging = true; //drag the selcted shape
                dragStart = e.Location;
            }
        }


        protected override void OnMouseMove(MouseEventArgs e) //This handle the movement of the mouse
        {
            if (isDragging && selectedShape != null)
            {
                int dx = e.X - dragStart.X; //Will calculate the mouse movement 
                int dy = e.Y - dragStart.Y;
                selectedShape.Move(dx, dy);
                dragStart = e.Location;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) //manages the release of the mouse button to end dragging.
        {
            isDragging = false;
        }



        protected override void OnKeyDown(KeyEventArgs e) //Does the moving input in the keyboard odf selected shape.
        {
            if (selectedShape != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        selectedShape.Move(-10, 0); //will move 10 pixels left 
                        break;
                    case Keys.Right:
                        selectedShape.Move(10, 0); //will move 10 pixels right  

                        break;
                    case Keys.Up:
                        selectedShape.Move(0, -10);//will move 10 pixels up

                        break;
                    case Keys.Down:
                        selectedShape.Move(0, 10);//will move 10 pixels down

                        break;
                    case Keys.R:
                        selectedShape.Rotate(10);//will rotate the shape 10 degree clockwise 

                        break;
                }
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e) 
        {
            foreach (var shape in shapes)
            {
                shape.Draw(e.Graphics); //wil draw the shape into the form
            }

            if (selectedShape != null) // to highlight the selected shape in red color as a box
            {
                e.Graphics.DrawRectangle(Pens.DarkRed, selectedShape.GetBoundingBox());
            }
        }

        private void DeleteShape()
        {
            if (selectedShape != null)
            {
                shapes.Remove(selectedShape); //will remove the shape 
                selectedShape = null; //clear the selection
                Invalidate(); 
            }
        }

        private void StartDragging()
        {
            currentMode = "Move"; // enable dragging to move the mouse
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.ResumeLayout(false);


        }
        private void ResizeSelectedShape(float scaleFactor)
        {
            if (selectedShape != null)
            {
                selectedShape.Resize(scaleFactor); //Scale size and shape
                Invalidate();
            }
        }


        private void MainForm_Load_1(object sender, EventArgs e) // it is empyty now , but this ready for future setup of the code
        {

        }
    }

    // Base Shape Class
    public abstract class Shape
    {
        protected Point position; // shapes position 
        protected float rotationAngle = 0; //angle which shape roatation take place 

        public Shape(Point position) //Constructor to set the shape's initial position
        {
            this.position = position;
        }

        public abstract void Draw(Graphics g); //Methos to draw the shape 
        public abstract bool Contains(Point p); // Method which checks that is apoint is inside in the shape
        public abstract Rectangle GetBoundingBox();

        public void Move(int dx, int dy) //The shape is moved by a given amount (dx, dy).
        {
            position.X += dx;
            position.Y += dy;
        }

        public void Rotate(float angle) //roatating shape according to the angle
        {
            rotationAngle += angle;
        }
        public abstract void Resize(float scaleFactor);

    }

    // Square Class
    public class Square : Shape
    {
        private int size; //sides length of the circle

        public Square(Point position, int size) : base(position)
        {
            this.size = size;
        }

        public override void Draw(Graphics g) //roatation is applied for the square
        {
            using (Matrix matrix = new Matrix())
            {
                matrix.RotateAt(rotationAngle, new PointF(position.X + size / 2, position.Y + size / 2)); //centre of the circle have rotation
                g.Transform = matrix;
                g.FillRectangle(Brushes.Red, position.X, position.Y, size, size); //Color inside the square 
                g.ResetTransform();//Reset the transformation 
            }
        }

        public override void Resize(float scaleFactor)
        {
            size = (int)(size * scaleFactor); // According to the factors square will be able resize
        }

        //Highligbht the selction 
        public override bool Contains(Point p) => new Rectangle(position.X, position.Y, size, size).Contains(p); 
        public override Rectangle GetBoundingBox() => new Rectangle(position.X, position.Y, size, size);


    }

    // Circle Class
    public class Circle : Shape
    {
        private int radius; //Circles radius 

        public Circle(Point position, int radius) : base(position) 
        {
            this.radius = radius;
        }

        public override void Draw(Graphics g) // Rotation is not needed for the circle  
        {
            g.FillEllipse(Brushes.Green, position.X - radius, position.Y - radius, radius * 2, radius * 2);
        }

        public override void Resize(float scaleFactor) //Changeing its radius to resize the circle
        {
            radius = (int)(radius * scaleFactor);
        }

        //Use the circle equation to determine whether a point is inside the circle

        public override bool Contains(Point p) =>
            (Math.Pow(p.X - position.X, 2) + Math.Pow(p.Y - position.Y, 2)) <= Math.Pow(radius, 2);

      //  Obtain the boundary box that completely encloses the circle.
        public override Rectangle GetBoundingBox() =>
            new Rectangle(position.X - radius, position.Y - radius, radius * 2, radius * 2);
    }

    // Triangle Class
    public class Triangle : Shape
    {
        private int size; // Triangles sides lenagth 

        public Triangle(Point position, int size) : base(position)
        {
            this.size = size;
        }

        // Resize the triangle
        public override void Resize(float scaleFactor)
        {
            size = (int)(size * scaleFactor);
        }

        //Rotating the  triangle based on the position and size 
        public override void Draw(Graphics g)
        {
            //Defining three points of the triangle
            Point[] points = {
                new Point(position.X, position.Y - size / 2), // Top
                new Point(position.X - size / 2, position.Y + size / 2), //bottom left 
                new Point(position.X + size / 2, position.Y + size / 2) //bottom right 
            };

            PointF center = new PointF(position.X, position.Y); // rotation center

            using (Matrix matrix = new Matrix())

            {
                //the triangle is roating aroubd centre
                matrix.RotateAt(rotationAngle, center);
                g.Transform = matrix;

                // color is filled with purple
                g.FillPolygon(Brushes.Purple, points);

                //To avoid affecting other drawings, reset the transformation.
                g.ResetTransform();
            }

        }

        public override bool Contains(Point p) => GetBoundingBox().Contains(p);
        public override Rectangle GetBoundingBox() => new Rectangle(position.X - size / 2, position.Y - size / 2, size, size);
    }
}

