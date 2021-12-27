using System;
using Mage;

namespace MageProject.Desktop
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            DesktopRunner.Run<MageProjectGame>();
        }
    }
}