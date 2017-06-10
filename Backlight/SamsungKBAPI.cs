using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Backlight
{
    class SamsungKBAPI
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        IntPtr DllModule;

        public SamsungKBAPI(string dllPath)
        {
            var dir = Path.GetDirectoryName(dllPath);
            var dll = Path.GetFileName(dllPath);

            /* The keyboard library requires other DLL files in the same directory,
               so just calling SetDllDirectory() did not work.
             */
            Environment.CurrentDirectory = dir;
            DllModule = LoadLibrary(dllPath);
            if (DllModule == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        Dictionary<string, Delegate> FunctionCache = new Dictionary<string, Delegate>();
        Delegate GetFunction<T>(string name)
        {
            Delegate func = null;

            if(!FunctionCache.TryGetValue(name, out func))
            {
                var functionAddress = GetProcAddress(DllModule, name);
                func = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
                FunctionCache[name] = func;
            }

            return func;
        }

        #region Delegates
        delegate void Arg0Ret0();
        delegate int Arg0RetInt();

        //https://stackoverflow.com/questions/3307000/calling-a-c-function-from-c-sharp-unbalanced-stack
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate int ArgIntRetInt(int arg0);

        void RunArg0Ret0(string name)
        {
            var func = GetFunction<Arg0RetInt>(name);
            func.DynamicInvoke();
        }

        int RunArg0RetInt(string name)
        {
            var func = GetFunction<Arg0RetInt>(name);
            return (int)func.DynamicInvoke();
        }

        int RunArgIntRetInt(string name, int arg0)
        {
            var func = GetFunction<ArgIntRetInt>(name);
            return (int)func.DynamicInvoke(arg0);
        }
        #endregion

        public int GetCurrentBrightness()
        {
            return RunArg0RetInt("MODULEKEYBOARDBACKLIT_GetBrightnessCurrentLevel");
        }

        public int Initialise()
        {
            return RunArg0RetInt("MODULEKEYBOARDBACKLIT_Initialize");
        }

        public int SetBrightness(int brightness)
        {
            return RunArgIntRetInt("MODULEKEYBOARDBACKLIT_SetBrightnessLevel", brightness);
        }

        public int SetBrightnessWithoutSaving(int brightness)
        {
            return RunArgIntRetInt("MODULEKEYBOARDBACKLIT_SetBrightnessLevelWithoutSave", brightness);
        }

        public int GetMaximumBrightness()
        {
            return RunArg0RetInt("MODULEKEYBOARDBACKLIT_GetBrightnessMaxLevel");
        }

        public int GetStatus(int arg0)
        {
            return RunArgIntRetInt("MODULEKEYBOARDBACKLIT_GetStatus", arg0);
        }

        public bool GetALSExistence()
        {
            var exists = false;
            if(RunArg0RetInt("MODULEKEYBOARDBACKLIT_GetALSExistence")==0)
            {
                exists = true;
            }
            return exists;
        }

        public int GetALSStatus()
        {
            return RunArg0RetInt("MODULEKEYBOARDBACKLIT_GetKBDALSStatus");
        }

        public void ToggleBacklight()
        {
           RunArg0RetInt("MODULEKEYBOARDBACKLIT_SetToggleBacklit");
        }

        public int GetKeboardType()
        {
            return RunArg0RetInt("MODULEKEYBOARDBACKLIT_GetType");
        }

        public int SetIdleLevel(int level)
        {
            return RunArgIntRetInt("MODULEKEYBOARDBACKLIT_SetIdleLevel", level);
        }
    }
}
