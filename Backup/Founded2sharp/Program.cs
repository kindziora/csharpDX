using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Founded2sharp
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Form1 gameForm = new Form1();
            gameForm.Show();
            while (gameForm.Created)
            {
                gameForm.check_keys();
                if (gameForm.running)
                {
                    gameForm.Render_Game();
                }
                else
                {
                    break;
                }

                Application.DoEvents();
                
            }
        }
    }
}
