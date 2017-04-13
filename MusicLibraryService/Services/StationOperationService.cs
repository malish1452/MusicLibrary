using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DataHelper.DBWork;
using DataHelper.Enums;
using DataHelper.Models;
using MusicLibraryService.Data;


namespace MusicLibraryService.Services
{

    public interface IStationOperationService
    {
       void ProcessStations();

        void ProcessStation(Station station);

        void ProcessPLRadioStation(Station station);
    }
    class StationOperationService:BaseService,IStationOperationService
    {
        public  StationOperationService(IDataProvider db):base(db)
        {
            
        }

        public string NormalizeText(string text)
        {

            string result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
            return result;


        }

        public void ProcessStations()
        {
            DateTime timeToProcess = DateTime.Now.AddHours(-6);
            var _stationsToProcess =
                Db.StationRepository.Where(c => (c.LastUpdateTime <= timeToProcess)||(c.LastUpdateTime == null)).Take(5);

            foreach (Station station in _stationsToProcess)
            {
                station.LastUpdateTime=DateTime.Now;
               
                ProcessStation(station);
            }
             Db.Save();

             var names = Db.TrackRepository.GetAll().GroupBy(z => z.MixedName).Where(c => c.Count() > 1);

             foreach (var name in names)
             {
                 string currentName = name.First().MixedName;

                 var track = Db.TrackRepository.FirstOrDefault(c => c.MixedName == currentName);

                 foreach (
                     Track trackToDelete in
                         Db.TrackRepository.Where(c => c.MixedName == track.MixedName && c.Id != track.Id))
                 {

                     //foreach (var mixedName in trackToDelete.MixedNames)
                     //{
                     //    Db.MixedNamesRepository.Delete(mixedName);
                     //}
                     
                     Db.TrackRepository.Delete(trackToDelete);
                 }
             }
             Db.Save();
        }

        public void ProcessStation(Station station)
        {
            switch (station.PlaylistServiceId)
            {
                case (int)PlaylistServicesEnum.plradio_tw1_ru: ProcessPLRadioStation(station);
                    break;

            }
        }

        public void ProcessPLRadioStation(Station station)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(station.PlaylistService.Url+station.Alias);

            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;


            WebResponse response = request.GetResponse();

            Stream httpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(httpStream, Encoding.UTF8);
            string result = reader.ReadToEnd();

            result = result.Substring(result.IndexOf("<div id=\"system\">"));
            result = result.Substring(0, result.IndexOf("</div>")+6);

            XmlDocument phpXml = new XmlDocument();
            phpXml.LoadXml(result);

            response.Close();

            if (phpXml.FirstChild.FirstChild.Attributes["src"].Value.IndexOf("plradio") < 0) return;

            request = (HttpWebRequest)WebRequest.Create(phpXml.FirstChild.FirstChild.Attributes["src"].Value);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            response = request.GetResponse();
            httpStream.Close();
            httpStream = response.GetResponseStream();
            reader.Close();
            reader = new StreamReader(httpStream, Encoding.UTF8);

            result = reader.ReadToEnd();

            result = result.Substring(result.LastIndexOf("</form></span>")+14);
            result = "<html>" + result;
            result =
                result.Replace("target=_blank", "")
                    .Replace("border=0 src=", "src=\"")
                    .Replace("border=0>", "\"/>")
                    .Replace("&", "&amp;")
                    .Replace("<br>", "")
                    .Replace("</body>", "");
            XmlDocument xmlResult = new XmlDocument();
            
            xmlResult.LoadXml(result);

            XmlNodeList strongNodesList = xmlResult.SelectNodes("/html/span/strong");

            foreach (XmlNode strongNode in strongNodesList)
            {
                string mixedName = strongNode.InnerText.Substring(strongNode.InnerText.IndexOf(":: ") + 3);

            if (!(Db.TrackRepository.Any(c=>c.MixedName==mixedName)||(Db.MixedNamesRepository.Any(c=>c.MixedName==mixedName))))
                {
                    Track newTrack = new Track();
                    newTrack.MixedName = NormalizeText(mixedName) ;
                    newTrack.Name = String.Empty;
                    newTrack.StatusId = (int) TrackStatusEnum.Новый;
                    var track = Db.TrackRepository.Add(newTrack);
                    Db.StationRepository.Where(c => c.Id == station.Id).First().Tracks.Add(track);
                }
            }
#region Old code
            //result = result.Substring(result.IndexOf("</strong>") + 9);
            //int i = 0;
            //if (result.IndexOf("<strong>")>0)
            //while  (i<20)// (result.IndexOf("<strong>") > -1)
            //{
            //    string mixedName = result.Substring(result.IndexOf("<strong>"),
            //        result.IndexOf("</strong>") - result.IndexOf("<strong>"));
            //    mixedName = mixedName.Substring(mixedName.IndexOf(":: ") + 3);
                
            //    Track newTrack = new Track();
            //    newTrack.MixedName = mixedName;
            //    newTrack.Name=String.Empty;
            //    newTrack.StatusId = (int) TrackStatusEnum.Новый;
            //    var track = Db.TrackRepository.Add(newTrack);
            //    Db.StationRepository.Where(c => c.Id == station.Id).First().Tracks.Add(track);

            //    //trackService.CreateNewByMixedName(mixedName);
            //    result = result.Substring(result.IndexOf("</strong>") + 9);
            //    i++;
            //}

#endregion
        }

    }
}
