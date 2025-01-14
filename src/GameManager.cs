using System.Numerics;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;
using Fjord.Input;
using Fjord.Graphics;
using Fjord.Scenes;
using Fjord.Ui;

namespace Fjord;

public class Window
{
    public int Width;
    public int Height;
}
public static class Game
{
    public static IntPtr SDLWindow;
    public static IntPtr SDLRenderer;
    
    public static Window Window = new();

    internal static bool Running = true;

    private static ulong timeNow = 0;
    private static ulong timeLast = 0;
    private static double deltaTime = 0.0;

    public static void Initialize(string title, int width, int height)
    {   
        #if DEBUG
            Debug.Log("Running in debug mode");
        #else
            Debug.Log("Running in release mode");
        #endif
        
        SDL_Init(SDL_INIT_EVERYTHING);

        SDLWindow = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height,
            SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

        SDLRenderer = SDL_CreateRenderer(SDLWindow, 0, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

        Window = new()
        {
            Width = width,
            Height = height
        };

        SDL_SetRenderDrawBlendMode(SDLRenderer, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
        Font.Initialize();
        Debug.Initialize();
        SceneHandler.Initialize();
        Debug.Log("Fjord Initalized");
    }

    public static void Stop()
    {
        Debug.Log("Fjord Stopped");
        Running = false;
        SDL_DestroyRenderer(SDLRenderer);
        SDL_DestroyWindow(SDLWindow);
        Font.Destroy();

        List<string> PrintLogs = new();

        foreach(DebugLog log in Debug.Logs)
        {
            if(log.level != LogLevel.User)
                PrintLogs.Add(String.Format("[{0}] {1} {2} -> {3}", log.time, log.level.ToString(), log.sender, log.message));
            else
                PrintLogs.Add(String.Format("[{0}] {1} -> {2}", log.time, log.level.ToString(), log.message));
        }

        var Path = "./Logs/Log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        if(!Directory.Exists("./Logs"))
        {
            Directory.CreateDirectory("./Logs");
        }
        if(PrintLogs.Count > 0) 
        {
            File.WriteAllLines(Path, PrintLogs);
        }
    }
    
    public static void Run()
    {
        bool open = false;
        while (Running)
        {
            timeNow = SDL_GetPerformanceCounter();
            deltaTime = (double)Math.Clamp(((timeNow - timeLast)*1000 / (double)SDL_GetPerformanceFrequency() )*0.001, 0, 1);
            timeLast = timeNow;
            
            EventHandler.HandleEvents();

            Update();
            Render(ref open);

            for(var i = 0; i < GlobalKeyboard.downKeys.Length; i++)
            {
                GlobalKeyboard.pressedKeys[i] = false;
            }

            foreach (var key in GlobalMouse.pressedKeys.Keys.ToList())
            {
                GlobalMouse.pressedKeys[key] = false;
            }
            GlobalMouse.downKeys[MB.ScrollDown] = false;
            GlobalMouse.downKeys[MB.ScrollLeft] = false;
            GlobalMouse.downKeys[MB.ScrollRight] = false;
            GlobalMouse.downKeys[MB.ScrollUp] = false;

            if(!SceneHandler.LoadedScenes.Any((s) => SceneHandler.Scenes[s].MouseInsideScene == true))
            {
                SDL_ShowCursor(SDL_ENABLE);
            }
        }
    }

    public static double GetDeltaTime()
    {
        return deltaTime;
    }

    public static void Update()
    {
        SDL_GetWindowSize(SDLWindow, out Window.Width, out Window.Height);

        foreach (string id in SceneHandler.GetLoadedScenes())
        {
            try {
                SceneHandler.Scenes[id].UpdateCall();
            } catch(Exception e) {
                SceneHandler.Unload(id);
                Debug.Log(LogLevel.Error, $"Scene '{id}' update crashed!");
                Debug.Log(LogLevel.Error, e.ToString());
            }
        }
        
        if (GlobalKeyboard.Pressed(Key.D, Mod.LShift, Mod.LCtrl))
        {
            if (!SceneHandler.IsLoaded("inspector"))
                SceneHandler.Load("inspector");
            else
                SceneHandler.Unload("inspector");
        }

        if (GlobalKeyboard.Pressed(Key.C, Mod.LShift, Mod.LCtrl))
        {
            if (!SceneHandler.IsLoaded("console")) {
                SceneHandler.Load("console");
            } else
                SceneHandler.Unload("console");
        }
    }

    public static void Render(ref bool open)
    {
        SDL_SetRenderDrawColor(SDLRenderer, 0, 0, 0, 255);
        SDL_RenderClear(SDLRenderer);

        foreach (string id in SceneHandler.GetLoadedScenes())
        {
            try {
                SceneHandler.Scenes[id].RenderCall();
            } catch(Exception e) {
                SceneHandler.Unload(id);
                Debug.Log(LogLevel.Error, $"Scene '{id}' render crashed!");
                Debug.Log(LogLevel.Error, e.ToString());
            }
        }
        
        Draw.DrawDrawBuffer(Draw.drawBuffer, null);
        Draw.drawBuffer = new();

        SDL_RenderPresent(SDLRenderer);
    }
}