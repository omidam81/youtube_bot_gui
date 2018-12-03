using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using youtube_bot_lib.model;

namespace youtube_bot_gui.util
{
    public class UserManager
    {
        public static void saveUser(User user)
        {
            try
            {
                
                XmlDocument document = new XmlDocument();
                document.Load("accounts.xml");

                XmlDocumentFragment xfrag = document.CreateDocumentFragment();
                xfrag.InnerXml = @"<account><username>" + user.UserName + "</username><password>" +
                                 user.Password + "</password><api-key>" + user.apiKey + "</api-key><app-name>" +
                                 user.appName + "</app-name><proxy>" + user.Proxy + "</proxy></account>";

                document.DocumentElement.AppendChild(xfrag);
                document.Save(new StreamWriter("accounts.xml"));
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not save user: " + user.UserName + Environment.NewLine + e.Message);
                
            }
        }
    }
}