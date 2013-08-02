using System;

namespace Engine
{
	public static class GlobalProps
	{
        public static string EnginePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
		public static string BasePath = "startup";
        public static string SystemPath = "system";
        public static string GameName = "game";
        public static int Width = 320;
        public static int Height = 240;
	}
}

