using static SDL2.SDL_ttf;
using static SDL2.SDL;
using System;
using System.Collections.Generic;
using Fjord.Modules.Misc;

namespace Fjord.Modules.Graphics
{
    public static class font_handler
    {
        private static Dictionary<string, dynamic> fonts = new Dictionary<string, dynamic>();
        private static Dictionary<string, int> font_sizes = new Dictionary<string, int>();

        private static Dictionary<string, IntPtr> texts = new Dictionary<string, IntPtr>();

        public static void init() {
            string ass = game_manager.asset_pack;
            game_manager.set_asset_pack("general");
            font_handler.load_font("default", "FiraCode", 255);
            font_handler.load_font("default-bold", "FiraCode-Bold", 255);
            game_manager.set_asset_pack(ass);
        }

        public static int get_font_size(string id) {
            return font_sizes[id];
        }

        public static bool load_font(string id, string font, int font_size) {
            if(!fonts.ContainsKey(id)) {
                fonts.Add(id, TTF_OpenFont(game_manager.executable_path + "\\resources\\" + game_manager.asset_pack + "\\assets\\fonts\\" + font + ".ttf", font_size));
                font_sizes.Add(id, font_size);
                return true;
            }
            return false;
        }

        public static void get_texture(IntPtr renderer, string text, string font_id, out IntPtr texture, int x = 0, int y = 0, byte r = 255, byte g = 255, byte b = 255, byte a = 255) {
            var key_ = hash.HashString(text + font_id);
            if(!texts.ContainsKey(key_)) {
                IntPtr surface;
                SDL_Color textColor;
                dynamic font = fonts[font_id];
                textColor.r = r;
                textColor.g = g;
                textColor.b = b;
                textColor.a = a;    

                surface = TTF_RenderText_Solid(font, text, textColor);
                var texture_ = SDL_CreateTextureFromSurface(renderer, surface);
                SDL_FreeSurface(surface);

                texts.Add(key_, texture_);
                texture = texture_;
            } else {
                texture = texts[key_];
            }
        }
    }
}