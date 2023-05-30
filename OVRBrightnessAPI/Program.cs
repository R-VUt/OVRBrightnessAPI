using OVRSharp;
using OVRSharp.Exceptions;
using System;
using System.Net;
using System.Text;
using Valve.VR;
using System.IO;

namespace OVRBrightnessAPI
{
    
    internal class Program
    {
        static float brightness = 0.0f;
        
        static Application ovrApplication = null;
        static Overlay overlay = null;

        static void Main(string[] args)
        {
            try
            {
                ovrApplication = new Application(Application.ApplicationType.Overlay);
            } catch (OpenVRSystemException<Exception> e)
            {
            }
            overlay = new Overlay("BrightnessAPI", "BrightnessAPI", false);

            var overlayBound = new VRTextureBounds_t()
            {
                uMin = 0,
                uMax = 1,
                vMin = 0,
                vMax = 1
            };
            overlay.Alpha = brightness;
            overlay.TextureBounds = overlayBound;
            

            overlay.TrackedDevice = Overlay.TrackedDeviceRole.Hmd;

            HmdMatrix34_t pose;
            int textureYflip = 1;
            var wx = -0f;
            var wy = -0f;
            var wz = -10f;

            pose.m0 = 100; pose.m1 = 0; pose.m2 = 0; pose.m3 = 2*wx;
            pose.m4 = 0; pose.m5 = 100*textureYflip; pose.m6 = 0; pose.m7 = 100*wy;
            pose.m8 = 0; pose.m9 = 0; pose.m10 = 1; pose.m11 = wz;
            overlay.Transform = pose;

            
            overlay.SetTextureFromFile(Path.GetFullPath("./black.jpg"));
            overlay.WidthInMeters = 1f;
            overlay.Show();
            StartHttpServer();

        }
        
        
        static void StartHttpServer()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:13902/");
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                string responseString = "";
                
                
                if (context.Request.Url.AbsolutePath == "/brightness")
                {
                    responseString = brightness.ToString();
                }
                else if (context.Request.Url.AbsolutePath == "/brightness/set")
                {
                    if (context.Request.HttpMethod == "POST")
                    {
                        string requestBody = new System.IO.StreamReader(context.Request.InputStream).ReadToEnd();
                        if (float.TryParse(requestBody, out float newBrightness))
                        {
                            if (float.IsNaN(newBrightness) || newBrightness < 0 || newBrightness > 1)
                            {
                                responseString = "Invalid value";
                            }
                            else
                            {

                                brightness = 1 - newBrightness;
                                overlay.Alpha = brightness;
                                responseString = "Successfully setted";
                            }
                        }
                        else
                        {
                            responseString = "Invalid value";
                        }
                    }
                    else
                    {
                        responseString = "Invalid request";
                    }
                }
                else
                {
                    responseString = "Invalid url";
                }
                
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();


            }
        }
    }
    
  
    
}
