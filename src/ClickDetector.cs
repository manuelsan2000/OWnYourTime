﻿using Gma.System.MouseKeyHook;
using System;

namespace OWn_Your_Time
{
    public class ClickDetector
    {


        public static void ListenForMouseEvents()
        {
            Console.WriteLine("Listening to mouse clicks.");

            //When a mouse button is pressed 
            Hook.GlobalEvents().MouseDown += async (sender, e) =>
            {
                //Console.WriteLine($"Mouse {e.Button} Down");
                updateValues();
            };
            //When a double click is made
            Hook.GlobalEvents().MouseDoubleClick += async (sender, e) =>
            {
                //Console.WriteLine($"Mouse {e.Button} button double clicked.");
                updateValues();
            };

            //When a move is made
            Hook.GlobalEvents().MouseMove += async (sender, e) =>
            {
                //Console.WriteLine($"Mouse moved X: {e.X} Y: {e.Y} ");
                updateValues();
            };
        }

        public static void updateValues()
        {
            ConfigValues.lastActivityMillis = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

    }
}
