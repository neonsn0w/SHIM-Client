using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ClientGUI_MultipleClientsChatTest
{
    internal class SoundTools
    {
        public static void playLogonSound()
        {
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "dooropen.wav");
            SoundPlayer simpleSound = new SoundPlayer(soundFilePath);
            try
            {
                simpleSound.Play();
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine("Sound file not found");
            }
        }

        public static void playLogoffSound()
        {
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "doorslam.wav");
            SoundPlayer simpleSound = new SoundPlayer(soundFilePath);
            try
            {
                simpleSound.PlaySync();
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine("Sound file not found");
            }
        }

        public static void playIMSendSound()
        {
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "imsend.wav");
            SoundPlayer simpleSound = new SoundPlayer(soundFilePath);
            try
            {
                simpleSound.Play();
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine("Sound file not found");
            }
        }

        public static void playIMRcvSound()
        {
            string soundFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "imrcv.wav");
            SoundPlayer simpleSound = new SoundPlayer(soundFilePath);
            try
            {
                simpleSound.Play();
            }
            catch (FileNotFoundException fne)
            {
                Console.WriteLine("Sound file not found");
            }
        }
    }
}
