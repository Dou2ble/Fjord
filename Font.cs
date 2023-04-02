using System.Numerics;
using SDL2;
using static SDL2.SDL_ttf;
using static SDL2.SDL;

namespace ShooterThingy;

public static class Font
{
    internal static Dictionary<string, IntPtr> Fonts = new ();
    internal static Dictionary<string, IntPtr> FontCache = new();

    internal static string DefaultFont = "helveticaneue.ttf";

    public static string GetDefaultFont()
    {
        return DefaultFont;
    }

    public static void Initialize()
    {
        TTF_Init();
    }

    public static void Destroy()
    {
        foreach (string id in FontCache.Keys)
        {
            SDL_DestroyTexture(FontCache[id]);
        }
        TTF_Quit();
    }

    public static void Draw(Vector2 position, string font, string text, int size, SDL_Color color)
    {
        if(!Fonts.ContainsKey(font + size.ToString()))
        {
            Fonts.Add(font + size.ToString(), TTF_OpenFont(font, size));
        }

        string CacheKey = font + text + size + color.r + color.g + color.b + color.a;
        if (!FontCache.ContainsKey(CacheKey))
        {
            IntPtr surface = TTF_RenderText_Blended(Fonts[font + size.ToString()], text, color);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Game.SDLRenderer, surface);
            FontCache.Add(CacheKey, texture);
            SDL.SDL_FreeSurface(surface);
        }

        SDL_QueryTexture(FontCache[CacheKey], out uint format, out int access, out int textureW, out int textureH);
        
        SDL_Rect rect = new()
        {
            x = (int)position.X,
            y = (int)position.Y,
            w = (int)(textureW),
            h = (int)(textureH)
            
        };
        
        SDL_RenderCopy(Game.SDLRenderer, FontCache[CacheKey], IntPtr.Zero, ref rect);
    }
    
    public static Vector2 DrawSize(string font, string text, int size, SDL_Color color)
    {
        if (!Fonts.ContainsKey(font + size.ToString()))
        {
            Fonts.Add(font + size.ToString(), TTF_OpenFont(font, size));
        }

        string CacheKey = font + text + size + color.r + color.g + color.b + color.a;
        if (!FontCache.ContainsKey(CacheKey))
        {
            IntPtr surface = TTF_RenderText_Blended(Fonts[font + size.ToString()], text, color);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(Game.SDLRenderer, surface);
            FontCache.Add(CacheKey, texture);
            SDL.SDL_FreeSurface(surface);
        }

        SDL_QueryTexture(FontCache[CacheKey], out uint format, out int access, out int textureW, out int textureH);

        return new()
        {
            X = textureW,
            Y = textureH
        };
    }
}