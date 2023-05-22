using System;
using System.Net;
using System.Text;
using Valve.VR;

namespace OVRBrightnessAPI
{
    internal class Program
    {
        static float brightness = 0.5f;
        static ulong overlayHandle = 0;

        static void Main(string[] args)
        {
            CVRSystem system = OpenVR.System;

            try
            {
                EVROverlayError overlayError = OpenVR.Overlay.CreateOverlay("OVRBrightnessAPI", "OVRBrightnessAPI", ref overlayHandle);
                if (overlayError != EVROverlayError.None)
                {
                    Console.WriteLine("Failed to create overlay: " + overlayError.ToString());
                    return;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to create overlay: " + e.Message);
                Console.ReadLine();
                return;
            }

            OpenVR.Overlay.SetOverlayTextureColorSpace(overlayHandle, EColorSpace.Gamma);
            var bound = new VRTextureBounds_t()
            { uMin = 0, uMax = 1, vMin = 0, vMax = 1 };
            OpenVR.Overlay.SetOverlayTextureBounds(overlayHandle, ref bound);

            OpenVR.Overlay.ShowOverlay(overlayHandle);

            StartHttpServer();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();

            OpenVR.Overlay.DestroyOverlay(overlayHandle);
            OpenVR.Shutdown();

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
                            brightness = newBrightness;
                            OpenVR.Overlay.SetOverlayAlpha(overlayHandle, brightness);
                            responseString = "Successfully setted";
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

                // 응답 전송
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();


            }
        }
    }
    
  
    
}
