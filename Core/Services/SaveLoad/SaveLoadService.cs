using System.Text;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Linq;

public class SaveLoadService : ISaveLoadService
{
    public PersistentData Load()
    {
        PersistentData persistentData = new();
        string data = string.Empty;
        int[] result = [];


        if (File.Exists("data.dat"))
        {
            using (var stream = File.Open("data.dat", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
                {
                    var length = reader.ReadInt32();
                    result = new int[length];
                    for (int i = 0; i < length; i++)
                        result[i] = reader.ReadInt32();
                }
            }
            byte[] bytes = result.Select(x => Convert.ToByte(~x)).ToArray();
            string str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            persistentData = JsonConvert.DeserializeObject<PersistentData>(str);
            return persistentData;
        }
        
        return null;
    }


    public void Save(PersistentData data)
    {
        using (FileStream fs = File.Create("data.dat"))
        {
            string str = JsonConvert.SerializeObject(data);

            int[] bytes = Encoding.ASCII.GetBytes(str).Select(x => Convert.ToInt32(~x)).ToArray();
            using (var writer = new BinaryWriter(fs, Encoding.ASCII, false))
            {
                writer.Write(bytes.Length);
                foreach (var i in bytes)
                    writer.Write(i);
            }
        }
    }
}