using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using System.IO;


namespace founded2sharp
{
    public class XSprite
    {
        private VertexBuffer VERTEXBUFFER = null;
        private Microsoft.DirectX.Direct3D.Device DEVICE;
        public float zoom = 0;

        public XSprite(ref Microsoft.DirectX.Direct3D.Device sender)
        {
            DEVICE = sender;
            // Now Create the VB
            VERTEXBUFFER = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, DEVICE, Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            DEVICE.RenderState.CullMode = Cull.None;
            // Turn off D3D lighting
            DEVICE.RenderState.Lighting = false;
            // Turn on the ZBuffer
            DEVICE.RenderState.ZBufferEnable = true;

            this.SetupMatrices();
        }

       
        private void SetupMatrices()
       	{
            DEVICE.Transform.View = Matrix.LookAtLH(new Vector3(0.0f, 3.0f, -5.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
            DEVICE.Transform.Projection = Matrix.OrthoLH(9.5f, 4.5f, 1.0f, -100.0f);
        }

        public void Draw2D(ref Sprite sprite, ref Texture Tex, ref Rectangle srcRect, ref RectangleF destRect, Color col, float Z, float zRotation)
        {

            Matrix transformation = Matrix.Scaling((destRect.Width / srcRect.Width) + zoom, (destRect.Height / srcRect.Height) + zoom, 1)

                  //rotate about the z axis (into the screen) of the centered sprite
                * Matrix.RotationZ(zRotation)
                //translate to the final position
                * Matrix.Translation(destRect.X, destRect.Y, Z);

            //set the transformation
            sprite.Transform = transformation;
     
            sprite.Draw(Tex, srcRect, Vector3.Empty, Vector3.Empty, col);
        }

        public void Draw(ref Texture texture, float xoffset, float yoffset)
        {
            xoffset = xoffset / 100;
            yoffset = yoffset / 100;
           
            // Setup our texture. Using textures introduces the texture stage states,
            // which govern how textures get blended together (in the case of multiple
            // textures) and lighting information. In this case, we are modulating
            // (blending) our texture with the diffuse color of the vertices.
            // Create a vertex buffer (100 customervertex)
            
            CustomVertex.PositionTextured[] verts = (CustomVertex.PositionTextured[])VERTEXBUFFER.Lock(0, 0); // Lock the buffer (which will return our structs)
            verts[0] = new CustomVertex.PositionTextured(-10f + xoffset, 0.0f - yoffset, 0.0f, 0.0f, 0.0f);
            verts[1] = new CustomVertex.PositionTextured(0.0f + xoffset, -5.0f - yoffset, 0.0f, 0.0f, 1.0f);
            verts[2] = new CustomVertex.PositionTextured(10f + xoffset, 0.0f - yoffset, 0.0f, 1.0f, 1.0f);
            verts[3] = new CustomVertex.PositionTextured(0.0f + xoffset, 5.0f - yoffset, 0.0f, 1.0f, 0.0f);
            // Unlock (and copy) the data
            VERTEXBUFFER.Unlock();

            

            DEVICE.SetTexture(0, texture);
          
            DEVICE.SetStreamSource(0, VERTEXBUFFER, 0);
            DEVICE.VertexFormat = CustomVertex.PositionTextured.Format;
            DEVICE.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        }

    }

   public class c_Xcore //game core beinhaltet alles was das game braucht 
    {
       /////////////////////////////////DEKLARATIONEN//////////////
       private c_resource_management pRESOURCES;//POINTER AUF RESOURCE OBJEKT
       public c_input INPUT;
       public c_engine MATHENGINE;
       public c_audio AUDIO;
       public bool PAUSE = false;

       private int ac = 0;

       ////////////////////////////////////////////////////////////

       public Microsoft.DirectX.Direct3D.Device DEVICE = null; // Our rendering device
       private Sprite ZEICHENSPRITE; //SPRITE DAS ZUM DARSTELLEN GENUTZT WIRD

       private int framecount = 0;
       private int framesperSEC = 70;
      
        public c_Xcore(ref c_resource_management trmgm)
        {
            pRESOURCES = trmgm; //ZUWEISEN
            pRESOURCES.HWND = GameDefinition.hwnd;
            this.initialisiere_Game_Device();
            this.init_Klassen();
            
        }
    
        private void initialisiere_Game_Device()
        {
            PresentParameters presentParams = new PresentParameters(); //darstellung parameterliste

            presentParams.Windowed = GameDefinition.window; //im fenster 

            if (!presentParams.Windowed)
            {
                presentParams.BackBufferWidth = GameDefinition.width;
                presentParams.BackBufferHeight = GameDefinition.height;
            }

            presentParams.AutoDepthStencilFormat = DepthFormat.D16; // And the stencil format
            presentParams.SwapEffect = SwapEffect.Discard; // swapping ist die darstellungsart für die darstellungspuffer
            presentParams.BackBufferFormat = Format.X8R8G8B8;


            DEVICE = new Microsoft.DirectX.Direct3D.Device(0, Microsoft.DirectX.Direct3D.DeviceType.Hardware, GameDefinition.hwnd, CreateFlags.NoWindowChanges | CreateFlags.HardwareVertexProcessing, presentParams);
            
    
        }

        private void init_Klassen()
        {
            MATHENGINE = new c_engine(); //berechnungen für die spielelogik etc
            INPUT = new c_input(GameDefinition.hwnd, GameDefinition.window);
            AUDIO = new c_audio(ref pRESOURCES);
            ZEICHENSPRITE = new Sprite(DEVICE); // einen sprite für das darstellungsgerät erstellen
        }

        public string key_string()
        {
            if (INPUT == null) return "";
            Key[] keys = INPUT.GetPressedKey();
            string valuecode = "";
            foreach (Key k in keys)
            {
               valuecode+=k.ToString();
            }
            return valuecode;
        }

        public int getFramerate()
        {
            if (FrameworkTimer.GetTime() >= 1.0)
            {
                FrameworkTimer.Reset();
                this.framesperSEC = this.framecount;
                this.framecount = 0;
            }
            this.framecount++;
            return this.framesperSEC;
        }

        private int check_way(int notCheck,ref Vector3[]weg,ref Point position,int o)
        {
            int i = 0;
            if (o > 0) i = o - 1;

            if (notCheck != 3)
            {
                if (pRESOURCES.detailmap[position.Y - 1, position.X].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
                {
                    weg[i].X = position.X - 1;
                    weg[i].Y = position.Y;
                return 3;
                }
            }
            if (notCheck != 0)
            {
                if (pRESOURCES.detailmap[position.Y + 1, position.X].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
                {
                    weg[i].X = position.X + 1;
                    weg[i].Y = position.Y;
                return 0;
                }
            }
            if (notCheck != 1)
            {
                if (pRESOURCES.detailmap[position.Y, position.X - 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
                {
                    weg[i].X = position.X;
                    weg[i].Y = position.Y - 1;
                return 1;
                }
            }
            if (notCheck != 2)
            {
                if (pRESOURCES.detailmap[position.Y, position.X + 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
                {
                    weg[i].X = position.X;
                    weg[i].Y = position.Y + 1;
                return 2;
                }
            }
           if (notCheck != 4)
            {
                if (pRESOURCES.detailmap[position.Y + 1, position.X - 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
            {
                weg[i].X = position.X + 1;
                weg[i].Y = position.Y - 1;
                return 4;
            }
           }
        if (notCheck != 5)
            {
                if (pRESOURCES.detailmap[position.Y - 1, position.X - 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
            {
                weg[i].X = position.X - 1;
                weg[i].Y = position.Y - 1;
                return 5;
            }
        }
           if (notCheck != 6)
            {
                if (pRESOURCES.detailmap[position.Y + 1, position.X + 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
            {
                weg[i].X = position.X + 1;
                weg[i].Y = position.Y + 1;
                return 6;
            }
           }
           if (notCheck != 7)
            {
                if (pRESOURCES.detailmap[position.Y - 1, position.X + 1].detailmap == -1 && MATHENGINE.vectorToPoint(weg[i]) != position)
                {
                    weg[i].X = position.X - 1;
                    weg[i].Y = position.Y + 1;
                    return 7;
                }
           }
           return 0;
        }
        
        public void get_Path(PointF position, PointF ziel, ref Vector3[] weg)
        {
            weg = new Vector3[25];
            PointF distanz = MATHENGINE.PointFToPointF(position, ziel);
            

            if (distanz.X < 0 && distanz.Y > -50 && distanz.Y < 50)
                weg[0].Z = 3;
            
            if (distanz.X > 0 && distanz.Y > -50 && distanz.Y < 50)
                weg[0].Z = 0;
            
            if (distanz.Y < 0 && distanz.X > -50 && distanz.X < 50)
                weg[0].Z = 1;
            
            if (distanz.Y > 0 && distanz.X > -50 && distanz.X < 50)
                weg[0].Z = 2;
            


            if (distanz.X < -50 && distanz.Y < -50)
                weg[0].Z = 5;
            
            if (distanz.X < -50 && distanz.Y > 50)
                weg[0].Z = 7;
            
            if (distanz.X > 50 && distanz.Y < -50)
                weg[0].Z = 4;
            
            if (distanz.X > 50 && distanz.Y > 50)
                weg[0].Z = 6;
           
        }

        public void get_Path(Point position, Point ziel, ref Vector3[] weg)
        {
  
          
            int i = 0;
            bool ende = false;

            
            weg = new Vector3[25];
          
            while (!ende)
            {
               
                Point distanz = MATHENGINE.PointToPoint(position, ziel);
                int direction = 0;

                if (distanz.X < 0)
                {
                    direction = 3;
                }
                if (distanz.X > 0)
                {
                    direction = 0;
                }
                if (distanz.Y < 0)
                {
                    direction = 1;

                    if (distanz.X < 0)
                    {
                        direction = 5;
                    }
                    if (distanz.X > 0)
                    {
                        direction = 4;
                    }
                }

                if (distanz.Y > 0)
                {
                    direction = 2;

                    if (distanz.X < 0)
                    {
                        direction = 7;
                    }
                    if (distanz.X > 0)
                    {
                        direction = 6;
                    }
                }

                weg[i].Z = direction;

                //////////////////////////////
                
              
                return;
                //////////////////////////////

                switch (direction)
                {
                    case -1:
                        ende = true;
                        break;
                    case 3:
                        if (pRESOURCES.detailmap[position.Y - 1, position.X].detailmap == -1)
                        {
                            weg[i].X = position.X - 1;
                            weg[i].Y = position.Y;
                        }
                        else
                        {
                            //weg[i].Z = this.check_way(3, ref weg, ref position, i);
                        }
                        break;
                    case 0:
                        if (pRESOURCES.detailmap[position.Y + 1, position.X].detailmap == -1){
                            weg[i].X = position.X + 1;
                            weg[i].Y = position.Y;
                         }
                        else
                        {
                            //weg[i].Z = this.check_way(0, ref weg, ref position, i);
                        }
                        break;
                    case 1:
                        if (pRESOURCES.detailmap[position.Y, position.X - 1].detailmap == -1){
                            weg[i].X = position.X;
                            weg[i].Y = position.Y - 1;
                        }
                        else
                        {
                            //weg[i].Z = this.check_way(1, ref weg, ref position, i);
                        }
                        break;
                    case 2:
                        if (pRESOURCES.detailmap[position.Y, position.X + 1].detailmap == -1){
                            weg[i].X = position.X;
                            weg[i].Y = position.Y + 1;
                             }
                        else
                        {
                            //weg[i].Z = this.check_way(2, ref weg, ref position, i);
                        }
                        break;



                    case 4:
                        if (pRESOURCES.detailmap[position.Y + 1, position.X - 1].detailmap == -1){
                            weg[i].X = position.X + 1;
                            weg[i].Y = position.Y - 1;
                             }
                        else
                        {
                            //weg[i].Z = this.check_way(4, ref weg, ref position, i);
                        }
                        break;
                    case 5:
                        if (pRESOURCES.detailmap[position.Y - 1, position.X - 1].detailmap == -1){
                            weg[i].X = position.X - 1;
                            weg[i].Y = position.Y - 1;
                             }
                        else
                        {
                            //weg[i].Z = this.check_way(5, ref weg, ref position, i);
                        }
                        break;
                    case 6:
                        if (pRESOURCES.detailmap[position.Y + 1, position.X + 1].detailmap == -1){
                            weg[i].X = position.X + 1;
                            weg[i].Y = position.Y + 1;
                             }
                        else
                        {
                            //weg[i].Z = this.check_way(6, ref weg, ref position, i);
                        }
                        break;
                    case 7:
                        if (pRESOURCES.detailmap[position.Y - 1, position.X + 1].detailmap == -1){
                            weg[i].X = position.X - 1;
                            weg[i].Y = position.Y + 1;
                             }
                        else
                        {
                            //weg[i].Z = this.check_way(7, ref weg, ref position, i);
                        }
                        break;

                }

                position.X = (int)weg[i].X;
                position.Y = (int)weg[i].Y;

                //pRESOURCES.detailmap[position.Y, position.X].detailmap = 0;

                if (ende == true && position != ziel || i>20)
                {
                    //kein weg gefunden
                    int o = weg.Length;
                    weg = null;
                    break;
                }

                if (position == ziel)
                {
                    //weg gefunden
                   int o =  weg.Length;
                    break;
                }
               
                i++;
            }
        }

        
       public bool  fight_p2p(ref c_npc npc, ref c_LOV player) // player wird angegriffen
            {
                int direction = 0;
                
                if (npc.Destbox.X < player.Destbox.X)
                {
                    direction = 8;
                }
                else
                {
                    direction = 9;
                }

                if (npc.canAttack)
                {
                    player.properties.livepoints -= npc.properties.staerke;
                    npc.canAttack = false;
                    AUDIO.play_sound(1);
                    if (player.properties.livepoints <= 0) // npc besiegt
                    {
                        player.Color = Color.Blue;
                        
                        AUDIO.play_sound(3);
                    }
                    npc.attackcnt = 1;
                    
                    return true;
                }
                else
                {
                    if(10 * npc.attackcnt <= npc.properties.attackSpeed)
                    {
                        npc.attackcnt++;
                    } else {
                        ac++;
                    }
                       if (ac >= 7)
                        ac = 0;
                    npc.setSprite(direction, ac);
                    
                    npc.caminar = false;
                    
                }

            return false;
        }
       
       public bool fight_p2p(ref c_npc npc, ref c_LOV player,bool playerAttacks)
        {

            if (playerAttacks) //player greift an
            {
                int direction = 0;
                if (player.Destbox.X < npc.Destbox.X)
                {
                    direction = 8;
                }
                else
                {
                    direction = 9;
                }
                if (player.canAttack)
                {
                    npc.properties.livepoints -= player.properties.staerke;
                    player.canAttack = false;
                    AUDIO.play_sound(1);
                    if (npc.properties.livepoints <= 0) // npc besiegt
                    {
                        npc = null;
                        AUDIO.play_sound(2);
                    }
                    player.attackcnt = 1;
                    player.setSprite(direction, 0);
                    return true;
                }
                else
                {
                    int ac = (int)(player.attackcnt / (player.properties.attackSpeed / ((player.properties.attackSpeed / 100) * 10)));
                    if (ac >= 7) ac = 0;
                        player.setSprite(direction, ac);
                        player.attackcnt++;

                }

            }
  
            return false;
        }
   
   }


}
