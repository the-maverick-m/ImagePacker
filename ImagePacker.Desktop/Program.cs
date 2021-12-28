using System;
using Mage;
using TexturePacker;

namespace ImagePacker.Desktop
{
	public static class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			using (var game = new TexturePackerGame(args))
				game.Run();
		}
	}
}