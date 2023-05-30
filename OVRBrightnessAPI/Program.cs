using OVRSharp;
using OVRSharp.Exceptions;
using System;
using System.Net;
using System.Text;
using System.Threading;
using Valve.VR;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using SharpDX.DXGI;

namespace OVRBrightnessAPI
{
    internal class Program
    {
        static float brightness = 0.5f;
        
        
        static Application ovrApplication = null;
        static Overlay overlay = null;
        static Texture2D texture;



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

            var _device = new Device(SharpDX.Direct3D.DriverType.Hardware, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded);
            texture = new Texture2D(
                _device,
                new Texture2DDescription
                {
                    Width = 512,
                    Height = 512,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Dynamic,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.Write
                });

            var _overlayTexture = new Texture_t
            {
                eColorSpace = EColorSpace.Gamma,
                handle = texture.NativePointer
            };
            overlay.SetTexture(_overlayTexture);

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
                            brightness = newBrightness;
                            overlay.Alpha = brightness;
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
