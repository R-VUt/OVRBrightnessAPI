using OVRSharp;
using OVRSharp.Exceptions;
using System;
using System.Net;
using System.Text;
using Valve.VR;
using System.IO;
using CommandLine;
using SharpOSC;


namespace OSCBrightness
{

    internal class Program
    {
        static float brightness = 0.0f;

        static Application ovrApplication = null;
        static Overlay overlay = null;

        class Options
        {
            [Option('p', "port", Default = 9001, HelpText = "Listening Port")]
            public int Port { get; set; }
        }

        static void Main(string[] args)
        {
            try
            {
                ovrApplication = new Application(Application.ApplicationType.Overlay);
            }
            catch (OpenVRSystemException<Exception> e)
            {
                Console.WriteLine("An error occured while initializing OpenVR: " + e.Message);
                return;
            }
            overlay = new Overlay("BrightnessAPI", "BrightnessAPI", false);

            var overlayBound = new VRTextureBounds_t()
            {
                uMin = 0,
                uMax = 1,
                vMin = 0,
                vMax = 1
            };
            overlay.Alpha = brightness;             // -> OpenVR.Overlay.SetOverlayAlpha
            overlay.TextureBounds = overlayBound;   // -> OpenVR.Overlay.SetOverlayTextureBounds


            overlay.TrackedDevice = Overlay.TrackedDeviceRole.Hmd;

            HmdMatrix34_t pose;
            int textureYflip = 1;
            var wx = -0f;
            var wy = -0f;
            var wz = -10f;

            pose.m0 = 100; pose.m1 = 0; pose.m2 = 0; pose.m3 = 2 * wx;
            pose.m4 = 0; pose.m5 = 100 * textureYflip; pose.m6 = 0; pose.m7 = 100 * wy;
            pose.m8 = 0; pose.m9 = 0; pose.m10 = 1; pose.m11 = wz;
            overlay.Transform = pose;


            overlay.SetTextureFromFile(Path.GetFullPath("./black.jpg"));
            overlay.WidthInMeters = 1f;     // OpenVR.Overlay.SetOverlayWidthInMeters
            overlay.Show();
            StartOSCServer();

        }

        static void StartOSCServer()
        {
            var o = new Options();
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var msg = (OscMessage)packet;

                if (msg.Address.Contains("OSCBrightness/set"))
                {
                    try
                    {
                        brightness = (float)msg.Arguments[0];
                    }
                    catch
                    {
                        Console.WriteLine("[ERR] brightness data is not float");
                        return;
                    }


                    if (brightness < 0 || brightness > 1)
                    {
                        Console.WriteLine($"[Err] Brightness Value not in range");
                        return;
                    }


                    Console.WriteLine($"[Hit] change into : {brightness}");
                    //overlay.Alpha = 1 - brightness;
                }
                
            };
            var listener = new UDPListener(9002, callback);


            Console.WriteLine("OSCBrightness V1");
            Console.WriteLine("Press enter to stop");

            Console.ReadLine();
            listener.Close();
        }
       

    }
}
