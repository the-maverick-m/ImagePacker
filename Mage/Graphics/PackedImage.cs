using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mage
{
    public class PackedImage
    {
        // File format is as follows
        // First 2 32-bit ints specify width & height
        // Rest of data is width * height amount of colors
        // First alpha, and if not completely transparent, then red, green, blue
        public static Texture2D Decode(GraphicsDevice graphicsDevice, Stream stream)
        {
            Texture2D result;
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                result = new Texture2D(graphicsDevice, width, height);
                Color[] data = new Color[width * height];

                for (int i = 0; i < width * height; i++)
                {
                    Color currData = new Color();
                    currData.A = reader.ReadByte();

                    if (currData.A == 0)
                    {
                        currData.R = currData.G = currData.B = 0;
                    }
                    else
                    {
                        currData.R = reader.ReadByte();
                        currData.G = reader.ReadByte();
                        currData.B = reader.ReadByte();
                    }

                    data[i] = currData;
                }

                result.SetData(data);
            }

            return result;
        }

        public static Texture2D Decode(GraphicsDevice graphicsDevice, string path)
        {
            using (FileStream stream = File.OpenRead(path))
                return Decode(graphicsDevice, stream);
        }

        public static void Encode(Texture2D texture, Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(texture.Width);
                writer.Write(texture.Height);

                Color[] data = new Color[texture.Width * texture.Height];
                texture.GetData(data);

                for (int i = 0; i < texture.Width * texture.Height; i++)
                {
                    writer.Write(data[i].A);
                    if (data[i].A == 0) continue;

                    writer.Write(data[i].R);
                    writer.Write(data[i].G);
                    writer.Write(data[i].B);
                }
            }
        }
    }
}