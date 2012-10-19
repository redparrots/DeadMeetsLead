using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Common
{
    public static class Utils
    {
        public static byte[] StructToBytes<T>(T strct) where T : struct
        {
            int rawsize = Marshal.SizeOf(strct);
            byte[] rawdatas = new byte[rawsize];
            GCHandle handle =
                GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(strct, buffer, false);
            handle.Free();
            return rawdatas;
        }

        public static object BytesToStruct(byte[] rawdatas, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length)
                return null;
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            handle.Free();
            return retobj;

        }

        public static byte[] StructArrayToBytes<T>(T[] data) where T : struct
        {
            MemoryStream m = new MemoryStream();
            BinaryWriter b = new BinaryWriter(m);
            foreach (var v in data)
                b.Write(StructToBytes(v));
            return m.ToArray();
        }

        public static byte[] IntArrayToBytes(int[] data)
        {
            MemoryStream m = new MemoryStream();
            BinaryWriter b = new BinaryWriter(m);
            foreach (var v in data)
                b.Write(BitConverter.GetBytes(v));
            return m.ToArray();
        }

        public static int[] ByteArrayToInts(byte[] data)
        {
            int[] array = new int[data.Length / 4];
            for (int i = 0; i < data.Length; i+=4 )
                array[i / 4] = BitConverter.ToInt32(data, i);
            return array;
        }

        public static List<T> BytesToStructArray<T>(byte[] data) where T : struct
        {
            MemoryStream m = new MemoryStream(data);
            BinaryReader b = new BinaryReader(m);
            Type t = typeof(T);
            int rawsize = Marshal.SizeOf(t);
            byte[] v;
            List<T> r = new List<T>();
            while ((v = b.ReadBytes(rawsize)) != null)
                r.Add((T)BytesToStruct(v, t));
            return r;
        }

        public static T[] Flatten<T>(T[,] values)
        {
            int w = values.GetLength(1);
            int h = values.GetLength(0);
            T[] newVals = new T[values.GetLength(0) * values.GetLength(1)];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    newVals[x + y * w] = values[y, x];
            return newVals;
        }

        public static byte[] BitFlatten<T>(T[,] values, int TSize, int pitch) where T : struct
        {
            int w = values.GetLength(1);
            int h = values.GetLength(0);
            MemoryStream m = new MemoryStream();
            BinaryWriter b = new BinaryWriter(m);
            for (int y = 0; y < h; y++)
            {
                int x = 0;
                for (; x < w; x++)
                    b.Write(StructToBytes(values[y, x]), 0, TSize);
                if (x * TSize < pitch)
                    b.Seek(pitch - x * TSize, SeekOrigin.Current);
            }
            return m.ToArray();
        }

        public static T[] GetRow<T>(T[,] data, int row)
        {
            T[] rowData = new T[data.GetLength(1)];
            for (int x = 0; x < data.GetLength(1); x++)
                rowData[x] = data[row, x];
            return rowData;
        }

        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int GetArrayHashCode<T>(T[,] array)
        {
            int w = array.GetLength(1), h = array.GetLength(0);
            return GetArrayHashCode(array, new System.Drawing.Rectangle(0, 0, w, h));
        }

        /// <param name="selection">In the range [0, 1]</param>
        public static int GetArrayHashCode<T>(T[,] array, System.Drawing.RectangleF selection)
        {
            int w = array.GetLength(1), h = array.GetLength(0);
            return GetArrayHashCode(array, new System.Drawing.Rectangle(
                (int)(selection.X * w), (int)(selection.Y * h),
                (int)(selection.Width * w), (int)(selection.Height * h)));
        }

        /// <param name="selection">In the range [0, width/height of array]</param>
        public static int GetArrayHashCode<T>(T[,] array, System.Drawing.Rectangle selection)
        {
            int hca = 1, hcb = 0;
            int w = array.GetLength(1);
            for (int y = selection.Top; y < selection.Bottom; y++)
                for (int x = selection.Left; x < selection.Right; x++)
                {
                    var hc = array[y, x].GetHashCode();
                    hca ^= y ^ x ^ hc;
                    hcb += (y * w + x) * hc;
                }
            return hca + hcb;
        }
        public static T[,] Select<T>(T[,] data, System.Drawing.RectangleF selection)
        {
            int w = data.GetLength(1), h = data.GetLength(0);
            return Select(data, new System.Drawing.Rectangle((int)(selection.X * w), (int)(selection.Y * h),
                (int)(selection.Width * w), (int)(selection.Height * h)));
        }

        public static T[,] Select<T>(T[,] data, System.Drawing.Rectangle selection)
        {
            T[,] vals = new T[selection.Height, selection.Width];
            for (int y = selection.Top; y < selection.Bottom; y++)
                for (int x = selection.Left; x < selection.Right; x++)
                {
                    vals[y - selection.Top, x - selection.Left] = data[y, x];
                }
            return vals;
        }

        public static string MatrixToString(SlimDX.Matrix matrix)
        {
            return 
                matrix.M11.ToString("00.0") + ", " + matrix.M12.ToString("00.0") + ", " + matrix.M13.ToString("00.0") + ", " + matrix.M14.ToString("00.0") + "\n" +
                matrix.M21.ToString("00.0") + ", " + matrix.M22.ToString("00.0") + ", " + matrix.M23.ToString("00.0") + ", " + matrix.M24.ToString("00.0") + "\n" +
                matrix.M31.ToString("00.0") + ", " + matrix.M32.ToString("00.0") + ", " + matrix.M33.ToString("00.0") + ", " + matrix.M34.ToString("00.0") + "\n" +
                matrix.M41.ToString("00.0") + ", " + matrix.M42.ToString("00.0") + ", " + matrix.M43.ToString("00.0") + ", " + matrix.M44.ToString("00.0") + "\n";
        }

        public static bool SequenceEquals<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a == null && b == null) return true;
            else if (a == null && b != null) return false;
            else if (a != null && b == null) return false;
            else return a.SequenceEqual(b);
        }

        public static string UniqueFilename(string name, string extension)
        {
            string ret = name + extension;
            if (!File.Exists(ret))
                return ret;

            int count = 1;
            do
            {
                ret = name + "(" + count++ + ")" + extension;
            } while (File.Exists(ret));

            return ret;
        }

        public static string GetMac()
        {
            var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            System.Net.NetworkInformation.PhysicalAddress backupAddress = null;
            foreach (var adapter in nics)
            {
                var nit = adapter.NetworkInterfaceType;
                if (nit == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet ||
                    nit == System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211)
                    return adapter.GetPhysicalAddress().ToString();
                else if (nit != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                    nit != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                    backupAddress = adapter.GetPhysicalAddress();
            }
            if (backupAddress != null)
            {
                return backupAddress.ToString();
            }
            return "Not found";
        }
    }
}
