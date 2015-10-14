using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

namespace founded2sharp
{

    abstract class gui_style
    {
        
        /// <summary>
        /// ///////DESIGN//////////////////////////////////////
        /// </summary>
            public RectangleF box; //für Position breite etc
            public Rectangle srcbox;
            public int opacity = 200;
            public bool movable = false;
            public Color col;
            public Color colOnhover;
            public Color colnohover;
            public bool hovereffekt = false;
            public float Z = 0.1f;
            public float zRotation;
            public Color fontcolor = Color.Black;
            public int fontsize = 18;
            public int textX = 30;
            public int textY = 20;
            public Microsoft.DirectX.Direct3D.Font font;
            public FontWeight fontweight;
            public string Text;
        ///////////////////////////////////////////////////////
        

        //////////////////////////////////
        //REFERENZ VARIABLEN (POINTER)
        public c_input REF_input;
        public Sprite REF_sprite;
        public Texture REF_texture;
        public Microsoft.DirectX.Direct3D.Device device;
        //////////////////////////////////

        //EVENT VARIABLEN //////////////////////////
        public bool aufobjectDOwn = false;
        public bool click = false;
        /// <summary>
        /// ////////////////////////////////////////
        /// </summary>
        /// <param name="device"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>

        public gui_style(ref Microsoft.DirectX.Direct3D.Device device, float x, float y, float width, float height)
        {
           this.box = new RectangleF(x,y,width,height);
           this.font = new Microsoft.DirectX.Direct3D.Font(device, fontsize / 4, fontsize, fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Verdana");
        }

        public void check_aktion()
        {
            if (this.hovereffekt != false) this.col = this.colnohover;
      
            if (this.REF_input.X > this.box.X)
            {
                if (this.REF_input.X < this.box.X + this.box.Width)
                {
                    if (this.REF_input.Y > this.box.Y)
                    {
                        if (this.REF_input.Y < this.box.Y + this.box.Height) //Maus befindet sich auf element
                        {
                            if (this.hovereffekt != false)
                            {
                                this.col = this.colOnhover;
                            }
                            
                            
                            byte[] buttons = this.REF_input.mouseButton();
                            if (0 != buttons[0])
                            {
                                this.aufobjectDOwn = true;

                                if (this.movable == true)
                                {
                                    this.box.Y = this.box.Y + this.REF_input.sY();
                                    this.box.X = this.box.X + this.REF_input.sX();
                                }


                            }
                            else
                            {
                                if (this.aufobjectDOwn)
                                {
                                    this.ON_mouse_click();
                                }
                                this.aufobjectDOwn = false;
                            }
                        }
                        else { this.aufobjectDOwn = false; }
                    }
                    else { this.aufobjectDOwn = false; }
                }
                else { this.aufobjectDOwn = false; }
            }
            else { this.aufobjectDOwn = false; }
            
        }

        public virtual void ON_mouse_click()
        {
            this.click = true;
        }

        public void Draw()
        {
            
            if (this.REF_texture != null)
            {
                Color destcolp = Color.FromArgb(this.opacity, this.col.R, this.col.G, this.col.B);
                Matrix transformation =
                    //First, translate to the center of the original (untransformed) sprite
                    //Matrix.Translation(-destRect.Width / 2f, -destRect.Height / 2f, 0f)

                //scale the sprite
                  Matrix.Scaling(this.box.Width / this.srcbox.Width, this.box.Height / this.srcbox.Height, 1)
                    //rotate about the z axis (into the screen) of the centered sprite
                * Matrix.RotationZ(this.zRotation)

                //translate to the final position
                * Matrix.Translation(this.box.X, this.box.Y, this.Z);

                //set the transformation
                this.REF_sprite.Transform = transformation;
                //Draw with position 0, center 0, and color white (all needed transformation has been completed at this point)

                this.REF_sprite.Draw(this.REF_texture, this.srcbox, Vector3.Empty, Vector3.Empty, destcolp);

                //this.REF_sprite.Draw2D(this.REF_texture,this.srcbox, new SizeF(this.box.Width, this.box.Height), new PointF(0, 0), 0, new PointF(this.box.X, this.box.Y), this.col);
            }
            if (this.Text !=null)
            {
                Color destcol = Color.FromArgb(this.opacity, fontcolor.R, fontcolor.G, fontcolor.B);

                this.font.DrawText(this.REF_sprite, this.Text, new Rectangle(textX, textY, (int)this.box.Width - textX, (int)this.box.Height - textY),
                   DrawTextFormat.NoClip | DrawTextFormat.WordBreak , destcol);
            }
            this.check_aktion();
        }

       public void Draw(float x, float y)
        {
            this.box.X = x;
            this.box.Y = y;
            this.Draw();
        }
       
    

    }



    class gui_button : gui_style
    {
        public float X;
        public float Y;

        public gui_button(ref Microsoft.DirectX.Direct3D.Device device, float x, float y, float width, float height)
            : base(ref device, x, y, width, height)
        {
            this.device = device;
        }
    }


    class gui_form_INFO : gui_style
    {
        private gui_button button_OK;
        public bool visible = false;
        


        public gui_form_INFO(ref Microsoft.DirectX.Direct3D.Device device, float x, float y, float width, float height)
            : base(ref device, x, y, width, height)
        {
            this.device = device;

        }

        public bool initialize_form(ref c_input inp, string pfad)
        {
            ///////FORM/////////////////
            this.REF_input   =  inp;
            
            this.REF_texture = TextureLoader.FromFile(this.device, pfad + @"texturen\form.PNG", 514 ,190 , 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            this.fontweight  = FontWeight.Normal;
            this.font = new Microsoft.DirectX.Direct3D.Font(this.device, this.fontsize, (this.fontsize/100) *30, this.fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Palatino Linotype");
            this.Text = "Welcome in the new game!\n This is a new era and at first i like to say i love programingThe pixel shader code is similar to the vertex shader. Position and texture coordinate input comes from the vertex shader. Output is just the color of the final pixel. A texture is the only global variable. Inside the ps_main function, the texel at the specified coordinate is fetched and then blended (multiplied) with a certain color.!";
            this.textX = 15;
            this.textY = 19;
            this.col         = Color.LemonChiffon;
            this.srcbox = new Rectangle(0, 0, 514, 190);
            this.movable = true;
            ////////////////////////////

            /////////////////////ELEMENTE////////////////////////////////
            button_OK             = new gui_button(ref this.device, this.box.X + 205, this.box.Y + 140, 80, 25);
            button_OK.col         = Color.Khaki;
            button_OK.X           = 205;
            button_OK.Y           = 140;
            button_OK.hovereffekt = true;
            button_OK.colOnhover  = Color.Brown;
            button_OK.colnohover  = button_OK.col;

            button_OK.REF_input   = inp;
            button_OK.REF_sprite  = this.REF_sprite;
            button_OK.REF_texture = TextureLoader.FromFile(this.device, pfad + @"texturen\button.PNG", 100 ,30 , 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            button_OK.fontweight = FontWeight.ExtraBold;
            button_OK.font = new Microsoft.DirectX.Direct3D.Font(this.device, 15, 7, button_OK.fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Verdana");
            button_OK.textX = 40;
            button_OK.textY = 8;
            button_OK.Text = "OK";
           
            button_OK.srcbox = new Rectangle(0, 0, 100, 30);

            /////////////////////////////////////////////////////////////
           
            return true;
        }

        public void DrawElemente()
        {
            if (this.visible)
            {
                /////DRAW ELEMENTE///////////////////
                this.Draw();

                float ausgleichx = 0; float ausgleichy = 0;

                if (this.movable == true && this.aufobjectDOwn == true)
                {
                    ausgleichx = -this.REF_input.sX();
                    ausgleichy = -this.REF_input.sY();
                }

                this.button_OK.Draw(this.box.X + this.button_OK.X + ausgleichx, this.box.Y + this.button_OK.Y + ausgleichy);

                /////////////////////////////////////

                this.check_();
            }
        }
        private void check_()
        {
            if (this.button_OK.click == true)
            {
                this.visible = false;
                this.button_OK.click = false;
            }
        }
        
    }

    class gui_form_VERWALTUNG : gui_style
    {
        private gui_button button_close;
        private gui_button button_cancelGame;

        public bool endGame = false;
        public bool visible = false;



        public gui_form_VERWALTUNG(ref Microsoft.DirectX.Direct3D.Device device, float x, float y, float width, float height)
            : base(ref device, x, y, width, height)
        {
            this.device = device;

        }

        public bool initialize_form(ref c_input inp, string pfad)
        {
            ///////FORM/////////////////
            this.REF_input = inp;

            this.REF_texture = TextureLoader.FromFile(this.device, pfad + @"texturen\Mgmenu.PNG", 350, 190, 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            this.fontweight = FontWeight.Bold;
            this.fontsize = 25;
            this.font = new Microsoft.DirectX.Direct3D.Font(this.device, this.fontsize, 10, this.fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Palatino Linotype");
            this.Text = "Main Menu";
            this.textX = 110;
            this.textY = 19;
            this.col = Color.LemonChiffon;
            this.srcbox = new Rectangle(0, 0, 350, 190);
           

            this.movable = true;
            ////////////////////////////

            /////////////////////ELEMENTE////////////////////////////////

            



            button_close = new gui_button(ref this.device, this.box.X + 205, this.box.Y + 100, 80, 25);
            button_close.col = Color.Khaki;
            button_close.X = 100;
            button_close.Y = 100;
            button_close.hovereffekt = true;
            button_close.colOnhover = Color.SandyBrown;
            button_close.colnohover = button_close.col;

            button_close.REF_input = inp;
            button_close.REF_sprite = this.REF_sprite;
            button_close.REF_texture = TextureLoader.FromFile(this.device, pfad + @"texturen\button.PNG", 100, 30, 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            button_close.fontweight = FontWeight.Bold;
            button_close.font = new Microsoft.DirectX.Direct3D.Font(this.device, 15,5, button_close.fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Verdana");
            button_close.textX = 20;
            button_close.textY = 8;
            button_close.Text = "return to Game";

            button_close.srcbox = new Rectangle(0, 0, 100, 30);
            button_close.box = new Rectangle((int)button_close.X, (int)button_close.Y, 150, 30);

            /////////BUTTON CLOSE GAME ////////////////////////

            button_cancelGame = new gui_button(ref this.device, this.box.X + 205, this.box.Y + 140, 80, 25);
            button_cancelGame.col = Color.Khaki;
            button_cancelGame.X = 100;
            button_cancelGame.Y = 140;
            button_cancelGame.hovereffekt = true;
            button_cancelGame.colOnhover = Color.SandyBrown;
            button_cancelGame.colnohover = button_cancelGame.col;

            button_cancelGame.REF_input = inp;
            button_cancelGame.REF_sprite = this.REF_sprite;
            button_cancelGame.REF_texture = button_close.REF_texture;
            button_cancelGame.fontweight = FontWeight.Bold;
            button_cancelGame.font = button_close.font;
            button_cancelGame.textX = 20;
            button_cancelGame.textY = 8;
            button_cancelGame.Text = "Close Game";

            button_cancelGame.srcbox = new Rectangle(0, 0, 100, 30);
            button_cancelGame.box = new Rectangle((int)button_cancelGame.X, (int)button_cancelGame.Y, 150, 30);


            /////////////////////////////////////////////////////////////





            return true;
        }
        public void DrawElemente()
        {
            if (this.visible)
            {
                /////DRAW ELEMENTE///////////////////
                this.Draw();

                float ausgleichx = 0; float ausgleichy = 0;

                if (this.movable == true && this.aufobjectDOwn == true)
                {
                    ausgleichx = -this.REF_input.sX();
                    ausgleichy = -this.REF_input.sY();
                }

                this.button_close.Draw(this.box.X + this.button_close.X + ausgleichx, this.box.Y + this.button_close.Y + ausgleichy);

                this.button_cancelGame.Draw(this.box.X + this.button_cancelGame.X + ausgleichx, this.box.Y + this.button_cancelGame.Y + ausgleichy);

                /////////////////////////////////////
                this.check_();
            }

        }
        private void check_()
        {
            if (this.button_close.click == true)
            {
                this.visible = false;
                this.button_close.click = false;
            }
            if (this.button_cancelGame.click == true)
            {
                this.endGame = true;
            }
        }

    }

     class gui_form_DEBUG : gui_style
    {
        public gui_form_DEBUG(ref Microsoft.DirectX.Direct3D.Device device, float x, float y, float width, float height)
            : base(ref device, x, y, width, height)
        {
            this.device = device;

        }

        public bool initialize_form(ref c_input inp, string pfad)
        {
            ///////FORM/////////////////
            this.REF_input = inp;

            this.REF_texture = TextureLoader.FromFile(this.device, pfad + @"texturen\form.PNG", 514, 190, 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            this.fontweight = FontWeight.Normal;
            this.font = new Microsoft.DirectX.Direct3D.Font(this.device, this.fontsize, (this.fontsize / 100) * 30, this.fontweight, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Palatino Linotype");
            this.textX = 15;
            this.textY = 19;
            this.col = Color.LemonChiffon;
            this.srcbox = new Rectangle(0, 0, 514, 190);
            this.movable = true;
            this.opacity = 255;
            ////////////////////////////
            return true;
        }

    }
    


}
