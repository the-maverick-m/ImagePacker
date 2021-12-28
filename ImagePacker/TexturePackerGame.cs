using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mage;

namespace TexturePacker
{
    public class TexturePackerGame : Game
    {
        private int _dimensions = 4096;

        private GraphicsDeviceManager _graphics;
        private string[] _args;
        private bool _shouldExit;

        private Rectangle[] _takenSpaces;
        private TextureItem[] _items;
        private Texture2D _mainTexture;

        private bool _imgFomat;

        public TexturePackerGame(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHelpMessage();
                _shouldExit = true;
            }
            // At least one input and output file
            else if (args.Length < 2)
            {
                Console.Write("Must specify at least one input and output file.\n\n");
                WriteHelpMessage();
                _shouldExit = true;
            }
            else
            {
                if (args[0] == "-i")
                {
                    string[] tempArgs = new string[args.Length - 1];
                    for (int i = 0; i < tempArgs.Length; i++)
                    {
                        tempArgs[i] = args[i + 1];
                    }

                    this._args = tempArgs;
                    _imgFomat = true;
                }
                else
                    this._args = args;
            }

            if (!_shouldExit)
            {
                _takenSpaces = new Rectangle[_args.Length - 1];
                _items = new TextureItem[_args.Length - 1];
            }

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void WriteHelpMessage()
        {
            Console.Write(
                "Specify all input files as first arguments. Your last argument should be your output file.\n");
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (!_shouldExit)
            {
                Point finalDimensions = Point.Zero;
                int totalItemCount = _args.Length - 1;

                for (int i = 0; i < _args.Length - 1; i++)
                {
                    _items[i] = new TextureItem {Name = _args[i]};
                }

                // Check there are no conflicting names
                Console.WriteLine("\nChecking for name conflicts...");
                foreach (var textureItem in _items)
                {
                    foreach (var otherTextureItem in _items)
                    {
                        if (Path.GetFileNameWithoutExtension(textureItem.Name) ==
                            Path.GetFileNameWithoutExtension(otherTextureItem.Name))
                        {
                            if (!ReferenceEquals(textureItem, otherTextureItem))
                            {
                                Console.WriteLine("\nConflicting names found! Names: " + textureItem.Name
                                    + " and " + otherTextureItem.Name);
                                return;
                            }
                        }
                    }
                }

                Console.WriteLine("No name conflicts found!\n");

                // Load textures after checking for name conflicts
                for (int i = 0; i < _args.Length - 1; i++)
                {
                    Console.WriteLine("Loading " + _items[i].Name + "...");
                    _items[i].Texture = Texture2D.FromFile(GraphicsDevice, _args[i]);
                }

                Array.Sort(_items);
                Console.WriteLine("All textures loaded!\n");

                // Initialize dimensions to largest item's width or height, whichever is bigger
                // {
                //     int initDimensions = _dimensions;
                //     _dimensions = Math.Max(_dimensions, _items[0].Texture.Width);
                //     _dimensions = Math.Max(_dimensions, _items[0].Texture.Height);

                //     // Resize to 1.5x if dimensions changed
                //     if (_dimensions != initDimensions)
                //         _dimensions = _dimensions + (_dimensions / 2);
                // }

                _mainTexture = new Texture2D(GraphicsDevice, _dimensions, _dimensions);
                for (int i = 0; i < _items.Length; i++)
                {
                    // Get only part of texture filled with pixels
                    Texture2D finalTexture = GetCroppedTexture(_items[i].Texture);
                    _items[i].Texture.Dispose();

                    Rectangle finalRect = GetSpace(finalTexture.Bounds, i);

                    Color[] data = new Color[finalTexture.Width * finalTexture.Height];
                    finalTexture.GetData(data);

                    //Convert back to actual texture Size
                    _mainTexture.SetData(0, finalRect, data, 0, data.Length);

                    finalTexture.Dispose();
                    Console.WriteLine(_items[i].Name + " written to image...");

                    // Add 1 pixel gap for next check
                    _takenSpaces[i] =
                        new Rectangle(finalRect.X, finalRect.Y, finalRect.Width + 1, finalRect.Height + 1);
                    _items[i].Rectangle = finalRect;
                }

                Console.WriteLine("All textures written!\n");

                // Make output texture smaller to only fit input textures

                Console.WriteLine("Cropping image...");
                for (int i = 0; i < _items.Length; i++)
                {
                    finalDimensions.X = Math.Max(finalDimensions.X, _items[i].Rectangle.Right);
                    finalDimensions.Y = Math.Max(finalDimensions.Y, _items[i].Rectangle.Bottom);
                }

                using (Texture2D outputTexture = new Texture2D(GraphicsDevice, finalDimensions.X, finalDimensions.Y))
                {
                    Color[] outputData = new Color[finalDimensions.X * finalDimensions.Y];
                    _mainTexture.GetData(0, new Rectangle(0, 0, finalDimensions.X, finalDimensions.Y), outputData, 0,
                        outputData.Length);
                    outputTexture.SetData(outputData);
                    if (_imgFomat)
                    {
                        Console.WriteLine("Saving image to " + _args[_args.Length - 1] + ".img...");
                        using (FileStream outputStream = File.OpenWrite(_args[_args.Length - 1] + ".img"))
                        {
                            PackedImage.Encode(outputTexture, outputStream);
                        }
                    }
                    else
                    {
                        using (FileStream outputStream = File.OpenWrite(_args[_args.Length - 1] + ".png"))
                        {
                            Console.WriteLine("Saving image to " + _args[_args.Length - 1] + ".png...");
                            outputTexture.SaveAsPng(outputStream, finalDimensions.X, finalDimensions.Y);
                        }
                    }
                }

                _mainTexture.Dispose();
                Console.WriteLine("Image Saved!\n");

                Console.WriteLine("Writing MetaData to " + _args[_args.Length - 1] + ".bin...");
                using (FileStream metaStream = File.OpenWrite(_args[_args.Length - 1] + ".bin"))
                using (BinaryWriter writer = new BinaryWriter(metaStream))
                {
                    writer.Write(totalItemCount);
                    for (int i = 0; i < totalItemCount; i++)
                    {
                        Console.WriteLine("Writing MetaData for " + _items[i].Name + "...");
                        writer.Write(Path.GetFileNameWithoutExtension(_items[i].Name));
                        writer.Write(_items[i].Rectangle.X);
                        writer.Write(_items[i].Rectangle.Y);
                        writer.Write(_items[i].Rectangle.Width);
                        writer.Write(_items[i].Rectangle.Height);
                    }
                }

                Console.WriteLine("\nFinished! :)\n");
            }
        }

        private Texture2D GetCroppedTexture(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData(data);

            Color[,] data2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    data2D[x, y] = data[y * texture.Width + x];
                }
            }

            int left = 0, right = texture.Width, top = 0, bottom = texture.Height;
            bool assignedLeft = false, assignedRight = false, assignedTop = false, assignedBottom = false;

            for (int x = 0; x < texture.Width && !assignedLeft; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (data2D[x, y].A > 0)
                    {
                        left = x;
                        assignedLeft = true;
                        break;
                    }
                }
            }

            for (int x = texture.Width - 1; x >= 0 && !assignedRight; x--)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    if (data2D[x, y].A > 0)
                    {
                        right = x;
                        assignedRight = true;
                        break;
                    }
                }
            }

            for (int y = 0; y < texture.Width && !assignedTop; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    if (data2D[x, y].A > 0)
                    {
                        top = y;
                        assignedTop = true;
                        break;
                    }
                }
            }

            for (int y = texture.Height - 1; y >= 0 && !assignedBottom; y--)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    if (data2D[x, y].A > 0)
                    {
                        bottom = y;
                        assignedBottom = true;
                        break;
                    }
                }
            }

            // +1 needed to avoid out of bounds check
            int width = right - left + 1;
            int height = bottom - top + 1;

            Rectangle sampleRect = new Rectangle(left, top, width, height);
            Color[] croppedData = new Color[(width) * (height)];
            texture.GetData(0, sampleRect, croppedData, 0, croppedData.Length);

            Texture2D result = new Texture2D(GraphicsDevice, sampleRect.Width, sampleRect.Height);
            result.SetData(croppedData);
            return result;
        }

        private Rectangle GetSpace(Rectangle textureBounds, int takenSpacesCount)
        {
            Rectangle finalRect = FindFreeSpace(textureBounds,
                takenSpacesCount);

            // Couldn't find match, need to resize texture
            if (finalRect != Rectangle.Empty)
                return finalRect;
            else
            {
                if (_dimensions * 2 >= 16384)
                {
                    Console.WriteLine("\nCurrent final image size at or above 16,384.");
                    Console.WriteLine("Sizes at or above this may result in crashes!\n");
                }

                Console.WriteLine("Resizing final image...");
                Color[] tempData = new Color[_mainTexture.Width * _mainTexture.Height];
                _mainTexture.GetData(tempData);
                _mainTexture.Dispose();

                // Resize to 1.5 times
                _mainTexture = new Texture2D(GraphicsDevice, _dimensions * 2, _dimensions * 2);
                _mainTexture.SetData(0, new Rectangle(0, 0, _dimensions,
                    _dimensions), tempData, 0, tempData.Length);

                _dimensions *= 2;

                return GetSpace(textureBounds, takenSpacesCount);
            }
        }

        private Rectangle FindFreeSpace(Rectangle rectangle, int takenSpacesCount)
        {
            Rectangle result = Rectangle.Empty;
            bool foundSpace = false;

            //Below code starts from (0, 1), so we test (0, 0) first
            {
                Rectangle testRect = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + 1, rectangle.Height + 1);

                bool foundIntersection = false;
                for (int i = 0; i < takenSpacesCount; i++)
                {
                    if (testRect.Intersects(_takenSpaces[i]))
                    {
                        foundIntersection = true;
                        break;
                    }
                }

                if (!foundIntersection)
                {
                    if (new Rectangle(0, 0, _dimensions, _dimensions).Contains(testRect))
                    {
                        result = new Rectangle(testRect.X, testRect.Y, rectangle.Width, rectangle.Height);
                        foundSpace = true;
                    }
                }
            }

            // We want to be as close to the top-left corner as possible
            for (int length = 0; length < _dimensions && !foundSpace; length++)
            {
                for (int x = 0; x < length; x++)
                {
                    // Width and Height increased by 1 for 1 pixel gap to prevent sampling other textures
                    Rectangle testRect = new Rectangle(x, length, rectangle.Width + 1, rectangle.Height + 1);
                    bool foundIntersection = false;

                    for (int i = 0; i < takenSpacesCount; i++)
                    {
                        if (testRect.Intersects(_takenSpaces[i]))
                        {
                            foundIntersection = true;
                            break;
                        }
                    }

                    if (!foundIntersection)
                    {
                        // Check it fits within main texture
                        if (new Rectangle(0, 0, _dimensions, _dimensions).Contains(testRect))
                        {
                            result = new Rectangle(testRect.X, testRect.Y, rectangle.Width, rectangle.Height);
                            foundSpace = true;
                            break;
                        }
                    }
                }

                // Did we already find an intersection when testing x-values?
                for (int y = 0; y < length && !foundSpace; y++)
                {
                    Rectangle testRect = new Rectangle(length, y, rectangle.Width + 1, rectangle.Height + 1);
                    bool foundIntersection = false;

                    for (int i = 0; i < takenSpacesCount; i++)
                    {
                        if (testRect.Intersects(_takenSpaces[i]))
                        {
                            foundIntersection = true;
                            break;
                        }
                    }

                    if (!foundIntersection)
                    {
                        // Check it fits within main texture
                        if (new Rectangle(0, 0, _dimensions, _dimensions).Contains(testRect))
                        {
                            result = new Rectangle(testRect.X, testRect.Y, rectangle.Width, rectangle.Height);
                            foundSpace = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        protected override void Update(GameTime gameTime)
        {
            Exit();

            base.Update(gameTime);
        }
    }
}
  