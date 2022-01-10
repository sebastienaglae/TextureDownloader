using System.Timers;

namespace TextureDownloader.Texture.Utils
{
    public abstract class Debug
    {
        private static bool isEnabled = true;

        public static bool IsEnabled { get => isEnabled; set => isEnabled = value; }

        public static void Jump()
        {
            if (!isEnabled) return;
            Console.WriteLine();
        }

        public static void Log(string message)
        {
            if (!isEnabled) return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            if (!isEnabled) return;
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.Beep();
        }

        public static void Warning(string message)
        {
            if (!isEnabled) return;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(message);
            Console.Beep();
        }

        public static void ReprintOnSameLine(string message)
        {
            if (!isEnabled) return;
            for (int i = message.Length; i < Console.CursorLeft; i++)
            {
                message += " ";
            }
            Console.Write("\r" + message);
        }
    }

    public class Bloc
    {
        private List<(int left, int top)> ps1;
        private List<int> maxTop = new List<int>();
        private int currentLine = 0;
        private int maxLine = 0;
        private (int left, int top) defaultCursor;
        private static (int left, int top) endCursor;
        private bool isFree = false;
        private static bool isSameLine = false;

        static Bloc()
        {
        }

        public static void SetCursorToEnd()
        {
            Console.SetCursorPosition(endCursor.left, endCursor.top);
        }

        public static void SameLine(bool sameline)
        {
            isSameLine = sameline;
        }

        public Bloc(int c)
        {
            if (isSameLine)
                c = 1;
            CreateBloc(c);
        }

        private void CreateBloc(int c)
        {
            ps1 = new List<(int, int)>();
            maxLine = c;
            for (int i = 0; i < c; i++)
            {
                ps1.Add(Console.GetCursorPosition());
                maxTop.Add(0);
                Console.WriteLine();
            }
            Console.WriteLine();
            defaultCursor = Console.GetCursorPosition();
            SetMaxCursor();
        }

        public bool IsFree()
        {
            return isFree;
        }

        private void SetMaxCursor()
        {
            if (endCursor.top < defaultCursor.top)
                endCursor = defaultCursor;
        }

        public void WriteToBloc(string message)
        {
            lock (Console.Out)
            {
                Console.SetCursorPosition(maxTop[currentLine], ps1[currentLine].top);
                Debug.ReprintOnSameLine(message);
                maxTop[currentLine] = Console.CursorLeft;
            }
        }

        public void NextLine()
        {
            if (isSameLine)
                return;
            if (currentLine + 1 < maxLine)
            {
                currentLine++;
            }
        }

        public void Free()
        {
            for (int i = 0; i < maxLine; i++)
            {
                currentLine = i;
                WriteToBloc("");
            }
            currentLine = 0;
            isFree = true;
        }

        public void UnFree()
        {
            isFree = false;
        }
    }
}
