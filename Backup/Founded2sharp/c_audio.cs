using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Microsoft.DirectX.AudioVideoPlayback;
using Microsoft.DirectX.DirectSound;




namespace founded2sharp
{
   public class c_audio
    {
        private c_resource_management pRESOURCES;

        private double lastmusicposition;
        private int actualIndex = -1;
        private ArrayList soundlist = new ArrayList();
        private Microsoft.DirectX.DirectSound.Device pDEVICE;
 


        public c_audio(ref c_resource_management tmpmgm)
        {
            pRESOURCES = tmpmgm;
            pDEVICE = pRESOURCES.SOUNDDEVICE;
        }

        private void managed_soundbuffers()
        {
            if (soundlist.Count > 0)
            {
                for (int i = soundlist.Count - 1; i > -1; i--)
                {
                    SecondaryBuffer currentsound = (SecondaryBuffer)soundlist[i];
                    if (!currentsound.Status.Playing)
                    {
                        currentsound.Dispose();
                        soundlist.RemoveAt(i);
                    }
                }
            }
        }

        public void play_sound(int index)
        {
            SecondaryBuffer newshotsound = pRESOURCES.SOUND[index].Clone(pDEVICE);
            
            newshotsound.Play(0, BufferPlayFlags.Default);
            soundlist.Add(newshotsound);
            managed_soundbuffers();
        }

        public void play_music(int index)
        {
            if (pRESOURCES.MUSIC[index].CurrentPosition == lastmusicposition)
             {
                 pRESOURCES.MUSIC[index].SeekCurrentPosition(0, SeekPositionFlags.AbsolutePositioning);
             }
            lastmusicposition = pRESOURCES.MUSIC[index].CurrentPosition;

             if (actualIndex != index)
             {
                 pRESOURCES.MUSIC[index].Play();
                 actualIndex = index;
             }
        }

        public void stop_music()
        {
            pRESOURCES.MUSIC[actualIndex].Stop();
        }
    }
}
