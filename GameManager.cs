using System.Numerics;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;
using Fjord.Input;
using Fjord.Graphics;
using Fjord.Scenes;

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

    public static void Initialize(string title, int width, int height)
    {   
        #if DEBUG
            Console.WriteLine("Running in debug mode");
        #else
            Console.WriteLine("Running in release mode");
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
    }

    public static void Stop()
    {
        Running = false;
        SDL_DestroyRenderer(SDLRenderer);
        SDL_DestroyWindow(SDLWindow);
        Font.Destroy();
    }
    
    public static void Run()
    {
        bool open = false;
        while (Running)
        {
            EventHandler.HandleEvents();
            
            Update();

            Render(ref open);
        }
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
        
        if (Keyboard.PressedExt(Key.D).With(Mod.LShift, Mod.LCtrl))
        {
            if (!SceneHandler.IsLoaded("inspector"))
                SceneHandler.Load("inspector");
            else
                SceneHandler.Unload("inspector");
        }

        if (Keyboard.PressedExt(Key.C).With(Mod.LShift, Mod.LCtrl))
        {
            if (!SceneHandler.IsLoaded("console"))
                SceneHandler.Load("console");
            else
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
        
        Draw.DrawDrawBuffer(Draw.drawBuffer);
        
        Keyboard.pressedKeys = new();
        Mouse.Pressed = false;
     
        SDL_RenderPresent(SDLRenderer);
    }
}