using System.Collections.Generic;
using System;
using System.IO;

namespace Proj.Modules.Misc {
    public static class Language {
        public static string language = "en_US";
        private static Dictionary<string, string> lang_file = new Dictionary<string, string>();

        public static void load_langfile(string lang) {
            language = lang;
            //string file_path = game_manager.executable_dir + "\\src\\resources\\" + game_manager.asset_pack + "data\\lang\\" + language + ".lang";

            // Debug workaround for filepath

            string file_path = game_manager.executable_path + "\\src\\resources\\" + game_manager.asset_pack + "\\data\\lang\\" + language + ".lang";

            string[] array = File.ReadAllLines(file_path);
            foreach(string i in array) {
                lang_file.Add(i.Split("::")[0], i.Split("::")[1]);
            }
        }

        public static string Text(string id) {
            if(lang_file.ContainsKey(id)) {
                return lang_file[id];
            } else {
                return id;
            }
        }
    }
}