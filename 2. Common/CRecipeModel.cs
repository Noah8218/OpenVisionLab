using Lib.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OpenVisionLab._2._Common
{
    public class CRecipeModel
    {
        public CRecipeModel()
        {

        }

        public string Code { get; set; } = "";
        public string Name { get; set; } = "";

        public List<CRecipeModel> GetProductsList()
        {
            var list = new List<CRecipeModel>();

            List<string> listRecipe = new List<string>();

            DirectoryInfo di = new DirectoryInfo(Application.StartupPath + "\\RECIPE");
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

                try
                {
                    string strModelNo = strName.Substring(strName.Length - 3);
                    string strRecipeName = strName.Substring(0, strName.Length - 3);

                    list.Add(new CRecipeModel { Code =strModelNo, Name = strRecipeName });
                }
                catch (Exception Desc)
                {
                    CLOG.ABNORMAL( $"[FAILED] {MethodBase.GetCurrentMethod().ReflectedType.Name}==>{MethodBase.GetCurrentMethod().Name}   Execption ==> {Desc.Message}");
                }
            }

            list.Sort((Code, Name) => Code.Code.CompareTo(Name.Code));

            return list;
        }
    }
}
