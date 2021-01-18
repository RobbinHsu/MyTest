using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spider
{
    public class GetGamePlayer
    {
        private readonly string[] _letters = new string[]
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "letter", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u",
            "v", "w", "x", "y", "z"
        };

        private TaskFactory _factory;
        private StringBuilder _answer;
        private List<Task> _taskList;
        private string _ans;
        private Match _nameSite;
        private string _baller;
        private string _ballerRecord;

        private const string Url = @"https://www.basketball-reference.com/players/";
        private const string FilePate = @"C:\Answer\";
        private const string Title = "Player,G,PTS,TRB,AST,FG(%),FG3(%),FT(%),eFG(%),PER,WS";

        public void GetLetter()
        {
            foreach (var letter in _letters)
            {
                var letterPage = GetHtmlByURL(Url + letter + "/"); // 個別字母球員列表頁面
                if (string.IsNullOrEmpty(letterPage))
                {
                    continue;
                }

                _answer = new StringBuilder();
                _answer.AppendLine(Title);
                var letterRows = Regex.Split(letterPage, @"<tr ><th");

                for (int i = 1; i < letterRows.Length; i++)
                {
                    var letterRow = letterRows[i];
                    if (IsLetterNotNull(letterRow, letter))
                    {
                        continue;
                    }

                    GetLetterPlayer(_answer);
                }

                File.WriteAllText(letter + ".csv", _answer.ToString());
                Console.WriteLine($"{letter.ToUpper()} is OK");
            }
        }

        private string GetHtmlByURL(string url)
        {
            var html = string.Empty;
            try
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream());
                html = Nancy.Helpers.HttpUtility.HtmlDecode(stream.ReadToEnd());
                response.Close();
                stream.Close();
            }
            catch (Exception)
            {
                Console.WriteLine($"{url} is failed.");
            }

            return html;
        }

        private void GetLetterPlayer(StringBuilder answer)
        {
            var values =
                Regex.Matches(_ballerRecord, @"<p>(?<valueOne>[^<]*)</p>\n<p>(?<valueTwo>[^<]*)</p>"); // 擷取個人數據

            string dataOne = "";
            string dataTwo = "";
            _ans = string.Empty;
            _ans += _baller;
            foreach (Match value in values)
            {
                var valueOne = value.Groups["valueOne"].Value;
                dataOne += "," + valueOne;
                var valueTwo = value.Groups["valueTwo"].Value;
                dataTwo += "," + valueTwo;
            }

            if (dataOne.Length > 10) // 判斷個人數據是否為兩行
            {
                answer.AppendLine(_ans + dataOne + "\n" + " " + dataTwo); // 加入兩行個人數據
            }
            else
            {
                answer.AppendLine(_ans + dataTwo);
            }

            Console.WriteLine($"{_baller} is join");
        }

        private bool IsLetterNotNull(string letterRow, string letter)
        {
            _nameSite = Regex.Match(letterRow, @"<a href=""/players/\w/(?<name>[^.]*).html"">(?<baller>[^<]*)<");

            _baller = _nameSite.Groups["baller"].Value;
            if (string.IsNullOrEmpty(_nameSite.Groups["name"].Value) ||
                string.IsNullOrEmpty(_nameSite.Groups["baller"].Value))
            {
                return true;
            }

            _ballerRecord = GetHtmlByURL(Url + letter + "/" + _nameSite.Groups["name"].Value + ".html");
            if (string.IsNullOrEmpty(_ballerRecord))
            {
                return true;
            }

            return false;
        }
    }
}