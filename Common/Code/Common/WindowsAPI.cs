using System;
using System.Runtime.InteropServices;

namespace ToB.Common
{
    public static class WindowsAPI
    {
        #region deletefileorfolder2bin

        const int FO_DELETE = 3;
        const int FOF_ALLOWUNDO = 0x40;
        const int FOF_NOCONFIRMATION = 0x0010;
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        public static int deleteFileOrFolder2Bin(string ff)
        {
            if (ff.EndsWith("\\")) ff = ff.Remove(ff.Length - 1, 1);
            SHFILEOPSTRUCT shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_DELETE;
            shf.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            shf.pFrom = ff + '\0' + '\0';
            return SHFileOperation(ref shf);
        }
        public static int deleteFileOrFolder2Bin(string[] listfile)
        {
            int ret = 0;
            foreach (string ff in listfile)
            {
                ret = deleteFileOrFolder2Bin(ff);
            }
            return ret;
        }

        #endregion      

        public static string getDesktopPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\";
        }
        public static void RegisterDlls(string filePath)
        {
            #region sample
            //   regsvr32 [/u] [/s] [/n] [/i[:cmdline]] dllname
            //   /u : Unregisters server.
            //   /s : Specifies regsvr32 to run silently and to not display any message boxes.
            //   /n : Specifies not to call DllRegisterServer. You must use this option with /i.
            //   /i : cmdline : Calls DllInstall passing it an optional [cmdline]. When used with /u, it calls dll uninstall.
            //   dllname : Specifies the name of the dll file that will be registered.
            #endregion

            //'/s' : Specifies regsvr32 to run silently and to not display any message boxes.
            string arg_fileinfo = "/s" + " " + "\"" + filePath + "\"";
            System.Diagnostics.Process reg = new System.Diagnostics.Process();
            //This file registers .dll files as command components in the registry.
            reg.StartInfo.FileName = "regsvr32.exe";
            reg.StartInfo.Arguments = arg_fileinfo;
            reg.StartInfo.UseShellExecute = false;
            reg.StartInfo.CreateNoWindow = true;
            reg.StartInfo.RedirectStandardOutput = true;
            reg.Start();
            reg.WaitForExit();
            reg.Close();
        }
        public static void unRegisterDlls(string filePath)
        {
            #region sample
            //   regsvr32 [/u] [/s] [/n] [/i[:cmdline]] dllname
            //   /u : Unregisters server.
            //   /s : Specifies regsvr32 to run silently and to not display any message boxes.
            //   /n : Specifies not to call DllRegisterServer. You must use this option with /i.
            //   /i : cmdline : Calls DllInstall passing it an optional [cmdline]. When used with /u, it calls dll uninstall.
            //   dllname : Specifies the name of the dll file that will be registered.
            #endregion

            //'/s' : Specifies regsvr32 to run silently and to not display any message boxes.
            string arg_fileinfo = "/u /s" + " " + "\"" + filePath + "\"";
            System.Diagnostics.Process reg = new System.Diagnostics.Process();
            //This file registers .dll files as command components in the registry.
            reg.StartInfo.FileName = "regsvr32.exe";
            reg.StartInfo.Arguments = arg_fileinfo;
            reg.StartInfo.UseShellExecute = false;
            reg.StartInfo.CreateNoWindow = true;
            reg.StartInfo.RedirectStandardOutput = true;
            reg.Start();
            reg.WaitForExit();
            reg.Close();
        }
    }
}
