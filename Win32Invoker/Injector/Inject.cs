using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

//This is used to actually inject the specific DLL into the selected process 

namespace Injector
{
  
    public static class Inject
    {   
        [Flags]
            enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VMOperation = 0x00000008,
                VMRead = 0x00000010,
                VMWrite = 0x00000020,
                DupHandle = 0x00000040,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                Synchronize = 0x00100000
            }
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
        private static class WINAPI
        {
         
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(
                ProcessAccessFlags dwDesiredAccess,
                [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
                UInt32 dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern Int32 CloseHandle(
                IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetProcAddress(
                IntPtr hModule,
                string lpProcName);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetModuleHandle(
                string lpModuleName);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
           public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
               uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern Int32 WriteProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                string  buffer,
                uint size,
                out IntPtr lpNumberOfBytesWritten);

          [DllImport("kernel32.dll")]
public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
   IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
   lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);


         
        }
        public static bool DoInject(
            Process pToBeInjected,
            string sDllPath,
            out string sError)
        {
            IntPtr hwnd = IntPtr.Zero;
            if (!CRT(pToBeInjected, sDllPath, out sError, out hwnd)) //CreateRemoteThread 
            {
                //close the handle, since the method wasn't able to get to that 
                if (hwnd != (IntPtr)0)
                    WINAPI.CloseHandle(hwnd);
                return false;
            }
            int wee = Marshal.GetLastWin32Error();
            return true;
        }

        private static bool CRT(
            Process pToBeInjected,
            string sDllPath,
            out string sError,
            out IntPtr hwnd)
        {
            sError = String.Empty; //in case we encounter no errors 

            IntPtr hndProc = WINAPI.OpenProcess( 
                ProcessAccessFlags.CreateThread| 
                ProcessAccessFlags.VMOperation| 
                ProcessAccessFlags.VMRead| 
                ProcessAccessFlags.VMWrite|
                 ProcessAccessFlags.QueryInformation, 
                false,
                (uint)pToBeInjected.Id);

            hwnd = hndProc;

            if (hndProc == (IntPtr)0)
            {
                sError = "Unable to attatch to process.\n";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }

            IntPtr lpLLAddress = WINAPI.GetProcAddress(
                WINAPI.GetModuleHandle("kernel32.dll"),
                "LoadLibraryA");

            if (lpLLAddress == (IntPtr)0)
            {
                sError = "Unable to find address of \"LoadLibraryA\".\n";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }
          //  byte[] bytes = CalcBytes(sDllPath);
            IntPtr lpAddress = WINAPI.VirtualAllocEx(
                hndProc,
                (IntPtr)null,
                (uint)sDllPath.Length+1, //520 bytes should be enough 
                 AllocationType.Commit,
                MemoryProtection.ExecuteReadWrite);

            if (lpAddress == (IntPtr)0)
            {
                if (lpAddress == (IntPtr)0)
                {
                    sError = "Unable to allocate memory to target process.\n";
                    sError += "Error code: " + Marshal.GetLastWin32Error();
                    return false;
                }
            }

      
            IntPtr ipTmp = IntPtr.Zero;

            WINAPI.WriteProcessMemory(
                hndProc,
                lpAddress,
                sDllPath,
                (uint)sDllPath.Length + 1,
                out ipTmp);

            if (Marshal.GetLastWin32Error() != 0)
            {
                sError = "Unable to write memory to process.";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }

            IntPtr ipThread = WINAPI.CreateRemoteThread(
                hndProc,
                (IntPtr)null,
                0,
                lpLLAddress,
                lpAddress,
                0,
                (IntPtr)null);

            if (ipThread == (IntPtr)0)
            {
                sError = "Unable to load dll into memory.";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }

            return true;
        }

    }
}



