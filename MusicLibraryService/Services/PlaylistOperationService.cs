using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataHelper.DBWork;
using DataHelper.Models;
using MusicLibraryService.Data;

namespace MusicLibraryService.Services
{
    public interface IPlaylistOperationService
    {
        void ReloadLists();
        void ReloadPLRadio();
    }

    public class PlaylistOperationService :BaseService,IPlaylistOperationService
    {

       public PlaylistOperationService(IDataProvider db):base(db)
        {
            
        }

        public void ReloadLists()
        {
            ReloadPLRadio();
        }

        public void ReloadPLRadio()
        {

            var pls=Db.PlaylistServiceRepository.Where(c => c.Name == "PLRadio");

            //var pls= _db.PlaylistServiceRepository.Where(c => c.Name == "PLRadio");

            if (pls.Any())
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(pls.FirstOrDefault().Url.ToString());
            
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;


            WebResponse response = request.GetResponse();

            Stream httpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(httpStream, Encoding.UTF8);
            string result = reader.ReadToEnd();

            result = result.Substring(result.IndexOf("<aside"));

                string leftPanelStr = result.Substring(0, result.IndexOf("</aside>") + 8);
                result = result.Substring(result.IndexOf("</aside>") + 8);
                result = result.Substring(result.IndexOf("<aside"));
                string rightPanelStr = result.Substring(0, result.IndexOf("</aside>") + 8);

                XmlDocument LeftPanel = new XmlDocument();
                LeftPanel.LoadXml(leftPanelStr);

                XmlDocument RightPanel = new XmlDocument();
                RightPanel.LoadXml(rightPanelStr);

                var panels = new List<XmlDocument>();
                panels.Add(LeftPanel);
                panels.Add(RightPanel);

                foreach (var panel in panels)
                {
                    XmlNodeList H3NodeList = panel.SelectNodes("/aside/div/div/h3");

                    foreach (XmlNode h3 in H3NodeList)
                    {
                        if (h3.InnerText.IndexOf("камеры") < 0)
                        {
                            XmlNodeList ANodeList = h3.NextSibling.SelectNodes("li/a");

                            foreach (XmlNode a in ANodeList)
                            {

                                Station station = new Station();

                                station.PlaylistService = pls.FirstOrDefault();
                                station.Name = a.FirstChild.InnerText + " (" + h3.InnerText + ")";
                                station.Alias = a.Attributes["href"].Value;
                                station.Rating = 0;

                                if (!Db.StationRepository.Any(c => c.Name == station.Name))
                                {
                                    Db.StationRepository.Add(station);    
                                }
                                

                            }
                        }
                    }
                }

                Db.Save();




            }
        }
    }
}

