//includes der nötigen libs
using System;
using System.Numeric;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;
using Microsoft.DirectX.AudioVideoPlayback;
using Microsoft.DirectX.DirectSound;
using Dsound = Microsoft.DirectX.DirectSound;
///////////////////////////////////////

namespace founded2sharp
{
    public class c_resource_management
    {
        //////////TEXTUREN SPEICHERN ///////////////////
        public Texture[,] TEXTURE = new Texture[4,18];//
        ////////////////////////////////////////////////
        
        ////////////////////////////////////////////////
        public Microsoft.DirectX.DirectSound.Device SOUNDDEVICE;
        public SecondaryBuffer[] SOUND = new SecondaryBuffer[5];
        public Audio[] MUSIC = new Audio[5];
        ////////////////////////////////////////////////
        public IntPtr HWND;
        /// <summary>
        /// //////NPCs
        /// </summary>
        public c_npc[,,] NPC;
 
        public readonly string Pfad;
        
        /// <summary>
        /// /MAPSTUFF
        /// </summary>
        public int[,] map;

       

        public struct detailObjekt
        {
            public int detailmap;
            public Rectangle rect;
            public RectangleF destinationRect;

            public Polygon HitBox;
            public Polygon DESTHitBox; 

            public int HitRadius;

            public float zebene;
            public Color color;



            public void THITBOX(float x, float y)
            {
                DESTHitBox = new Polygon();
                for (int i = 0; i < 4; i++)
                {
                    DESTHitBox.Points.Add(new Vector(HitBox.Points[i].X + x, HitBox.Points[i].Y + y));
                }
                DESTHitBox.BuildEdges();
            }
        }

        public detailObjekt[,] detailmap;
    
        public int mapsize;
        public int detailmapsize;
        /// <summary>
        /// MAPSTUFF
        /// </summary>
        /// 

        public c_resource_management(string basepfad, IntPtr hwnd)
        {
            this.Pfad = basepfad;
            HWND = hwnd;
            SOUNDDEVICE = new Microsoft.DirectX.DirectSound.Device();
            SOUNDDEVICE.SetCooperativeLevel(HWND, CooperativeLevel.Normal);
        }

        public Hashtable load_config(string Datname)
        {
            Hashtable hash = new Hashtable();
            CfgFile conf = new CfgFile(this.Pfad + @"config\" + Datname);
            
            hash.Add("width", Convert.ToInt32(conf.getValue("bildschirm", "width", true)));
            hash.Add("height", Convert.ToInt32(conf.getValue("bildschirm", "height", true)));
            hash.Add("fullscreen", Convert.ToBoolean(conf.getValue("bildschirm", "fullscreen", true)));
            
            return hash;
        }


        public void load_map(string Datname)
        {
            StreamReader sr = new StreamReader( this.Pfad + @"\maps\bigmaps\" + Datname);

            int o = -1; int tex =0;
            while (sr.Peek() != -1)
            {
                o++;
                string line = sr.ReadLine();
                
                if (o == 0)
                {
                    this.mapsize = line.Length;
                    map = new int[this.mapsize, this.mapsize];
                }

                for (int i = 0; i < line.Length; i++)
                {
                    switch (line[i])
                    {
                        case '1': tex = 0;
                            break;
                        case '2': tex = 1;
                            break;
                        case '3': tex = 2;
                            break;
                        case '4': tex = 3;
                            break;
                        case '5': tex = 4;
                            break;
                        case '6': tex = 5;
                            break;
                        case '7': tex = 6;
                            break;
                        case '8': tex = 7;
                            break;
                    }
                    map[o, i] = tex;
                }
            }
            sr.Close();
        }

        public void load_detailmap(string Datname)
        {

            StreamReader sr = new StreamReader(this.Pfad + @"\maps\detailmaps\" + Datname);

            int o = -1;
            
            while (sr.Peek() != -1)
            {
                o++;
                string line = sr.ReadLine();

                if (o == 0)
                {
                    this.detailmapsize = line.Length;
                    detailmap = new detailObjekt[this.detailmapsize, this.detailmapsize];
                    NPC = new c_npc[this.detailmapsize, this.detailmapsize,20];

                }
               
                for (int i = 0; i < line.Length; i++)
                {
                    
                    switch (line[i]) ////BÄUME UND ÄHNLICHES
                    {
                        case '0': 
                            detailmap[o, i].detailmap = -1;

                            break;

                        case '#':

                            NPC[o, i,0] = new c_npc(3, 1, 11, 9, 96,this.Pfad + @"\npc\");
                            NPC[o, i, 0].caminar = true;
                            
                            NPC[o, i, 0].Color = Color.White;
                            NPC[o, i, 0].speed.X = 0.8f;
                            NPC[o, i, 0].speed.Y = 0.8f;

                            //die DrawBox des players///
                            NPC[o, i, 0].box.X = i * 128;
                            NPC[o, i, 0].box.Y = o * 97;
                            NPC[o, i, 0].box.Width = 96;
                            NPC[o, i, 0].box.Height = 96;

                            NPC[o, i, 0].Destbox.X = NPC[o, i, 0].box.X;
                            NPC[o, i, 0].Destbox.Y = NPC[o, i, 0].box.Y;
                            NPC[o, i, 0].Destbox.Width = NPC[o, i, 0].box.Width;
                            NPC[o, i, 0].Destbox.Height = NPC[o, i, 0].box.Height;

                            NPC[o, i, 0].HitBox = new Polygon();
                            NPC[o, i, 0].HitBox.Points.Add(new Vector(30, 50));
                            NPC[o, i, 0].HitBox.Points.Add(new Vector(60, 50));
                            NPC[o, i, 0].HitBox.Points.Add(new Vector(60, 80));
                            NPC[o, i, 0].HitBox.Points.Add(new Vector(30, 80));
                            NPC[o, i, 0].HitBox.BuildEdges();

                            //////////NPC EIGENSCHAFTEN///////////////
                            NPC[o, i, 0].properties.evil = true;
                            NPC[o, i, 0].properties.expirience = 10;
                            NPC[o, i, 0].properties.klasse = "wiking";
                            NPC[o, i, 0].properties.level = 1;
                            NPC[o, i, 0].properties.livepoints = 50;
                            NPC[o, i, 0].properties.mana = 10;
                            NPC[o, i, 0].properties.name = "npc";
                            NPC[o, i, 0].properties.staerke = 1;
                            NPC[o, i, 0].properties.defensivpoints = 0;
                            NPC[o, i, 0].properties.attackSpeed = 20;
                            NPC[o, i, 0].properties.attackRange = 80;
                            //////////////////////////////////////////

                            ////////////////////////////
                            detailmap[o, i].detailmap = -1; //OBJEKTKLASSE
                            
                            
                            break;


                        case '1': 
                            detailmap[o, i].detailmap = 1; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 135; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 150; //ZIELHÖHE
                            
                           ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(55, 105));
                            detailmap[o, i].HitBox.Points.Add(new Vector(92, 105));
                            detailmap[o, i].HitBox.Points.Add(new Vector(92, 125));
                            detailmap[o, i].HitBox.Points.Add(new Vector(55, 125));
                            detailmap[o, i].HitBox.BuildEdges();
                           ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(0,0,130,130); //SRC RECTANGLE AUS DATEI

                            break;
                        case '2': 
                            
                            detailmap[o, i].detailmap = 1; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 135; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 150; //ZIELHÖHE
                            
                           ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(55, 105));
                            detailmap[o, i].HitBox.Points.Add(new Vector(92, 105));
                            detailmap[o, i].HitBox.Points.Add(new Vector(92, 125));
                            detailmap[o, i].HitBox.Points.Add(new Vector(55, 125));
                            detailmap[o, i].HitBox.BuildEdges();
                           ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(150,0,130,130); //SRC RECTANGLE AUS DATEI

                            break;
                        case '3':

                            detailmap[o, i].detailmap = 1; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 350; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 335; //ZIELHÖHE

                            ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(145, 265));
                            detailmap[o, i].HitBox.Points.Add(new Vector(192, 265));
                            detailmap[o, i].HitBox.Points.Add(new Vector(192, 285));
                            detailmap[o, i].HitBox.Points.Add(new Vector(145, 285));
                            detailmap[o, i].HitBox.BuildEdges();
                            ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(0, 145, 350, 363); //SRC RECTANGLE AUS DATEI

                            break;
                    }


                    switch (line[i]) ////WÄNDE UND ÄHNLICHES
                    {
                        case 'a':

                            detailmap[o, i].detailmap = 2; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 128; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 97; //ZIELHÖHE
                           
                            ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 68));
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 68));
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 93));
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 93));
                            detailmap[o, i].HitBox.BuildEdges();
                            ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(0, 0, 128, 97); //SRC RECTANGLE AUS DATEI



                            break;

                        case 'b':

                            detailmap[o, i].detailmap = 2; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 128; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 190; //ZIELHÖHE

                            ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 68));
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 165));
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 192));
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 94));
                            detailmap[o, i].HitBox.BuildEdges();
                            ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(128, 0, 128, 190); //SRC RECTANGLE AUS DATEI


                            break;
                        case 'c':

                            detailmap[o, i].detailmap = 2; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 128; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 190; //ZIELHÖHE

                            ////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 68));
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 165));
                            detailmap[o, i].HitBox.Points.Add(new Vector(0, 192));
                            detailmap[o, i].HitBox.Points.Add(new Vector(128, 94));
                            detailmap[o, i].HitBox.BuildEdges();
                            ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(256, 0, 128, 190); //SRC RECTANGLE AUS DATEI


                            break;

                        case 'd':

                            detailmap[o, i].detailmap = 2; //OBJEKTKLASSE

                            detailmap[o, i].destinationRect.Width = 30; //ZIELBREITE
                            detailmap[o, i].destinationRect.Height = 150; //ZIELHÖHE

                            ////////////////////////////////////////////////////////
                            detailmap[o, i].HitRadius = 10;

                            detailmap[o, i].HitBox = new Polygon();
                            detailmap[o, i].HitBox.Points.Add(new Vector(0 , 0));
                            detailmap[o, i].HitBox.Points.Add(new Vector(25, 0));
                            detailmap[o, i].HitBox.Points.Add(new Vector(25, 150));
                            detailmap[o, i].HitBox.Points.Add(new Vector(0 , 150));
                            detailmap[o, i].HitBox.BuildEdges();
                            ////////////////////////////////////////////////////
                            detailmap[o, i].rect = new Rectangle(388, 0, 30, 97); //SRC RECTANGLE AUS DATEI


                            break;
                    }
                   
                        detailmap[o, i].color = Color.White;
                       

                    
                }
            }
            sr.Close();
        }

        


        public void LoadTexture(int klasse ,int id, string Datname, ref Microsoft.DirectX.Direct3D.Device device) //textur laden
        {
            Bitmap tmp = (Bitmap)Image.FromFile(this.Pfad + @"texturen\" + Datname);

            this.TEXTURE[klasse,id] = TextureLoader.FromFile(device, this.Pfad + @"texturen\" + Datname, tmp.Width, tmp.Height, 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, Color.FromArgb(45, 45, 255).ToArgb());
            tmp.Dispose();
            tmp = null;
        }

        public void LoadTexture(int klasse, int id, string Datname, ref Microsoft.DirectX.Direct3D.Device device, Color colorkey) //textur laden
        {
            Bitmap tmp = (Bitmap)Image.FromFile(this.Pfad + @"texturen\" + Datname);

            this.TEXTURE[klasse, id] = TextureLoader.FromFile(device, this.Pfad + @"texturen\" + Datname, tmp.Width, tmp.Height, 0, 0, Format.Unknown, Pool.Default, Filter.None, Filter.None, colorkey.ToArgb());
            tmp.Dispose();
            tmp = null;
        }

        public void UnloadTexture(int klasse,int index)
        {
            this.TEXTURE[klasse, index] = null;
            this.TEXTURE[klasse, index].Dispose();
        }


        public void load_sound(int index, string file)
        {
            BufferDescription description = new BufferDescription();
            description.ControlEffects = false;
            SOUND[index] = new SecondaryBuffer(Pfad + @"sounds\sounds\" + file, description, SOUNDDEVICE);
        }

        public bool load_music(int index, string file)
        {
            MUSIC[index] = new Audio(Pfad + @"sounds\music\" + file);

            return true;
        }
        
    }

    public struct c_messages
    {
       public string[,] msg;  // msg[frage,antwort,richtige]
       public int[] richtige;
       public void load_msgs(string Datname) //parsen der texte und speichern in msg array
       {
            StreamReader sr = new StreamReader(Datname);
            
            int frageC = -1;
            int antwortC = 0;
           

            
          
            this.msg = new string[4, 4];
            this.richtige = new int[4]; 

            while (sr.Peek() != -1)
            {
                string line = sr.ReadLine();
                

                if (line.Substring(0, 1) == "?")
                {
                    frageC++;
                    antwortC = 0;
                    this.msg[frageC, antwortC] = line.Substring(1, line.Length - 1);
                    
                }

                if (line.Substring(0, 1) == "!")
                {
                    antwortC++;
                    this.msg[frageC, antwortC] = line.Substring(1, line.Length - 1);
                    
                }
                
                if (line.Substring(0, 1) == "=")
                {
                    this.richtige[frageC] = System.Convert.ToInt16(line.Substring(1, line.Length - 1));

                }
                
            }
       }

    }
  

}
    