using System;
using System.Configuration;
using System.IO;
using System.Security.Principal;

namespace Backlight
{
    class Program
    {
        static int Main(string[] args)
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                var isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
                if(!isAdmin)
                {
                    Console.WriteLine("Not run as administrator. Set functions will not work.");
                }
            }
            var dllPath = ConfigurationManager.AppSettings["DllPath"];
            if(!File.Exists(dllPath))
            {
                Console.WriteLine($"Samsung keyboard DLL does not exist at {dllPath}. Set the correct path in the App.config file.");
                return -1;
            }

            var kbapi = new SamsungKBAPI(dllPath);
            kbapi.Initialise();

            if(args.Length == 1)
            {
                int brightness = int.Parse(args[0]);
                SetBrightness(kbapi, brightness);
            }
            else
            {
                DisplayInfo(kbapi);
            }

            return 0;
        }

        static void SetBrightness(SamsungKBAPI kbapi, int brightness)
        {
            var result = kbapi.SetBrightness(brightness);
            Console.WriteLine($"SetBrightness({brightness}) returned {result}.");
            brightness = kbapi.GetCurrentBrightness();
            Console.WriteLine("Current brightness: " + brightness);
        }

        static void DisplayInfo(SamsungKBAPI kbapi)
        {
            var t = kbapi.GetKeboardType();
            Console.WriteLine("Keyboard type: " + t);
            var als = kbapi.GetALSExistence();
            Console.WriteLine("Automatic light sensor: " + als);
            var alsStatus = kbapi.GetALSStatus();
            Console.WriteLine("Automatic light sensor status: " + alsStatus);
            var max = kbapi.GetMaximumBrightness();
            Console.WriteLine("Maximum supported brightness: " + max);
            var brightness = kbapi.GetCurrentBrightness();
            Console.WriteLine("Current brightness: " + brightness);
        }
    }
}
