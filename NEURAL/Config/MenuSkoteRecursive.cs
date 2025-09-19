using NEURAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace NEURAL.Config
{
    public class MenuSkoteRecursive
    {
        List<MENU_T> allMenuItems;

        public const string OPEN_PARENT_LIST_TAG = "<ul class=\"metismenu list-unstyled\" id=\"side-menu\">";
        public const string CLOSE_PARENT_LIST_TAG = "</ul>";

        public const string MENU_TITLE = "<li class=\"menu-title\">{0}</li>";
        
        public const string OPEN_MENU_HEADER = "<li class=\"left-menu-custom\"><a href=\"javascript: void(0);\" class=\"has-arrow waves-effect\"><i class=\"{1}\"></i><span>{0}</span></a><ul class=\"sub-menu\" aria-expanded=\"false\">";
        public const string MENU_SUB = "<li><a href=\"{0}\" target=\"{1}\">{2}</a></li>";
        public const string MENU_SUB2 = "<li class=\"left-menu-custom\"><a href=\"{0}\" target=\"{1}\"><i class=\"{3}\"></i><span>{2}</span></a></li>";
        public const string CLOSE_MENU_HEADER = "</ul></li>";

        public MenuSkoteRecursive()
        {
            //allMenuItems = GetMenuItems();
        }

        public void InitMenu(List<MENU_T> Division, List<MENU_T> Menu)
        {
            allMenuItems = Menu;
        }

        public string GenerateMenuUi()
        {
            var strBuilder = new StringBuilder();
            List<MENU_T> parentItems = (from a in allMenuItems where a.PARENT_ID == "0" orderby a.MENU descending select a).ToList();

            strBuilder.Append(OPEN_PARENT_LIST_TAG);
            strBuilder.Append(String.Format(MENU_TITLE, "Menu"));

            foreach (var parentcat in parentItems)
            {
                string icon = "bx bx-list-ul";
                if (parentcat.MENU.Equals("Home"))
                {
                    icon = "bx bx-home-circle";
                }
                else if (parentcat.MENU.Equals("Aplikasi"))
                {
                    icon = "bx bx-briefcase-alt-2";
                }
                else if (parentcat.MENU.Equals("Administrator"))
                {
                    icon = "bx bx-receipt";
                }

                List<MENU_T> childItems = (from a in allMenuItems where a.PARENT_ID == parentcat.MENU_ID orderby a.MENU_ID ascending select a).ToList();
                if (childItems.Count > 0)
                {
                    strBuilder.Append(String.Format(OPEN_MENU_HEADER, parentcat.MENU, icon));
                    //List<MENU_T> childItems = (from a in allMenuItems where a.PARENT_ID == parentcat.MENU_ID orderby a.MENU_ID ascending select a).ToList();
                    //if (childItems.Count > 0)
                    AddChildItem(parentcat, strBuilder);
                    strBuilder.Append(CLOSE_MENU_HEADER);
                }
                else
                {
                    strBuilder.Append(String.Format(MENU_SUB2, "/"+parentcat.URL, "", parentcat.MENU, icon));
                }
                
            }

            strBuilder.Append(CLOSE_PARENT_LIST_TAG);
            return strBuilder.ToString();
        }

        private void AddChildItem(MENU_T childItem, StringBuilder strBuilder)
        {
            string y = "";
            List<MENU_T> childItems = (from a in allMenuItems where a.PARENT_ID == childItem.MENU_ID orderby a.MENU ascending select a).ToList();
            foreach (MENU_T cItem in childItems)
            {
                //string x = cItem.URL.Substring(0, 4);
                
                //y = y + ", " + x;
                
                string target = "";
                
                //if (x.Equals("http"))
                //{
                //    target = "_blank";
                //}
                //else
                //{
                    cItem.URL = "/" + cItem.URL;
                //}
                strBuilder.Append(String.Format(MENU_SUB, cItem.URL, target, cItem.MENU));
            }
        }
    }
}