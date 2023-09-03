using System;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace UDPReceiver
{
    class Program
    {
        static string localIpString = "192.168.1.60";
        static int localPort = 5001;

        static System.Net.IPAddress localIPAddress = System.Net.IPAddress.Parse(localIpString);
        static System.Net.IPEndPoint localIPEP = new System.Net.IPEndPoint(localIPAddress, localPort);
        static System.Net.Sockets.UdpClient udp = new System.Net.Sockets.UdpClient(localIPEP);

        static int bufferIdx = 0;
        static int dataIdx = 0;
        static int fileIdx = 0;
        const int DATA_BUFFER_NUM = 2;
        const int DATA_BUFFER_SIZE = 5000;
        static CommonLib.HandInfo[,] hapticInfo = new CommonLib.HandInfo[DATA_BUFFER_NUM, DATA_BUFFER_SIZE];

        private static Mutex mut = new Mutex();

        static void saveBufferDataToFile(int idx)
        {
            fileIdx++;
            new Thread(() => {
                using (var writer = File.CreateText(fileIdx*DATA_BUFFER_SIZE + ".csv"))
                {
                    for (int i=0; i<DATA_BUFFER_SIZE; i++)
                    {
                        var hi = hapticInfo[bufferIdx, i];
                        writer.Write("{0},",hi.seq);

                        for (int j=0; j<CommonLib.HandInfo.FINGER_NUM; j++)
                        {
                            var fi = hi.fingerInfos[j];
                            writer.Write("{0},{1},{2},",
                                fi.accel.v1,
                                fi.accel.v2,
                                fi.accel.v3
                            );
                            writer.Write("{0},{1},{2},",
                                fi.force.v1,
                                fi.force.v2,
                                fi.force.v3
                            );
                            writer.WriteLine("{0},{1},{2},{3},{4},{5}",
                                fi.temp.v1,
                                fi.temp.v2,
                                fi.temp.v3,
                                fi.temp.v4,
                                fi.temp.v5,
                                fi.temp.v6
                            );
                        }
                    }
                }
                // Enable buffer swith again
                mut.ReleaseMutex();
            }).Start();
        }

        static void switchBuffer()
        {
            // Lock buffer change until filewrite complete
            mut.WaitOne();
            int oldBufferIdx = bufferIdx;
            bufferIdx = ++bufferIdx % DATA_BUFFER_NUM;
            dataIdx = 0;
            // save old buffer data to file
            saveBufferDataToFile(oldBufferIdx);
        }

        static void processFingerInfo(int idx, CommonLib.FingerInfo fi)
        {
            // insert data into fixed buffer
            var hifi = hapticInfo[bufferIdx, dataIdx].fingerInfos[idx];
            hifi.accel.v1 = fi.accel.v1;
            hifi.accel.v2 = fi.accel.v2;
            hifi.accel.v3 = fi.accel.v3;

            hifi.force.v1 = fi.force.v1;
            hifi.force.v2 = fi.force.v2;
            hifi.force.v3 = fi.force.v3;

            hifi.temp.v1 = fi.temp.v1;
            hifi.temp.v2 = fi.temp.v2;
            hifi.temp.v3 = fi.temp.v3;
            hifi.temp.v4 = fi.temp.v4;
            hifi.temp.v5 = fi.temp.v5;
            hifi.temp.v6 = fi.temp.v6;
        }

        static void processHandInfo(int seq)
        {
            // set seq number and evaluate buffer free space
            hapticInfo[bufferIdx, dataIdx].seq = seq;
            if (++dataIdx >= DATA_BUFFER_SIZE) {
                switchBuffer();
            }
        }

        static void exec()
        {
            Console.WriteLine("=== Loop started! ===");

            int READ_SIZE = Marshal.SizeOf(typeof(CommonLib.FingerInfo));
            Console.WriteLine("Size = {0}", READ_SIZE);
            for(;;)
            {
                System.Net.IPEndPoint remoteEP = null;
                byte[] rcvBytes = udp.Receive(ref remoteEP);

                using (var stream = new MemoryStream(rcvBytes))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        Console.WriteLine("Data Received!...");

                        stream.Seek(0, SeekOrigin.Begin);
                        var seq = reader.ReadInt32();

                        for (int fingerID = 0; fingerID < CommonLib.HandInfo.FINGER_NUM; fingerID++)
                        {
                            byte[] buff = reader.ReadBytes(READ_SIZE);
                            GCHandle handle = GCHandle.Alloc(buff, GCHandleType.Pinned);
                            CommonLib.FingerInfo fi = (CommonLib.FingerInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),typeof(CommonLib.FingerInfo));
                            processFingerInfo(fingerID, fi);
                            handle.Free();
                        }
                        processHandInfo(seq);
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("=== UDP Receiver App ===");
            exec();
        }
    }
}
