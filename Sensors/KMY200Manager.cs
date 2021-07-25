using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Sensors
{
    public class KMY200Manager
    {
        [DllImport("KMY200.dll", EntryPoint = "G_Initialize")]
        public static extern int Initialize(int nPort);

        [DllImport("KMY200.dll", EntryPoint = "G_Close")]
        public static extern int Close();

        [DllImport("KMY200.dll", EntryPoint = "G_StateCheck")]
        public static extern byte StateCheck();

        [DllImport("KMY200.dll", EntryPoint = "G_SetLamp")]
        public static extern int SetLamp(uint iBoard, uint iLamp, uint iState);

        [DllImport("KMY200.dll", EntryPoint = "G_SetDoor")]
        public static extern int SetDoor(uint iDoor, uint iState);
    }
}
