using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace founded2sharp
{
    public class c_point_sprite
    {
        //private Texture TTexture;
        private Device Srbuffer;
        public int count;

        private myownvertexformat[] ParticleInfo;
        public enviroment world;
        public bool alive = true;
        

        public struct enviroment
        {
            public float gravityX;
            public float gravityY;
        }



        public struct myownvertexformat
        {
            public Vector2 speed;
            public int lifetime;
            public int lifecount;
        }
        CustomVertex.TransformedColored[] vert;


        public void add_particle(float X, float Y, int color, Vector2 speed,int lifetime)
        {
            if (count + 1 < ParticleInfo.Length)
            {
                count++;
                ParticleInfo[count] = new myownvertexformat();
                ParticleInfo[count].speed = speed;
                ParticleInfo[count].lifetime = lifetime;
                ParticleInfo[count].lifecount = 0;
                vert[count]                       = new CustomVertex.TransformedColored(X, Y, 0, 0, color);
            }
        }

        public void remove_particle(int index)
        {
            if (this.count <= 1)
                this.alive = false;
            this.count--;
            if (this.count - 1 >= 0)
            {
                
                if (index == -1) index = count-1;
                if (index >= 0)
                {
                    ParticleInfo[index] = new myownvertexformat();
                    vert[index] = new CustomVertex.TransformedColored();

                    //for (int i = index; i < count; i++)
                    //{
                    //    ParticleInfo[i] = ParticleInfo[i + 1];
                    //    vert[i] = vert[i + 1];
                    //}
                }
            }
        }




        public c_point_sprite(ref Device tmpScreening, ref Texture textur,int max)
        {
            Srbuffer = tmpScreening;
            //TTexture = textur;
            ParticleInfo = new myownvertexformat[max];
            vert = new CustomVertex.TransformedColored[max];
            this.world.gravityY = 0.4f;
         
        }

        public c_point_sprite()
        {
           
        }

        public void draw_particle()
        {
            //Srbuffer.RenderState.AlphaBlendEnable = true;
            //Srbuffer.RenderState.AlphaSourceBlend = Blend.SourceAlpha;
            //Srbuffer.RenderState.AlphaDestinationBlend = Blend.InvSourceAlpha;

            for (int i = 0; i < count; i++)
            {
                UpdatePosition(ref vert[i], ref ParticleInfo[i], i);
            }

            if (count > 0)
            {
                Srbuffer.VertexFormat = CustomVertex.TransformedColored.Format;
                // Srbuffer.SetTexture(0, TTexture);
               

                Srbuffer.DrawUserPrimitives(PrimitiveType.PointList, count, vert);

            }
        }

        public virtual void UpdatePosition(ref CustomVertex.TransformedColored vt, ref myownvertexformat Particle, int i)
        {
           //dummy hier kommt die animation rein
        }
    }

    public class c_explosion : c_point_sprite
    {
        private Vector2 center;

        public void set_zentrum(float x,float y)
        {
            this.center = new Vector2(x , y);
        }

        public Vector2 get_zentrum()
        {
            return this.center;
        }

        public c_explosion()
        {

        }
        public c_explosion(ref Device tmpScreening, ref Texture textur, int max) : base(ref tmpScreening, ref textur, max)
        {

        }

        public override void UpdatePosition(ref CustomVertex.TransformedColored vt, ref myownvertexformat Particle,int i)
        {
           
                vt.X += Particle.speed.X + this.world.gravityX;
                vt.Y += Particle.speed.Y + (this.world.gravityY * Particle.lifecount/5f);
 
                Particle.lifecount++;
                if (Particle.lifecount >= Particle.lifetime )
                {
                    this.remove_particle(i);
                   

                }
            
        }

        public void generate_explosion(int range,int countt, float X, float Y, int color, Vector2 speed,float gravY)
        {
            Random rnd = new Random();
            Vector2 spd = new Vector2();
            this.count = 0;
            this.world.gravityY = gravY;
            for (int i = 0; i < countt; i++)
            {
                spd.X = (speed.X + (float)(Math.Sqrt(rnd.Next(i))) - (float)(Math.Sqrt(rnd.Next(i)))) / 15f;
                spd.Y = (speed.Y + (float)(Math.Sqrt(rnd.Next(i))) - (float)(Math.Sqrt(rnd.Next(i)))) / 15f;
                this.add_particle(X + spd.X, Y + spd.Y, color, spd, range + rnd.Next(0, range));
            }
            
        }

    }

    public static class c_regen
    {
        private static Device Srbuffer;
        private static CustomVertex.TransformedColored[] vert;
        private static int[] lineListIndices;

        public static void generate_rain(int level, ref Device def, int color)
        {
            Srbuffer = def;
            vert = new CustomVertex.TransformedColored[level];
            Random rnd = new Random();
            lineListIndices = new int[(level * 2)];

            for (int i = 0; i < level; i++)
            {
                vert[i] = new CustomVertex.TransformedColored(rnd.Next(1000), 0, 0, 0, color);
                lineListIndices[i * 2] = (int)(i + 1);
                lineListIndices[(i * 2) + 1] = (int)(i + 2);
            }

            lineListIndices[(level * 2) - 1] = 1;

        }

       
        private static void update_regen()
        {
            for (int i = 0; i < vert.Length; i++)
            {

                if (vert[i].Y < 768)
                {
                    vert[i].Y++;
                }
                else
                {
                    Random rnd = new Random();
                    vert[i].Y = 0;
                    vert[i].X = rnd.Next(1000);
                }

            }
        }

        public static void draw_regen()
        {
            update_regen();
            Srbuffer.VertexFormat = CustomVertex.TransformedColored.Format;
            Srbuffer.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, vert.Length + 1, vert.Length, lineListIndices,false,vert);
 
        }

    }

    public class c_line_list
    {
        private Device Srbuffer;
        private CustomVertex.TransformedColored[] vert;

        public c_line_list(ref Device bf, PointF p1, PointF p2, int color)
        {

            this.Srbuffer = bf;
            this.vert = new CustomVertex.TransformedColored[2];
            this.vert[0] = new CustomVertex.TransformedColored(p1.X, p1.Y, 0, 0, color);
            this.vert[1] = new CustomVertex.TransformedColored(p2.X, p2.Y, 0, 0, color);
        }

        public c_line_list(ref Device bf, float X1, float Y1, float X2, float Y2, int color)
        {
            
            this.Srbuffer = bf;
            this.vert = new CustomVertex.TransformedColored[2];
            this.vert[0] = new CustomVertex.TransformedColored(X1, Y1, 0, 0, color);
            this.vert[1] = new CustomVertex.TransformedColored(X2, Y2, 0, 0, color);
        }
        
        public void draw_line(PointF p1,PointF p2)
        {

            this.Srbuffer.VertexFormat = CustomVertex.TransformedColored.Format;
            this.vert[0].X = p1.X; 
            this.vert[0].Y = p1.Y;

            this.vert[1].X = p2.X;
            this.vert[1].Y = p2.Y;

            this.Srbuffer.DrawUserPrimitives(PrimitiveType.LineList, this.vert.Length, this.vert);

        }

    }


}
