using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using founded2sharp;
using Microsoft.DirectX.Direct3D;

//////////////////////////////////////////////////////////////////////
////// ENGINE FOUNDED2SHARP © 2009 BY ALEXANDER KINDZIORA//
//////////////////////////////////////////////////////////////////////


namespace Founded2sharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// //////////////VARIABLEN DEKLARIEREN
        /// </summary>
        private c_Xcore ENGINE;
        private c_resource_management RESOURCES;
        private XSprite BACK;
        private Sprite BatchSprite;
        private c_sprite CURSOR;
        private c_sprite ENERGY;
        private bool MDOWN = false;
        private bool MDOWNR = false;

        private byte[] MBUTTONS;
        public bool running = true;

        //MAP POSITION///////////////////
        private float PositionX = -3000;
        private float PositionY = -950;
        /////////////////////////////////

        public List <c_explosion> EXPLOSIONS;

        private c_LOV CHARAKTER;
        private gui_form_INFO GUI_TALK;
        private gui_form_VERWALTUNG GUI_VERWALTUNG;

        #region rects
        RectangleF TMPRECT = new RectangleF();
        RectangleF SRCRECTF = new RectangleF();
        Rectangle SRCRECT = new Rectangle();
        RectangleF SCreen = new RectangleF();
        c_line_list line;
        #endregion
        
        #region debug stuff
        /// <summary>
        /// ///////////DEBUG STUFF
        /// </summary>
        /// <returns></returns>
            private bool DEBUG = true;
            private int FPS;
            private gui_form_DEBUG GUI_DEBUG;
            private int count=0;
            private int npcCount = 0;
            private Microsoft.DirectX.Direct3D.Font firstFont = null;
            int Objcount = 0;
        #endregion

        private void init_CHARAKTER()
        {
            CHARAKTER = new c_LOV(3, 1, 11, 9, 96);

         
            CHARAKTER.Color = Color.White;
            CHARAKTER.speed.X = 3;
            CHARAKTER.speed.Y = 3;

            //die DrawBox des players///
            CHARAKTER.box.X = 500;
            CHARAKTER.box.Y = 350;
            CHARAKTER.box.Width = 96;
            CHARAKTER.box.Height = 96;

            CHARAKTER.Destbox.X = CHARAKTER.box.X;
            CHARAKTER.Destbox.Y = CHARAKTER.box.Y;
            CHARAKTER.Destbox.Width = CHARAKTER.box.Width;
            CHARAKTER.Destbox.Height = CHARAKTER.box.Height;
            ////////////////////////////

            //bouncebox des players

            CHARAKTER.HitBox = new Polygon();
            CHARAKTER.HitBox.Points.Add(new Vector(30, 50));
            CHARAKTER.HitBox.Points.Add(new Vector(60, 50));
            CHARAKTER.HitBox.Points.Add(new Vector(60, 80));
            CHARAKTER.HitBox.Points.Add(new Vector(30, 80));
            CHARAKTER.HitBox.Offset(new Vector(CHARAKTER.box.X, CHARAKTER.box.Y));
            CHARAKTER.HitBox.BuildEdges();

            CHARAKTER.properties.evil = false;
            CHARAKTER.properties.expirience = 20;
            CHARAKTER.properties.klasse = "ak";
            CHARAKTER.properties.level = 2;
            CHARAKTER.properties.livepoints = 100;
            CHARAKTER.properties.maxlivepoints = CHARAKTER.properties.livepoints;
            CHARAKTER.properties.mana = 10;
            CHARAKTER.properties.name = "npc";
            CHARAKTER.properties.staerke = 10;
            CHARAKTER.properties.defensivpoints = 0;
            CHARAKTER.properties.attackSpeed = 300;
            CHARAKTER.properties.attackRange = 150;

            CHARAKTER.Zebene = (CHARAKTER.HitBox.Points[0].Y + 100) / 1000f;
        
        }

        private bool init_game()
        {
            ////////////////////////////////////////////////
            DirectoryInfo diThis = new DirectoryInfo(Application.StartupPath);
            string pfad = diThis.Parent.Parent.FullName + @"\";
            ////////////////////////////////////////////////

            ///////////////////////////////////////////////////////////////////////
            RESOURCES = new c_resource_management(pfad, this.Handle);//RESOURCE VERWALTUNG         


            Hashtable config = RESOURCES.load_config("screen.ini");
           
            GameDefinition.width = (int)config["width"];
            GameDefinition.height = (int)config["height"];
            GameDefinition.window = (bool)config["fullscreen"];
            GameDefinition.hwnd = this.Handle;
            GameDefinition.music = true;
            GameDefinition.sounds = true;
            GameDefinition.isometric = true;
                
            ENGINE = new c_Xcore(ref RESOURCES);  //GAME ENGINE

            SCreen.Width = (int)config["width"];
            SCreen.Height = (int)config["height"];

            RESOURCES.LoadTexture(0, 0, "b1.PNG",ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 1, "b2.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 2, "b3.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 3, "b4.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 4, "b5.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 5, "b6.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 6, "b7.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(0, 7, "b8.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(2, 1, @"detailtexturen\d1.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(2, 2, @"detailtexturen\wall1.png", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(3, 1, @"detailtexturen\wiking.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(1, 1, "cross.bmp", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(1, 2, "cross2.bmp", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(1, 3, "energy.PNG", ref ENGINE.DEVICE);
            RESOURCES.LoadTexture(1, 4, "energy2.PNG", ref ENGINE.DEVICE);

            RESOURCES.load_music(0, "soulfly1.mp3");
            RESOURCES.load_sound(0, "shot.wav");
            RESOURCES.load_sound(1, "punch1.wav");
            RESOURCES.load_sound(2, "close2.wav");
            RESOURCES.load_sound(3, "earth02.wav");
           
            ///////////////////////////////////////////////////////////////////////
            RESOURCES.load_map("map1.txt");
            RESOURCES.load_detailmap("map2.txt");

            BACK = new XSprite(ref ENGINE.DEVICE);
            BatchSprite = new Sprite(ENGINE.DEVICE);

            ///////////////////////GUI////////////////////////
            this.init_gui();
            //////////////////////////////////////////////////

            this.init_CHARAKTER();
            
            EXPLOSIONS = new List<c_explosion>();
            EXPLOSIONS.Add(new c_explosion(ref ENGINE.DEVICE, ref RESOURCES.TEXTURE[1, 1], 1000));

            ENGINE.AUDIO.play_music(0);
            
            return true;
        }
        
        private void init_gui()
        {
           ///////////////SYSTEM GUI///////////////////////////
            
            GUI_DEBUG = new gui_form_DEBUG(ref ENGINE.DEVICE, 800, 0, 514, 190);
            GUI_DEBUG.initialize_form(ref ENGINE.INPUT, RESOURCES.Pfad);
            GUI_DEBUG.REF_sprite = new Sprite(ENGINE.DEVICE);
            GUI_DEBUG.Text = "DEBUG";
            //////////////////CURSOR///////////////////////////
            CURSOR = new c_sprite(1, 1);

            CURSOR.box.Width = 51;
            CURSOR.box.Height = 51;
            CURSOR.Destbox.Width = 51;
            CURSOR.Destbox.Height = 51;
            ///////////////////////////////////////////////////
            ENERGY = new c_sprite(1, 3);

            ENERGY.box.Width = 120;
            ENERGY.box.Height = 120;
            ENERGY.Destbox.Width = 120;
            ENERGY.Destbox.Height = 120;


            //////VERWALTUNGSMENU/////////////////////////
            GUI_VERWALTUNG = new gui_form_VERWALTUNG(ref ENGINE.DEVICE, 350, 300, 350, 190);
            GUI_VERWALTUNG.REF_sprite = GUI_DEBUG.REF_sprite;
            GUI_VERWALTUNG.initialize_form(ref ENGINE.INPUT, RESOURCES.Pfad);
            GUI_VERWALTUNG.visible = false;
         ///////////////////////////////////////////////////////
            

            line = new c_line_list(ref ENGINE.DEVICE, 111, 100, 555, 555, Color.Yellow.ToArgb());

            firstFont = new Microsoft.DirectX.Direct3D.Font(ENGINE.DEVICE, 20, 5, FontWeight.Medium, 0, false, CharacterSet.Default, Precision.Default, FontQuality.Default, PitchAndFamily.DefaultPitch | PitchAndFamily.FamilyDoNotCare, "Verdana");


            ////////////////GUI FÜR GESPRÄCHE/////////////////////////
            GUI_TALK = new gui_form_INFO(ref ENGINE.DEVICE, 300, 300, 514, 190);
            GUI_TALK.REF_sprite = GUI_DEBUG.REF_sprite;
            GUI_TALK.initialize_form(ref ENGINE.INPUT, RESOURCES.Pfad);

            
            GUI_TALK.Text = "Hallo so siehts hier aus bei uns in unserer (noch) bescheidenen Welt!";

        }

        private void Render_Gui()
        {
            GUI_DEBUG.REF_sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);



            Rectangle scrrect1 = new Rectangle(0, 0, 120, 120);

            int pos = (int)(1.2f * (CHARAKTER.properties.livepoints / (CHARAKTER.properties.maxlivepoints / 100)));
            Rectangle scrrect2 = new Rectangle(0, 120-pos, 120, 120);

            BACK.Draw2D(ref  GUI_DEBUG.REF_sprite, ref RESOURCES.TEXTURE[1, ENERGY.Texture_id], ref scrrect1, ref ENERGY.Destbox, Color.White, 1.0f, 0);

            ENERGY.Destbox.Y = 120-pos;
            BACK.Draw2D(ref  GUI_DEBUG.REF_sprite, ref RESOURCES.TEXTURE[1, 4], ref scrrect2, ref ENERGY.Destbox,Color.FromArgb(180,Color.Red), 1.0f, 0);
            ENERGY.Destbox.Y = 0;


            
            GUI_TALK.DrawElemente();

                GUI_VERWALTUNG.DrawElemente();

            ///////////////SYSTEM GUI//////////////////////////////////


            GUI_DEBUG.Text = "\nObjekte:" + count.ToString();
            GUI_DEBUG.Text += "\nFPS:" + FPS.ToString();
            GUI_DEBUG.Text += "\nOBJEKTE IN RADIUS:" + Objcount.ToString();
            GUI_DEBUG.Text += "\nPOSitionX:" + CHARAKTER.sp.X.ToString();
            GUI_DEBUG.Text += "\nPOSsitionY:" + CHARAKTER.sp.Y.ToString();
            GUI_DEBUG.Text += "\nAIMX:" + CHARAKTER.AIM.X.ToString();
            GUI_DEBUG.Text += "\nAIMY:" + CHARAKTER.AIM.Y.ToString();
            GUI_DEBUG.Text += "\nNpcs:" + npcCount.ToString();
            
            //////////DEBUG///////////////
            GUI_DEBUG.Draw();
            //////////////////////////////


            //VERWALTUNGSMENU///////////////////////


            ////////////////////////////////////////

        
           ////////////MOUSE////////////////////////////////////////
            CURSOR.Destbox.X = ENGINE.INPUT.X;
            CURSOR.Destbox.Y = ENGINE.INPUT.Y;

            MBUTTONS = ENGINE.INPUT.mouseButton();


            if (0 != MBUTTONS[1])
            {
                MDOWNR = true;
    
            }
            else
            {
                if (MDOWNR)
                {
                    //this.ON_mouse_click();
                }
                MDOWNR = false;
           
            }

            if (0 != MBUTTONS[0])
            {
                MDOWN = true;
                CURSOR.Texture_id = 2;
            }
            else
            {
                if (MDOWN)
                {
                    //this.ON_mouse_click();
                }
                MDOWN = false;
                CURSOR.Texture_id = 1;
            }
            


            Rectangle scrrect = new Rectangle(0,0,51,51);
            BACK.Draw2D(ref  GUI_DEBUG.REF_sprite, ref RESOURCES.TEXTURE[1, CURSOR.Texture_id], ref scrrect, ref CURSOR.Destbox, Color.White, 1.0f, 0);
            /////////////////////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            GUI_DEBUG.REF_sprite.End();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.init_game();   
        }

        private void Render_Level1()
        {
           int modPosY = (int)((-PositionY) / 768);
           int modPosX = (int)((-PositionX) / 1024);
            int maxX = 1; int maxY = 1;
            if (modPosX >= RESOURCES.mapsize - 1) maxX = 0;
            if (modPosY >= RESOURCES.mapsize - 1) maxY = 0;

            if (modPosX < 0) modPosX = 0;
            if (modPosY < 0) modPosY = 0;

            int maxPosX = modPosX + maxX;
            int maxPosY = modPosY + maxY;
            int i = 0;

           
            BatchSprite.Begin(SpriteFlags.None);
            RectangleF objRect = new RectangleF();
            objRect.Height = 768;
            objRect.Width = 1024;

            for (int y = modPosY; y <= maxPosY; y++)
             {
                 for (int x = modPosX; x <= maxPosX; x++)
                 {
                     objRect.X = PositionX + 1024 * x;
                     objRect.Y = PositionY + 768 * y;
                     
                     TMPRECT = RectangleF.Intersect(SCreen, objRect);
                     SRCRECTF = ENGINE.MATHENGINE.RectF_Scale(TMPRECT, 0.78f, 0.78f);
                     SRCRECT = ENGINE.MATHENGINE.RectF_Convert(SRCRECTF);

                     if (SRCRECTF.X > 0)
                         SRCRECT.X = 2;
                     else
                         SRCRECT.X = 800 - SRCRECT.Width;

                     if (SRCRECTF.Y > 0) 
                         SRCRECT.Y = 2;
                     else
                         SRCRECT.Y = 600 - SRCRECT.Height;

                     BACK.Draw2D(ref BatchSprite, ref RESOURCES.TEXTURE[0, RESOURCES.map[y, x]], ref SRCRECT, ref TMPRECT, Color.White, 0, 0);
                     
                     i++;
                 }
             }

             BatchSprite.End();
        }

        private void renderLevel2() //Detailmap mit Objekten
        {

            ////////////MAP OBJEKTE///////////////////////////////////////////
            count = 0;
           
            int DmodPosY = (int)((-PositionY) / 97) - 1;
            int DmodPosX = (int)((-PositionX) / 128) - 2;

            int maxX = 10; int maxY = 12;
            if (DmodPosX >= RESOURCES.detailmapsize - 1) maxX = 0;
            if (DmodPosY >= RESOURCES.detailmapsize - 1) maxY = 0;

            int DmaxPosX = DmodPosX + maxX;
            int DmaxPosY = DmodPosY + maxY;

            if (DmodPosY < 0) DmodPosY = 0;
            if (DmodPosX < 0) DmodPosX = 0;
            npcCount = 0;

            BatchSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthBackToFront);

            for (int y = DmodPosY; y <= DmaxPosY; y++)
            {                       
                for (int x = DmodPosX; x <= DmaxPosX; x++)
                {
                    if (RESOURCES.detailmap[y, x].detailmap != -1)
                    {
                        RESOURCES.detailmap[y, x].destinationRect.X = PositionX + 128 * x;
                        RESOURCES.detailmap[y, x].destinationRect.Y =  (PositionY + 97 *y)-RESOURCES.detailmap[y, x].destinationRect.Height;
                        RESOURCES.detailmap[y, x].zebene = (RESOURCES.detailmap[y, x].destinationRect.Y + RESOURCES.detailmap[y, x].HitBox.Points[0].Y + 100) / 1000f;
                        
                        BACK.Draw2D(ref BatchSprite, ref RESOURCES.TEXTURE[2, RESOURCES.detailmap[y, x].detailmap], ref RESOURCES.detailmap[y, x].rect, ref RESOURCES.detailmap[y, x].destinationRect,RESOURCES.detailmap[y, x].color,RESOURCES.detailmap[y, x].zebene,0);

                        count++;
                    }
                    #region  npc part

                  
                        
                        int maxzelevel = RESOURCES.NPC.GetUpperBound(2);
                        
                        for (int z = 0; z < maxzelevel; z++)
                        {
                            if (RESOURCES.NPC[y, x, z] != null)
                            {



                                ///////RENDER NPCS////////////////////////
                                RESOURCES.NPC[y, x, z].Destbox.X = (PositionX) + RESOURCES.NPC[y, x, z].box.X;
                                RESOURCES.NPC[y, x, z].Destbox.Y = (PositionY) + RESOURCES.NPC[y, x, z].box.Y;

                                RESOURCES.NPC[y, x, z].sp.X = (int)(RESOURCES.NPC[y, x, z].box.X / 128);
                                RESOURCES.NPC[y, x, z].sp.Y = (int)(RESOURCES.NPC[y, x, z].box.Y / 97);

                                if (RESOURCES.NPC[y, x, z].sp.X < 0) RESOURCES.NPC[y, x, z].sp.X = -RESOURCES.NPC[y, x, z].sp.X;
                                if (RESOURCES.NPC[y, x, z].sp.Y < 0) RESOURCES.NPC[y, x, z].sp.Y = -RESOURCES.NPC[y, x, z].sp.Y;



                                bool npcIsAttacking = false;

                                if (!GUI_VERWALTUNG.visible)
                                {
                                    npcIsAttacking = this.NPC_check(ref RESOURCES.NPC[y, x, z]);
                                    if (RESOURCES.NPC[y, x, z] == null)
                                    {

                                        EXPLOSIONS.Add(new c_explosion(ref ENGINE.DEVICE, ref RESOURCES.TEXTURE[1, 1], 9000));
                                        EXPLOSIONS[EXPLOSIONS.Count - 1].generate_explosion(30, 9000, CURSOR.Destbox.X, CURSOR.Destbox.Y, Color.Red.ToArgb(), new Microsoft.DirectX.Vector2(0.16f, 0.13f), 0.45f);

                                        continue;
                                    }



                                    #region npc laufen

                                    if (!npcIsAttacking)
                                    {
                                        ////////NPC LAUFEN LASSEN////////////////////////////
                                        bool coll = false;

                                        if (RESOURCES.NPC[y, x, z].direction == 2)
                                        {
                                            RESOURCES.NPC[y, x, z].box.Y += RESOURCES.NPC[y, x, z].speed.Y;


                                            if (RESOURCES.detailmap[y + 1, x].detailmap != -1)
                                            {
                                                RESOURCES.NPC[y, x, z].direction = 1;
                                                coll = true;
                                            }
                                        }
                                        else
                                            if (RESOURCES.NPC[y, x, z].direction == 0)
                                            {
                                                RESOURCES.NPC[y, x, z].box.X += RESOURCES.NPC[y, x, z].speed.X;
                                                if (RESOURCES.detailmap[y, x + 1].detailmap != -1)
                                                {
                                                    RESOURCES.NPC[y, x, z].direction = 3;
                                                    coll = true;
                                                }
                                            }
                                            else
                                                if (RESOURCES.NPC[y, x, z].direction == 1)
                                                {
                                                    RESOURCES.NPC[y, x, z].box.Y -= RESOURCES.NPC[y, x, z].speed.Y;
                                                    int small = -1;
                                                    if (y - 1 < 0) small = 0;

                                                    if (RESOURCES.detailmap[y + small, x].detailmap != -1)
                                                    {
                                                        RESOURCES.NPC[y, x, z].direction = 2;
                                                        coll = true;
                                                    }
                                                }
                                                else
                                                    if (RESOURCES.NPC[y, x, z].direction == 3)
                                                    {
                                                        RESOURCES.NPC[y, x, z].box.X -= RESOURCES.NPC[y, x, z].speed.X;
                                                        int small = -1;
                                                        if (x - 1 < 0) small = 0;
                                                        if (RESOURCES.detailmap[y, x + small].detailmap != -1)
                                                        {
                                                            RESOURCES.NPC[y, x, z].direction = 0;
                                                            coll = true;
                                                        }
                                                    }
                                                    else
                                                        if (RESOURCES.NPC[y, x, z].direction == 7)
                                                        {
                                                            RESOURCES.NPC[y, x, z].box.X -= RESOURCES.NPC[y, x, z].speed.X;
                                                            RESOURCES.NPC[y, x, z].box.Y += RESOURCES.NPC[y, x, z].speed.Y;
                                                            int smallx = -1;
                                                            if (x - 1 < 0) smallx = 0;
                                                            if (RESOURCES.detailmap[y + 1, x + smallx].detailmap != -1)
                                                            {
                                                                RESOURCES.NPC[y, x, z].direction = 0;
                                                                coll = true;
                                                            }
                                                        }
                                                        else
                                                            if (RESOURCES.NPC[y, x, z].direction == 6)
                                                            {
                                                                RESOURCES.NPC[y, x, z].box.X += RESOURCES.NPC[y, x, z].speed.X;
                                                                RESOURCES.NPC[y, x, z].box.Y += RESOURCES.NPC[y, x, z].speed.Y;

                                                                if (RESOURCES.detailmap[y + 1, x + 1].detailmap != -1)
                                                                {
                                                                    RESOURCES.NPC[y, x, z].direction = 0;
                                                                    coll = true;
                                                                }
                                                            }
                                                            else
                                                                if (RESOURCES.NPC[y, x, z].direction == 5)
                                                                {
                                                                    RESOURCES.NPC[y, x, z].box.X -= RESOURCES.NPC[y, x, z].speed.X;
                                                                    RESOURCES.NPC[y, x, z].box.Y -= RESOURCES.NPC[y, x, z].speed.Y;

                                                                    if (RESOURCES.detailmap[y - 1, x - 1].detailmap != -1)
                                                                    {
                                                                        RESOURCES.NPC[y, x, z].direction = 0;
                                                                        coll = true;
                                                                    }
                                                                }
                                                                else
                                                                    if (RESOURCES.NPC[y, x, z].direction == 4)
                                                                    {
                                                                        RESOURCES.NPC[y, x, z].box.X += RESOURCES.NPC[y, x, z].speed.X;
                                                                        RESOURCES.NPC[y, x, z].box.Y -= RESOURCES.NPC[y, x, z].speed.Y;

                                                                        if (RESOURCES.detailmap[y - 1, x + 1].detailmap != -1)
                                                                        {
                                                                            RESOURCES.NPC[y, x, z].direction = 0;
                                                                            coll = true;
                                                                        }
                                                                    }


                                        if (coll == false)
                                        {
                                            RESOURCES.NPC[y, x, z].move(RESOURCES.NPC[y, x, z].direction);
                                        }
                                    }
                                    /////////////////////////////////////////////////////////////////////////
                                    #endregion


                                }
                                    npcCount++;

                                    /////////////////////////////////////////
                                    RESOURCES.NPC[y, x, z].Zebene = (RESOURCES.NPC[y, x, z].Destbox.Y+RESOURCES.NPC[y, x, z].HitBox.Points[0].Y + 100) / 1000f;
                                    
                                    BACK.Draw2D(ref BatchSprite, ref RESOURCES.TEXTURE[3, RESOURCES.NPC[y, x, z].Texture_id], ref RESOURCES.NPC[y, x, z].srcRect, ref RESOURCES.NPC[y, x, z].Destbox, RESOURCES.NPC[y, x, z].Color, RESOURCES.NPC[y, x, z].Zebene, 0);
                                    
                                    if (DEBUG)
                                    {
                                        firstFont.DrawText(BatchSprite, "lp:" + RESOURCES.NPC[y, x, z].properties.livepoints, new Rectangle(0, 0, 0, 0),
                                        DrawTextFormat.NoClip, Color.White);
                                        
                                    }


                                    ///////UPDATE MAP POSITION //IM//ARRAY///
                                    if (RESOURCES.NPC[y, x, z].sp.X != x)
                                    {
                                        if (RESOURCES.NPC[y, RESOURCES.NPC[y, x, z].sp.X, z] == null)
                                        {
                                            RESOURCES.NPC[y, RESOURCES.NPC[y, x, z].sp.X, z] = RESOURCES.NPC[y, x, z];

                                        }
                                        else
                                        {
                                            for (int f = 0; f < maxzelevel; f++)
                                            {
                                                if (f != z)
                                                {
                                                    if (RESOURCES.NPC[y, RESOURCES.NPC[y, x, z].sp.X, f] == null)
                                                    {
                                                        RESOURCES.NPC[y, RESOURCES.NPC[y, x, z].sp.X, f] = RESOURCES.NPC[y, x, z];
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        RESOURCES.NPC[y, x, z] = null;

                                        continue;
                                    }



                                    if (RESOURCES.NPC[y, x, z].sp.Y != y)
                                    {

                                        if (RESOURCES.NPC[RESOURCES.NPC[y, x, z].sp.Y, x, z] == null)
                                        {
                                            RESOURCES.NPC[RESOURCES.NPC[y, x, z].sp.Y, x, z] = RESOURCES.NPC[y, x, z];

                                        }
                                        else
                                        {
                                            for (int f = 0; f < maxzelevel; f++)
                                            {
                                                if (f != z)
                                                {
                                                    if (RESOURCES.NPC[RESOURCES.NPC[y, x, z].sp.Y, x, f] == null)
                                                    {
                                                        RESOURCES.NPC[RESOURCES.NPC[y, x, z].sp.Y, x, f] = RESOURCES.NPC[y, x, z];
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        RESOURCES.NPC[y, x, z] = null;

                                    }
                                }
                        }

                    
                    #endregion
                }
            }
            /////////////////////////////////////////////////////////////////////

            this.Render_CHARAKTER();

            BatchSprite.End();
            //Debug.Print(count.ToString());
        }

        private bool NPC_check(ref c_npc npc)
        {
            npc.caminar = true;

            if (ENGINE.MATHENGINE.punkt_rect_Collision(ref npc.Destbox, CURSOR.Destbox.Location) == true)
            {
                if (!npc.properties.evil)
                {
                    npc.Color = Color.Green;
                    npc.stop();
                    if (MDOWNR && GUI_TALK.visible == false)
                    {
                        npc.caminar = false;
                        npc.set_interval(false);
                        GUI_TALK.Text = npc.get_random_Message();

                        GUI_TALK.visible = true;
                    }
                    else
                    {
                        if (npc.get_interval() == false && GUI_TALK.visible == false)
                        {
                            npc.set_interval(true);
                        }
                    }
                    if (!npc.caminar)
                        npc.direction = -1;
                }
                else
                {///MÖGLICHER//GENGNER////////////////////////////
                    npc.Color = Color.Red;

                    /////////ANGRIFF///////////////
                    if (MDOWNR && GUI_TALK.visible == false)
                    {
                        ////////PLAYER GREIFT AN////////////////////
                        if (ENGINE.MATHENGINE.PointToPoint(CHARAKTER.Destbox.Location, npc.Destbox.Location) >= CHARAKTER.properties.attackRange)
                        {
                            CHARAKTER.caminar = true;

                            CHARAKTER.AIMF.X = (ENGINE.INPUT.X + (-PositionX));
                            CHARAKTER.AIMF.Y = (ENGINE.INPUT.Y + (-PositionY));

                            CHARAKTER.AIM.X = (int)(CHARAKTER.AIMF.X) / 128;
                            CHARAKTER.AIM.Y = (int)(CHARAKTER.AIMF.Y) / 97;
                        }
                        else
                        {
                            if (ENGINE.fight_p2p(ref npc, ref CHARAKTER, true))
                            {
                                EXPLOSIONS.Add(new c_explosion(ref ENGINE.DEVICE, ref RESOURCES.TEXTURE[1, 1], 500));
                                EXPLOSIONS[EXPLOSIONS.Count - 1].generate_explosion(22, 500, CURSOR.Destbox.X, CURSOR.Destbox.Y, Color.Red.ToArgb(), new Microsoft.DirectX.Vector2(0.15f, 0.15f), 0.5f);
                            }
                        }
                        
                    }

                    
                }
            }
            else
            {
                npc.Color = Color.White;
            }

            if (npc != null)
            {
                if (npc.properties.evil)
                {
                    if (npc != null)
                    {
                        ///////////NPC GREIFT AN////////////////////
                        if (ENGINE.MATHENGINE.PointToPoint(CHARAKTER.Destbox.Location, npc.Destbox.Location) >= npc.properties.attackRange)
                        {
                            npc.caminar = true;
                            ENGINE.get_Path(npc.Destbox.Location, CHARAKTER.box.Location, ref npc.weg);
                            npc.direction = (int)npc.weg[0].Z;

                            if (npc.box.Location == CHARAKTER.box.Location)
                            {
                                //npc.caminar = false;
                            }
                            return false;
                        }
                        else
                        {
                            if (ENGINE.fight_p2p(ref npc, ref CHARAKTER))
                            {
                                Random rnd = new Random();

                                EXPLOSIONS.Add(new c_explosion(ref ENGINE.DEVICE, ref RESOURCES.TEXTURE[1, 1], 500));
                                EXPLOSIONS[EXPLOSIONS.Count - 1].generate_explosion(22, 500, rnd.Next((int)CHARAKTER.Destbox.X, (int)(CHARAKTER.Destbox.X + CHARAKTER.Destbox.Width)), rnd.Next((int)CHARAKTER.Destbox.Y, (int)(CHARAKTER.Destbox.Y + CHARAKTER.Destbox.Height)), Color.Salmon.ToArgb(), new Microsoft.DirectX.Vector2(0.15f, 0.15f), 0.5f);
                                
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void Render_CHARAKTER()
        {
            
            BACK.Draw2D(ref BatchSprite, ref RESOURCES.TEXTURE[3, CHARAKTER.Texture_id], ref CHARAKTER.srcRect, ref CHARAKTER.box, CHARAKTER.Color, CHARAKTER.Zebene, 0);

            firstFont.DrawText(BatchSprite, "lp:" + CHARAKTER.properties.livepoints, new Rectangle(0, 0, 0, 0),
                                       DrawTextFormat.NoClip, Color.Red);

            if (CHARAKTER.properties.livepoints <= 0 && GUI_VERWALTUNG.visible ==false)
            {
                EXPLOSIONS.Add(new c_explosion(ref ENGINE.DEVICE, ref RESOURCES.TEXTURE[1, 1], 105000));
                EXPLOSIONS[EXPLOSIONS.Count - 1].generate_explosion(150, 105000, CHARAKTER.Destbox.X, CHARAKTER.Destbox.Y, Color.Red.ToArgb(), new Microsoft.DirectX.Vector2(0.15f, 0.15f), 0.0f);
                
                GUI_VERWALTUNG.visible = true;
            }

            if (DEBUG)
            {
                line.draw_line(new PointF(CHARAKTER.HitBox.Points[0].X, CHARAKTER.HitBox.Points[0].Y), new PointF(CHARAKTER.HitBox.Points[1].X, CHARAKTER.HitBox.Points[1].Y));
                line.draw_line(new PointF(CHARAKTER.HitBox.Points[1].X, CHARAKTER.HitBox.Points[1].Y), new PointF(CHARAKTER.HitBox.Points[2].X, CHARAKTER.HitBox.Points[2].Y));
                line.draw_line(new PointF(CHARAKTER.HitBox.Points[2].X, CHARAKTER.HitBox.Points[2].Y), new PointF(CHARAKTER.HitBox.Points[3].X, CHARAKTER.HitBox.Points[3].Y));
                line.draw_line(new PointF(CHARAKTER.HitBox.Points[3].X, CHARAKTER.HitBox.Points[3].Y), new PointF(CHARAKTER.HitBox.Points[0].X, CHARAKTER.HitBox.Points[0].Y));
            }
        }

        public void check_keys()
        {
            string tasten = ENGINE.key_string();
            

            if (tasten.Contains("W"))
            {
                BACK.zoom -= 0.01f;
            }

            if (tasten.Contains("A"))
            {
               
            }
          
            if (tasten.Contains("S"))
            {
                BACK.zoom+=0.01f;
            }

            if (tasten.Contains("D"))
            {

            }

            if (tasten.Contains("M"))
            {
                //if (ENGINE.PAUSE)
                //{
                //    ENGINE.PAUSE = false;
                //}
                //else
                //{
                //    ENGINE.PAUSE = true;
                //}
                GUI_TALK.visible = true;

            }

            if (tasten.Contains("Escape"))
            {
                GUI_VERWALTUNG.visible = true;
            }
 

        }

        private void Check_Mouse()
        {
            ENGINE.INPUT.pollMouse();

            if (GUI_TALK.visible == false & GUI_VERWALTUNG.visible == false)
            {
                CHARAKTER.spF.X = CHARAKTER.Destbox.X + (-PositionX);
                CHARAKTER.spF.Y = CHARAKTER.Destbox.Y + (-PositionY);

                CHARAKTER.sp.X = (int)(CHARAKTER.spF.X / 128);
                CHARAKTER.sp.Y = (int)(CHARAKTER.spF.Y / 97);

                if (MDOWNR)
                {
                   // EXPLOSION.generate_explosion(18, 1000, ENGINE.INPUT.X, ENGINE.INPUT.Y, Color.Red.ToArgb(), new Microsoft.DirectX.Vector2(1, 5));
                }

                if (MDOWN == true)
                {

                    if (GUI_TALK.visible == true && GUI_TALK.aufobjectDOwn == true)
                    {

                    }
                    else
                    {
                        CHARAKTER.caminar = true;

                        CHARAKTER.AIMF.X = (ENGINE.INPUT.X + (-PositionX));
                        CHARAKTER.AIMF.Y = (ENGINE.INPUT.Y + (-PositionY));

                        CHARAKTER.AIM.X = (int)(CHARAKTER.AIMF.X) / 128;
                        CHARAKTER.AIM.Y = (int)(CHARAKTER.AIMF.Y) / 97;

                     }


                }

                if (CHARAKTER.caminar == true)
                    this.Move_CHARAKTER();

            }
        
        }

        private Vector set_velocity()
        {
            Vector velocity = new Vector();

            switch (CHARAKTER.direction)
            {
                case 0:
                    velocity = new Vector(-CHARAKTER.speed.X, 0);
                    break;
                case 1: velocity = new Vector(0, CHARAKTER.speed.Y);
                    break;
                case 2: velocity = new Vector(0, -CHARAKTER.speed.Y);
                    break;
                case 3: velocity = new Vector(CHARAKTER.speed.X, 0);
                    break;
                case 4: velocity = new Vector(-CHARAKTER.speed.X, CHARAKTER.speed.Y);
                    break;
                case 5:
                    velocity = new Vector(CHARAKTER.speed.X, CHARAKTER.speed.Y);
                    break;
                case 6:
                    velocity = new Vector(-CHARAKTER.speed.X, -CHARAKTER.speed.Y);
                    break;
                case 7:
                    velocity = new Vector(CHARAKTER.speed.X, -CHARAKTER.speed.Y);
                    break;
            }
            return velocity;
        }

        private void charakter_collision(ref Vector playerTranslation)
        {
            PolygonCollisionResult result;

            Objcount = 0;

            int DmodPosY = (int)CHARAKTER.sp.Y+1;
            int DmodPosX = (int)CHARAKTER.sp.X - 1;

            int maxX = 3; int maxY = 3;

            if (DmodPosX >= RESOURCES.detailmapsize - 1) maxX = 0;
            if (DmodPosY >= RESOURCES.detailmapsize - 1) maxY = 0;

            int DmaxPosX = DmodPosX + maxX;
            int DmaxPosY = DmodPosY + maxY;

            if (DmodPosY < 0) DmodPosY = 0;
            if (DmodPosX < 0) DmodPosX = 0;

            int collisionsCount = 0;

            for (int y = DmodPosY; y < DmaxPosY; y++)
            {
                for (int x = DmodPosX; x < DmaxPosX; x++)
                {
                    if (RESOURCES.detailmap[y, x].detailmap != -1 && RESOURCES.detailmap[y, x].detailmap != -2)
                    {

                        RESOURCES.detailmap[y, x].destinationRect.X = PositionX + 128 * x;
                        RESOURCES.detailmap[y, x].destinationRect.Y = (PositionY + 97 * y) - RESOURCES.detailmap[y, x].destinationRect.Height;

                        if (CHARAKTER.Destbox.IntersectsWith(RESOURCES.detailmap[y, x].destinationRect) == true)
                        {
                            Objcount++;
                            RESOURCES.detailmap[y, x].THITBOX(RESOURCES.detailmap[y, x].destinationRect.X + playerTranslation.X, RESOURCES.detailmap[y, x].destinationRect.Y + playerTranslation.Y);

                            if (RESOURCES.detailmap[y, x].HitBox.Points[0].Y != RESOURCES.detailmap[y, x].HitBox.Points[1].Y) //KEINE NORMALE BOUNDING BOX
                            {
                                result = ENGINE.MATHENGINE.PolygonCollision(CHARAKTER.HitBox, RESOURCES.detailmap[y, x].DESTHitBox, playerTranslation);

                                if (ENGINE.MATHENGINE.behind(new PointF(CHARAKTER.HitBox.Points[0].X, CHARAKTER.HitBox.Points[0].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[0].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[0].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[1].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[1].Y)) == true)
                                {
                                    CHARAKTER.Zebene = RESOURCES.detailmap[y, x].zebene - 0.001f;
                                    RESOURCES.detailmap[y, x].color = Color.FromArgb(200, 255, 255, 255);
                                }
                                else
                                {
                                    CHARAKTER.Zebene = (CHARAKTER.HitBox.Points[0].Y + 100) / 1000f;
                                    RESOURCES.detailmap[y, x].color = Color.White;
                                }



                                if (result.WillIntersect)
                                {
                                    collisionsCount++;
                                    if (collisionsCount <= 1)
                                    {
                                        playerTranslation = -result.MinimumTranslationVector + playerTranslation;
                                        CHARAKTER.caminar = false;
                                    }
                                    else
                                    {
                                        playerTranslation = new Vector(0, 0);
                                        CHARAKTER.caminar = false;
                                    }
                                }
                            }
                            else //BEI NORMALER BOUNDING BOX !!!
                            {

                                ///////////////////////////////////////////////
                                if (CHARAKTER.HitBox.Points[0].Y < RESOURCES.detailmap[y, x].DESTHitBox.Points[0].Y)
                                {
                                    //dahinter
                                    //RESOURCES.detailmap[y, x].zebene = CHARAKTER.Zebene + (RESOURCES.detailmap[y, x].DESTHitBox.Points[0].Y+100) / 100f;
                                    RESOURCES.detailmap[y, x].color = Color.FromArgb(200, 255, 255, 255);
                                }
                                else
                                {
                                    //davor
                                    //RESOURCES.detailmap[y, x].zebene = CHARAKTER.Zebene - Objcount / 20f;
                                    RESOURCES.detailmap[y, x].color = Color.White;
                                }
                                ////////////////////////////////////////////////
                                if (CHARAKTER.HitBox.intersect(RESOURCES.detailmap[y, x].DESTHitBox.Points) == true)
                                {
                                    playerTranslation = new Vector(0, 0);
                                    CHARAKTER.caminar = false;
                                }



                            }
                            if (DEBUG)
                            {
                                line.draw_line(new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[0].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[0].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[1].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[1].Y));
                                line.draw_line(new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[1].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[1].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[2].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[2].Y));
                                line.draw_line(new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[2].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[2].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[3].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[3].Y));
                                line.draw_line(new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[3].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[3].Y), new PointF(RESOURCES.detailmap[y, x].DESTHitBox.Points[0].X, RESOURCES.detailmap[y, x].DESTHitBox.Points[0].Y));
                            }
                        }
                        else
                        {
                            CHARAKTER.Zebene = (CHARAKTER.HitBox.Points[0].Y + 100) / 1000f;
                            RESOURCES.detailmap[y, x].color = Color.White;
                        }
                    }
                }
            }
        }

        private void Move_CHARAKTER()
        {
            
            ENGINE.get_Path(CHARAKTER.spF, CHARAKTER.AIMF, ref CHARAKTER.weg);          
            
            //if (CHARAKTER.weg != null)
                //{
                CHARAKTER.direction = (int)CHARAKTER.weg[CHARAKTER.schrittPosition].Z;
                //    if (CHARAKTER.sp == ENGINE.MATHENGINE.vectorToPoint(CHARAKTER.weg[CHARAKTER.schrittPosition]))
                //        CHARAKTER.schrittPosition++;
                //}

                if (CHARAKTER.sp != CHARAKTER.AIM)
                {
                    Vector playerTranslation = this.set_velocity();  //GESCHWINDIGKEITS VECtOR SETZEN jE nAch RICHTUNG

                    this.charakter_collision(ref playerTranslation); //KOLLISIONSCHECK

                    //bewegung/////////////////////////
                    PositionX += playerTranslation.X;
                    PositionY += playerTranslation.Y;
                    ///////////////////////////////////


                    CHARAKTER.move(CHARAKTER.direction); //animation

                }

        }
      
        public bool Render_Game()
        {
            if (GUI_VERWALTUNG.endGame)
            {
                running = false;
                this.Close();
            }
            if (running == false)
                return false;

            FPS = ENGINE.getFramerate();
            
            ///////////////////////
            ENGINE.DEVICE.Clear(ClearFlags.Target, Color.Black, 1.0f, 0);



            if (ENGINE.DEVICE == null)
                return false;

            ENGINE.DEVICE.BeginScene();
            //GAME DARSTELLUNG HIER
            
            if (!ENGINE.PAUSE)
            {
                this.Render_Level1();
                this.renderLevel2();

                for (int i = 0; i < EXPLOSIONS.Count; i++)
                {
                    EXPLOSIONS[i].draw_particle();

                    if (!EXPLOSIONS[i].alive)
                    {
                        EXPLOSIONS.RemoveAt(i);
                    }
                }
            }

            this.Check_Mouse();

            this.Render_Gui();


            ENGINE.DEVICE.EndScene();
            ENGINE.DEVICE.Present();
        
           
           return true;
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            running = false;
            ENGINE.DEVICE.Dispose();
            ENGINE.DEVICE = null;
            ENGINE.INPUT.free();
            ENGINE.INPUT = null;
         
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

 

        //protected override void OnResize(System.EventArgs e)
        //{
        //    ENGINE.PAUSE = ((this.WindowState == FormWindowState.Minimized) || !this.Visible);
        //}

    }




}
