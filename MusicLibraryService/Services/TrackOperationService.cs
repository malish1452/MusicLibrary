using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq.Expressions;
using DataHelper.DBWork;
using DataHelper.Enums;
using DataHelper.Models;


namespace MusicLibraryService.Services
{
    public interface ITrackOperationService
    {
        void ParseSingleMixedName(Track track);
        void ParseNewMixedNames();

        void SearchSingleTrack(Track track);

        void SearchTracks();
        ParseName ParseMixedName(string trueMixedName);

    }
    public class TrackOperationService:BaseService,ITrackOperationService
    {
        public TrackOperationService(IDataProvider db) : base(db)
        {
            
        }

        public void SearchTracks()
        {
            foreach (Track track in Db.TrackRepository.Where(c => c.StatusId == (int)TrackStatusEnum.Распознан).Take(5))
            {
                SearchSingleTrack(track);
            }

            Db.Save();
            
        }

        public void SearchSingleTrack(Track track)
        {
            string url = String.Format("https://www.mp3poisk.me/search/{0}", WebUtility.UrlEncode(track.MixedName));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch, br");
            request.Connection = "Alive";
            HttpWebResponse response;
            try
            {

                response = request.GetResponse() as HttpWebResponse;

            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;

            }
            catch (Exception ex)
            {
                track.StatusId = (int)TrackStatusEnum.Нераспознан;
                return;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                track.StatusId = (int)TrackStatusEnum.Нераспознан;
                return;
            }



            Stream httpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(httpStream, Encoding.UTF8);
            string result = reader.ReadToEnd();
            if (result.IndexOf("<li itemprop=\"track\"") < 0)
            {
                return;
            }
            result = result.Substring(result.IndexOf("<li itemprop=\"track\"") - 1);

            while ((result.IndexOf("<li itemprop=\"track\"") > 0) && (track.StatusId != (int)TrackStatusEnum.Скачан))
            {
                string substring = result.Substring(0, result.IndexOf("</li>"));
                int index = substring.IndexOf("<span class=\"bitrait hide-on-small-mobile\"><strong>");
                int bitrate;
                if (index > 0)
                {
                    bool res = int.TryParse(substring.Substring(index + 51, 4).Trim(), out bitrate);
                    if (!res) { bitrate = 0; }
                    if (bitrate == 320)
                    {
                        substring = substring.Substring(substring.IndexOf("content =") + 10);
                        string adress = substring.Substring(0, substring.IndexOf("\""));
                        WebClient DownLoader = new WebClient();                         
                        DownLoader.DownloadFile(adress, Path.Combine(@"C:\MusicLibrary\", String.Format("{0}.{1}.mp3", track.Id, track.MixedName.Replace("#","").Replace("*", "").Replace("|", ""))));

                        track.StatusId = (int)TrackStatusEnum.Скачан;
                    }
                }
                result = result.Substring(result.IndexOf("</li>") + 5);
            }
            if (track.StatusId != (int)TrackStatusEnum.Скачан)
            { track.StatusId = (int)TrackStatusEnum.Нераспознан; }

        }

        public string NormalizeText(string text)
        {

            string result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
            return result;


        }
        
        public void ParseNewMixedNames()
        {

            var _tracks=Db.TrackRepository.Where(c => c.StatusId == (int) TrackStatusEnum.Новый).Take(20);
            foreach (Track track in _tracks)
            {
                try
                {
                    ParseSingleMixedName(track);
                }
                catch
                {
                    track.StatusId = (int) TrackStatusEnum.Нераспознан;
                }
            }

            Db.Save();

            foreach (Track track in Db.TrackRepository.Where(c=>c.StatusId==(int)TrackStatusEnum.Мусор))
            {
                Db.TrackRepository.Delete(track);
            }
            Db.Save();
        }

        public ParseName ParseMixedName(string trueMixedName)
        {

            ParseName resultName = new ParseName();
            if ((trueMixedName.IndexOf("-") != trueMixedName.LastIndexOf("-"))||trueMixedName.IndexOf("-")<0)
            {
                //два дефиса в названии пока не судьба или нет дефиса
                return null;
            }

            string part1 = trueMixedName.Substring(0, trueMixedName.LastIndexOf("-")-1);

            string part2 = trueMixedName.Substring(trueMixedName.LastIndexOf("-")+1);

             part1 = part1.Contains("feat")
                        ? part1.Substring(0, part1.IndexOf("feat") - 1).Trim()
                        : part1.Trim();
            part2 = part2.Contains("feat")
                        ? part2.Substring(0, part2.IndexOf("feat") - 1).Trim()
                        : part2.Trim();

            part1 = part1.Contains("&")
           ? part1.Substring(0, part1.IndexOf("&") - 1).Trim()
           : part1.Trim();
            part2 = part2.Contains("&")
                        ? part2.Substring(0, part2.IndexOf("&") - 1).Trim()
                        : part2.Trim();

            if (Db.MusicianRepository.Any(c => c.Name.ToLower() == part1.ToLower()))
            {
                //part1 - исполнитель
                //part2 - название
                resultName.Musician = Db.MusicianRepository.Where(c=>c.Name.ToLower()==part1.ToLower()).First().Name;
                resultName.Name = part2;
            }
            else if (Db.MusicianRepository.Any(c => c.Name.ToLower() == part2.ToLower()))
            {
                resultName.Musician = Db.MusicianRepository.Where(c => c.Name.ToLower() == part2.ToLower()).First().Name;
                resultName.Name = part1;
                
            }
            else
            {  
                //отправляем строку музыка + part 1 ищем процент результатов Part1+part 2

                int part1IsMusician = CheckBySearch(part1, part2);
                int part2IsMusician = CheckBySearch(part2, part1);

                if (((part1IsMusician >= part2IsMusician)&&(part1IsMusician>0))||(part2IsMusician==0))
                {
                    resultName.Musician = part1.Contains("feat")
                        ? part1.Substring(0, part1.IndexOf("feat") - 1).Trim()
                        : part1.Trim();
                    resultName.Name = part2;
                }
                else
                {
                    resultName.Musician = part2.Contains("feat")
                      ? part2.Substring(0, part2.IndexOf("feat") - 1).Trim()
                      : part2.Trim();
                    resultName.Name = part1;
                }

                //Musician musician = new Musician();
                //musician.Name = resultName.Musician;
                //musician.Score = 0;
                //Db.MusicianRepository.Add(musician);
            }

            return resultName;
        }

        public void ParseSingleMixedName(Track track)

        {
            string url = String.Format("http://192.168.0.69/yandexproxy/api/values?SearchString={0}", WebUtility.UrlEncode(track.MixedName));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            request.ContentType = "aplication/text";

            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (Exception ex)
            {
                return;
            }
            Stream httpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(httpStream, Encoding.UTF8);
            string result = reader.ReadToEnd();
            result=result.Replace("\\\"","\"").Replace("\\r","").Replace("\\n","").Trim('"');

            List<String> searchResultTitles = new List<string>();

            XmlDocument searchResult = new XmlDocument();
            searchResult.LoadXml(result);

            XmlNodeList titlesList = searchResult.SelectNodes("/yandexsearch/response/results/grouping/group/doc/title");

            foreach (XmlNode xmlNode in titlesList)
            {
                searchResultTitles.Add(" "+xmlNode.InnerXml.Replace("<hlword>", " ").Replace("</hlword>", " ").Replace("  ", " ").Replace("  ", " ").Replace(" FT ", " feat ").Replace(" ft ", " feat ").Replace(" Ft ", " feat ").Replace(" ft.", " feat ").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace(".", "").Replace("—", "-").Replace("–", "-").Replace("MP3", "").Replace("mp3", "").Replace("  ", " ").Replace("  ", " ").Replace("- YouTube", "").Replace("скачать","").Replace("бесплатно","").Replace("слушать","").Replace("Слушать",""));
            }

            List<string> cleanderResults =CleanList(CleanList(searchResultTitles),false);

            if (cleanderResults.Count == 0)
            {
                //oops
                //foreach (var mixedName in track.MixedNames)
                //{
                //    Db.MixedNamesRepository.Delete(mixedName);
                //}

                track.StatusId = (int)TrackStatusEnum.Мусор;
                // Db.TrackRepository.Delete(track);

                return;
            }

            List<PrevWord> possibleCleanName = new List<PrevWord>();

            foreach (string cleanderResult in cleanderResults)
            {
                if (possibleCleanName.All(c => c.Word != cleanderResult))
                {    
                    PrevWord prevWord = new PrevWord();
                    prevWord.Word = cleanderResult;
                    prevWord.Count = 1;
                    possibleCleanName.Add(prevWord);
                }
                else
                {
                    possibleCleanName.Find(c => c.Word == cleanderResult).Count++;
                }
            }

            possibleCleanName.Sort();
            possibleCleanName.Reverse();

            string trueMixedName = NormalizeText(possibleCleanName[0].Word.Trim());

            if ((trueMixedName!=String.Empty)&&(trueMixedName[trueMixedName.Length - 1] == '-'))
                trueMixedName = trueMixedName.Substring(0, trueMixedName.Length - 2);

            ParseName parsedName =  ParseMixedName(trueMixedName);
            if (parsedName == null)
            {
                track.MixedNames.Add(new MixedNames{Track=track,MixedName = track.MixedName});
                track.MixedNames.Add(new MixedNames { Track = track, MixedName = trueMixedName });
                track.StatusId = (int) TrackStatusEnum.Нераспознан;
               // Db.TrackRepository.Delete(track);
                return;
            }

            //Распарсили что дальше:
            //по трумиксед есть полное совпадение - удаляем и прикрепляем текущий миксед к найденому
            //по парснейму есть полное совпадение - удаляем и прекрепляем текущий миксед b трумиксед к найденому
            //нет совпадений - переименовываем оставляем переводим в след статус.

            if (Db.MixedNamesRepository.Any(c => c.MixedName == trueMixedName && c.TrackId!=track.Id ))
            {
                Track trueTrack = Db.MixedNamesRepository.Where(c => c.MixedName == trueMixedName && c.TrackId != track.Id).First().Track;


                if (!Db.MixedNamesRepository.Any(c => c.MixedName == track.MixedName))
                {
                    Db.MixedNamesRepository.Add(new MixedNames
                    {
                        Track = trueTrack,
                        MixedName = track.MixedName
                    });
                }

                //track.StatusId = (int) TrackStatusEnum.Нераспознан;
                track.StatusId = (int) TrackStatusEnum.Мусор;
                // Db.TrackRepository.Delete(track);
            }
            else if (Db.TrackRepository.Any(c => c.Name == parsedName.Name && c.Musician.Name == parsedName.Musician && c.Id != track.Id))
            {

                Track trueTrack = Db.TrackRepository.Where(c => c.Name == parsedName.Name && c.Musician.Name == parsedName.Musician && c.Id != track.Id).First();

                if (!Db.MixedNamesRepository.Any(c => c.MixedName == track.MixedName))
                {
                    Db.MixedNamesRepository.Add(new MixedNames
                    {
                        Track = trueTrack,
                        MixedName = track.MixedName
                    });
                }

                if (!Db.MixedNamesRepository.Any(c => c.MixedName == trueMixedName))
                {
                    Db.MixedNamesRepository.Add(new MixedNames
                    {
                        Track = trueTrack,
                        MixedName = trueMixedName
                    });
                }

                //track.StatusId = (int)TrackStatusEnum.Нераспознан;

                track.StatusId = (int)TrackStatusEnum.Мусор;
                // Db.TrackRepository.Delete(track);
            }
            else
            {
                Db.MixedNamesRepository.Add(new MixedNames
                {
                    Track = track,
                    MixedName = track.MixedName
                });

                track.MixedName = trueMixedName;
                
                Db.MixedNamesRepository.Add(new MixedNames
                {
                    Track = track,
                    MixedName = trueMixedName
                });

                track.Name = parsedName.Name;

                if (Db.MusicianRepository.Any(c => c.Name == parsedName.Musician))
                {
                    track.Musician = Db.MusicianRepository.Where(c => c.Name == parsedName.Musician).First();

                }
                else
                {
                    track.Musician = Db.MusicianRepository.Add(new Musician
                    {
                        Name = parsedName.Musician
                    });
                }

                track.StatusId = (int) TrackStatusEnum.Распознан;
            }



        }

        private int CheckBySearch(string part1, string part2 )
        {
            if (part1.Contains("feat")) part1 = part1.Substring(0, part1.IndexOf("feat") - 1);
            if (part2.Contains("feat")) part2 = part2.Substring(0, part2.IndexOf("feat") - 1);

            string url = String.Format("https://yandex.com/search/xml?user=kirpichnikov-ro&key=03.431424486:f54dbc8d2b13cdb8d5576f9534edc84e&query={0}&l10n=en&sortby=rlv&filter=none&groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D10.docs-in-group%3D1", WebUtility.UrlEncode("музыка " + part1));



            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;


            WebResponse response = request.GetResponse();

            Stream httpStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(httpStream, Encoding.UTF8);
            string result = reader.ReadToEnd();

            List<String> searchResultTitles = new List<string>();

            XmlDocument searchResult = new XmlDocument();
            searchResult.LoadXml(result);

            XmlNodeList titlesList = searchResult.SelectNodes("/yandexsearch/response/results/grouping/group/doc/title");

            foreach (XmlNode xmlNode in titlesList)
            {
                searchResultTitles.Add(" " + xmlNode.InnerXml.Replace("<hlword>", " ").Replace("</hlword>", " ").Replace("  ", " ").Replace("  ", " ").Replace(" FT ", " feat ").Replace(" ft ", " feat ").Replace(" Ft ", " feat ").Replace(" ft.", " feat ").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<").Replace(".", "").Replace("—", "-").Replace("–", "-").Replace("MP3", "").Replace("mp3", "").Replace("  ", " ").Replace("  ", " ").Replace("- YouTube", "").Replace("скачать", "").Replace("бесплатно", "").ToLower());
            }
            decimal a = searchResultTitles.Count(c => c.Contains(part2.ToLower()));
            decimal b = searchResultTitles.Count();
            decimal percentage = ((b - a)/b)*100;
           return (int)Math.Round(percentage);
        }

        private List<string> CleanList(List<string> input,bool devideFeat=true)
        {
            List<Vocabulary> vocabularyList = new List<Vocabulary>();

            foreach (string resultTitle in input)
            {
                var splitWord = resultTitle.Split(' ');

                for (int i = 1; i < splitWord.Length; i++)
                {
                    if (splitWord[i] != "")
                    {
                        Vocabulary Word = vocabularyList.Find(c => c.LowerWord == splitWord[i].ToLower());

                        if (Word != null)
                        {

                            Word.Count++;
                            //TODO: приоритет заглавных букв ???

                            string prevWordStr = i > 1 ? splitWord[i - 1] : "!@#$";

                            PrevWord prevWordSearch = Word.Prev.Find(c => c.LowerWord == prevWordStr.ToLower());

                            if (prevWordSearch != null)
                            {
                                prevWordSearch.Count++;
                            }
                            else
                            {
                                PrevWord prevWord = new PrevWord();

                                if (i == 1)
                                {
                                    prevWord.Word = "!@#$";
                                    prevWord.LowerWord = "!@#$";
                                }
                                else
                                {
                                    prevWord.Word = splitWord[i - 1];
                                    prevWord.LowerWord = splitWord[i - 1].ToLower();
                                }
                                prevWord.Count = 1;
                                Word.Prev.Add(prevWord);
                            }
                        }
                        else
                        {
                            Vocabulary newWord = new Vocabulary();
                            newWord.Word = splitWord[i];
                            newWord.LowerWord = splitWord[i].ToLower();
                            newWord.Count = 1;
                            newWord.Prev = new List<PrevWord>();
                            PrevWord prevWord = new PrevWord();

                            if (i == 1)
                            {
                                prevWord.Word = "!@#$";
                                prevWord.LowerWord = "!@#$";
                            }
                            else
                            {
                                prevWord.Word = splitWord[i - 1];
                                prevWord.LowerWord = splitWord[i - 1].ToLower();
                            }

                            prevWord.Count = 1;
                            newWord.Prev.Add(prevWord);
                            vocabularyList.Add(newWord);


                        }
                    }
                }

            }

            vocabularyList.Sort();
            vocabularyList.Reverse();

            var toDelete = vocabularyList.Where(c => c.Count < (input.Count/3));

            List<string> cleanedSearchResultTitle = new List<string>();

            foreach (string searchResultTitle in input)
            {
                string temp = searchResultTitle;
              
                foreach (Vocabulary vocabulary in toDelete)
                { 
                   if (vocabulary.Word!="feat")
                    temp = (temp+" ").Replace(" "+vocabulary.Word + " ", " ").Replace("  ", " ");
                }
               
                if ( (input.Count(c => c.Contains(" feat ")) <= 5)||(input.Count(c => c.Contains(" feat ")) > 5 && temp.Contains(" feat "))|| (!devideFeat))
                    cleanedSearchResultTitle.Add(temp);
            }

          

            return cleanedSearchResultTitle;
        }
    }

   
}


// vocabularyList.RemoveAll(c=>c.Count<3);

// foreach (Vocabulary vocabulary in vocabularyList)
// {
//     vocabulary.Prev.Sort();
//     vocabulary.Prev.Reverse();
// }

// var possibleFirstWord=vocabularyList.FindAll(c => c.Prev[0].LowerWord == "!@#$");
// var firstWord = possibleFirstWord.Where(c => c.Prev[0].Count ==possibleFirstWord.Max(z => z.Prev[0].Count) );
// Vocabulary word = new Vocabulary();

// int min = 99;

//     foreach (Vocabulary vocabulary in firstWord)
//     {
//         if (mixedName.ToLower().IndexOf(vocabulary.LowerWord) >= 0 && mixedName.ToLower().IndexOf(vocabulary.LowerWord) < min)
//         {
//             min = mixedName.ToLower().IndexOf(vocabulary.LowerWord);
//             word = vocabulary;
//         }
//     }


// string correctMixedName = String.Empty;


// Vocabulary nextVocabulary =new Vocabulary();//= vocabularyList.First(c => c.Prev[0].Word == word.Word);

// List<Vocabulary> usedVocabularies = new List<Vocabulary>();

// bool found = true;

//while (found)
// { 
//     int max = 0;
//     found = false;
//     foreach (Vocabulary vocabulary in vocabularyList)
//     {

//         if ((vocabulary.Prev.Any(c => c.Word == word.Word))&&(max < (vocabulary.Prev.Where(c => c.Word == word.Word).Max(z => z.Count)))&&(!usedVocabularies.Any(c=>c.Word==vocabulary.Word)))
//         {
//             max = vocabulary.Prev.Where(c => c.Word == word.Word).Max(z => z.Count);
//             nextVocabulary = vocabulary;
//             found = true;
//         }


//     }

//     correctMixedName = correctMixedName + " " + word.Word;
//     usedVocabularies.Add(word);
//     word = nextVocabulary;

//    if (max < 6)
//     {
//         found = false;
//     }


//    //nextVocabulary = vocabularyList.First(c => c.Prev[0].Word == word.Word);
// }