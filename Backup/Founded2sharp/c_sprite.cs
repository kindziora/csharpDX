//includes der nötigen libs
using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Collections;
using System.Timers;
///////////////////////////////////////
namespace founded2sharp
{
    
    public class c_sprite
    {
        public RectangleF box = new RectangleF();
        public RectangleF Destbox = new RectangleF();
        /// <summary>
        /// /////////////////////BounceBox
        /// </summary>
        public Polygon HitBox = new Polygon();
      

        /// <summary>
        /// //////////BounceBox
        /// </summary>

        public Color Color;
        public float rotationAngle;
        public PointF rotationCenter = new PointF();
        public Vector speed = new Vector();
        public float Zebene = 0.0f;

        public readonly int Texture_Typ;
        public int Texture_id;

        public c_sprite(int textur_resource_id,int id) // konstruktor
        {
            Texture_Typ = textur_resource_id;
            Texture_id = id;
            
        }

        

    }

    

    public class c_LOV : c_sprite
    {
        public Rectangle srcRect;
        private Rectangle[,] spriteRect;

        public int counter = 0;
        private int spritePointer = 0;
        public int anispeed = 3;
        public int direction = 2;
        public bool caminar = false;
        /// <summary>
        /// ZIEL POSITION//////
        /// </summary>
        public Point AIM = new Point();
        public PointF AIMF = new PointF();
        /// <summary>
        /// ///////AKTUELLE POSITION/////////////
        /// </summary>
        public Point sp = new Point();
        public PointF spF = new PointF();

        public Vector3[] weg;
        public int schrittPosition = 0;
        public static int countObj = 0;
        public int attackcnt = 1;

        /// <summary>
        /// /////////////////LIVING OBJECT EIGENSCHAFTEN/////////////////////////////
        /// </summary>
        public struct eigenschaften
        {
            public string name;
            public bool evil;
            public int staerke;
            public int livepoints;
            public int defensivpoints;
            public int mana;
            public int expirience;
            public int level;
            public string klasse;
            public int attackSpeed;
            public int attackRange;
            public int maxlivepoints;
        }

        public eigenschaften properties = new eigenschaften();

        /////////////////////////////////////////////////////////////////////////////
        private System.Timers.Timer AttackTimer = new System.Timers.Timer();
        public bool canAttack = true;

        public c_LOV(int textur_resource_id, int id,int zeile,int spalte,int Bsize) : base(textur_resource_id, id)
        {

            this.AttackTimer.Start();
            this.AttackTimer.AutoReset = false;
            this.AttackTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.makeAttack);
            /// character set laden 
            
            spriteRect = new Rectangle[zeile, spalte];
            for (int y = 0; y < zeile; y++)
           {
               for (int x = 0; x < spalte; x++)
               {
                   spriteRect[y, x].Y = y * 96;
                   spriteRect[y, x].X = x * 96;
                   spriteRect[y, x].Width = 96;
                   spriteRect[y, x].Height = 96;
               }
           }
            this.setSprite(0, 0);
            countObj++;

        }

        public void stop()
        {
            this.AttackTimer.Stop();
        }
        public void start()
        {
            this.AttackTimer.Start();
        }

        private void makeAttack(object sender, EventArgs e)
        {
            canAttack = true;
            AttackTimer.Interval = this.properties.attackSpeed;
        }

       
        public void move(int tdirection)
        {
            if (tdirection != -1)
            {
                if (this.caminar)
                {
                    this.direction = tdirection;
                    this.counter++;
                    //charakter bewegen und animieren
                    if (this.counter > this.anispeed)
                    {
                        this.setSprite(direction, spritePointer);
                        this.spritePointer++;
                        if (this.spritePointer >= 7)
                        {
                            this.spritePointer = 0;
                            this.caminar = false;
                        }
                        this.counter = 0;
                    }
                }
            }
            else
            {
                this.setSprite(2, 2);
            }
        }

        public void setSprite(int Zeile ,int spalte)
        {
            //sprite wechseln
            this.srcRect = this.spriteRect[Zeile, spalte]; 
        }

        public bool checkplayerColl(ref c_npc otherPlayer)
        {
            if (this.Destbox.IntersectsWith(otherPlayer.Destbox) == true)
            {
                otherPlayer.Color = Color.Red;
                if (this.Destbox.Y >= otherPlayer.Destbox.Y)
                {
                    this.Zebene = 0.2f;
                }
                else
                {
                    this.Zebene = 0.6f;
                }
                return true;
            }
            otherPlayer.Color = Color.White;
            return false;
        }
    }


    

    public class c_npc : c_LOV
    {
        private System.Timers.Timer Movetimer = new System.Timers.Timer();
        private Random rnd;
        private int npcNummer;
        private c_messages msgs = new c_messages();

        public c_npc(int textur_resource_id, int id, int zeile, int spalte, int Bsize,string pfad)
            : base( textur_resource_id, id,  zeile,  spalte,  Bsize)
        {
            
            this.Movetimer.Elapsed += new System.Timers.ElapsedEventHandler(this.moving);
            this.Movetimer.Enabled = true;
            this.npcNummer = countObj;
           // this.msgs.load_msgs(pfad + "npc1.txt");
        }


        public void set_interval(bool enabled)
        {
            if (enabled)
                Movetimer.Start();
            else
                Movetimer.Stop();
        }
        public bool get_interval()
        {
            return Movetimer.Enabled;
        }

        public string get_random_Message()
        {
            return "null"; // msgs.msg[rnd.Next(3), 0];
        }

        public void moving(object sender, EventArgs e)
        {
            this.rnd = new Random();
            this.direction = this.rnd.Next(-1, 7);
            this.Movetimer.Interval = this.rnd.Next(2500, 4500) + this.npcNummer / 10;
        }

    }
}