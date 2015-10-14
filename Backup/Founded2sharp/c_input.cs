using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.DirectInput;
using System.Diagnostics;

namespace founded2sharp
{
   public class c_input
    {
        
        private Microsoft.DirectX.DirectInput.Device keyboard = null;
        private Microsoft.DirectX.DirectInput.Device mouse = null;
        private MouseState state;
        public int X;
        public int Y;

        public c_input(IntPtr hwnd,bool window)
        {
            keyboard = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            

            mouse = new Device(SystemGuid.Mouse);
            mouse.Properties.AxisModeAbsolute = false;
            if (window)
            {
                mouse.SetCooperativeLevel(hwnd, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);
                keyboard.SetCooperativeLevel(hwnd, CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);
            
            }
            else
            {
                mouse.SetCooperativeLevel(hwnd, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.Exclusive);
                keyboard.SetCooperativeLevel(hwnd, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Foreground);
            
            }
            
            mouse.Acquire();
            keyboard.Acquire();

            this.X = 1024 / 2;
            this.Y = 768 / 2;
           
        }

        public void free()
        {
            mouse.Unacquire();
            keyboard.Unacquire();
        }

        public Key[] GetPressedKey()
        {
           return keyboard.GetPressedKeys();
        }

        public void pollMouse()
        {
           this.state = mouse.CurrentMouseState;
           if (this.X >= 0 - this.state.X && this.X <= 1024 - this.state.X) this.X += this.state.X;
           if (this.Y >= 0 - this.state.Y && this.Y <= 768 - this.state.Y) this.Y += this.state.Y;
        }

        public int sX()
        {
            return this.state.X;
        }

        public int sY()
        {
            return this.state.Y;
        }

        public byte[] mouseButton()
        {
           return this.state.GetMouseButtons();
        }
    }
}
