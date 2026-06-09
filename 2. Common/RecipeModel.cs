using Lib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OpenVisionLab._2._Common
{
    public class RecipeModel
    {
        public RecipeModel()
        {

        }

        public string Code { get; set; } = "";
        public string Name { get; set; } = "";

        public List<RecipeModel> GetProductsList()
        {
            var list = new List<RecipeModel>();

            List<string> listRecipe = new List<string>();

            DirectoryInfo di = new DirectoryInfo(AppPathService.RecipeRootDirectory);
            if (di.Exists)
            {
                DirectoryInfo[] diRecipies = di.GetDirectories();
                for (int i = 0; i < diRecipies.Length; i++)
                {
                    string strRecipe = diRecipies[i].Name;
                    listRecipe.Add(strRecipe);
                }
            }

            for (int i = 0; i < listRecipe.Count; i++)
            {
                string strName = listRecipe[i];

                                string strModelNo = strName.Substring(strName.Length - 3);
                string strRecipeName = strName.Substring(0, strName.Length - 3);

                list.Add(new RecipeModel { Code =strModelNo, Name = strRecipeName });
            
            }

            list.Sort((Code, Name) => Code.Code.CompareTo(Name.Code));

            return list;
        }
    }
}
