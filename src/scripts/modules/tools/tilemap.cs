using Proj.Modules.Graphics;
using System.Numerics;

namespace Proj.Modules.Tools {
    public class tilemap {

        public string asset_pack;

        public int[,] map;
        public int grid_w, grid_h;
        public int w, h;

        public List<string> textures = new List<string>();
        List<IntPtr> textures_intptr = new List<IntPtr>();

        public tilemap(int w, int h, int grid_w, int grid_h) {
            map = new int[w,h];
            this.grid_w = grid_w;
            this.grid_h = grid_h;
            this.w = w;
            this.h = h;
        }

        public void load_textures() {
            string ass = game_manager.asset_pack;
            game_manager.set_asset_pack(asset_pack);

            foreach(string texture in textures) {
                textures_intptr.Add(texture_handler.load_texture(texture, game_manager.renderer));
            }

            game_manager.set_asset_pack(ass);
        }

        public void draw_tilemap(int x, int y) {
            for(var i = 0; i < w; i++) {
                for(var j = 0; j < h; j++) {
                    if(map[i, j] != -1 && map[i, j] < textures_intptr.Count)
                        draw.texture(game_manager.renderer, textures_intptr[map[i, j]], x + i * grid_w, y + j * grid_h, 0, new Vector2(0, 0), true);
                }
            }
        }
    }
}