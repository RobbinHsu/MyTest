using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spider
{
    public class GetGamePlayer
    {
        private readonly string[] _letters = new string[]
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u",
            "v", "w", "x", "y", "z"
        };

        private StringBuilder _answer;
        private string _ans;
        private Match _nameSite;
        private string _baller;
        private string _ballerRecord;
        private List<string> _letterPages;
        private string[] _letterRows;
        private List<string> _playerName;
        private Dictionary<string, string> _ballerRecords;

        public GetGamePlayer()
        {
            _letterPages = new List<string>();
            _playerName = new List<string>();
            _ballerRecords = new Dictionary<string, string>();
        }

        private const string Url = @"https://www.basketball-reference.com/players/";
        private const string Title = "Player,G,PTS,TRB,AST,FG(%),FG3(%),FT(%),eFG(%),PER,WS";

        public void GetLetter()
        {
            foreach (var letter in _letters)
            {
                var letterPage = (Url + letter + "/").GetHtmlByURL(); // 個別字母球員列表頁面
                if (string.IsNullOrEmpty(letterPage))
                {
                    continue;
                }

                _letterPages.Add(letterPage);
            }

            foreach (var letterPage in _letterPages)
            {
                _letterRows = Regex.Split(letterPage, @"<tr ><th");

                foreach (var letterRow in _letterRows)
                {
                    _nameSite = Regex.Match(letterRow,
                        @"<a href=""/players/\w/(?<name>[^.]*).html"">(?<baller>[^<]*)<");

                    var baller = _nameSite.Groups["baller"].Value;
                    var name = _nameSite.Groups["name"].Value;
                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(baller))
                    {
                        continue;
                    }

                    _playerName.Add(name);
                }
            }

            foreach (var name in _playerName)
            {
                var letter = name.Substring(0, 1);
                _ballerRecord =
                    (Url + letter + "/" + name + ".html").GetHtmlByURL();
                if (string.IsNullOrEmpty(_ballerRecord))
                {
                    continue;
                }

                _ballerRecords.Add(name, _ballerRecord);
            }

            foreach (var ballerRecord in _ballerRecords)
            {
                _answer = new StringBuilder();
                _answer.AppendLine(Title);
                _letterRows = Regex.Split(ballerRecord.Value, @"<tr ><th");

                var letterRow = _letterRows[0];
                GetLetterPlayer(letterRow);
                File.WriteAllText(ballerRecord.Key + ".csv", _answer.ToString());
                Console.WriteLine($"{ballerRecord.Key.ToUpper()} is OK");
            }
        }

        private void GetLetterPlayer(string ballerRecord)
        {
            _answer.Clear();
            var values =
                Regex.Matches(ballerRecord, @"<p>(?<valueOne>[^<]*)</p>\n<p>(?<valueTwo>[^<]*)</p>"); // 擷取個人數據

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
                _answer.AppendLine(_ans + dataOne + "\n" + " " + dataTwo); // 加入兩行個人數據
            }
            else
            {
                _answer.AppendLine(_ans + dataTwo);
            }

            Console.WriteLine($"{_baller} is join");
        }

        private bool IsLetterNotNull(string letterRow, string letterPage)
        {
            _nameSite = Regex.Match(letterRow, @"<a href=""/players/\w/(?<name>[^.]*).html"">(?<baller>[^<]*)<");

            _baller = _nameSite.Groups["baller"].Value;
            if (string.IsNullOrEmpty(_nameSite.Groups["name"].Value) ||
                string.IsNullOrEmpty(_nameSite.Groups["baller"].Value))
            {
                return true;
            }

            _ballerRecord = (Url + letterPage + "/" + _nameSite.Groups["name"].Value + ".html").GetHtmlByURL();
            if (string.IsNullOrEmpty(_ballerRecord))
            {
                return true;
            }

            return false;
        }
    }
}