using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestXNA.Sources.NodeJSClient.MessageData
{
    public class PlayerInfoRoot
    {
        public string name { get; set; }
        public List<PlayerInfo> args { get; set; }
    };

    public class PlayerInfo
    {
        public int gameID { get; set; }
        public string pseudo { get; set; }
        public int score { get; set; }
        public List<int> territories { get; set; }
    }

    public class CaptureInfoRoot
    {
        public string name { get; set; }
        public List<CaptureInfo> args { get; set; }
        //public List<List<int>> args { get; set; }
    };

    public class CaptureInfo
    {
        public List<int> orderedPlayers{ get; set; }
    };

    public class QuestionData
    {
        public string type { get; set; }
        public string title { get; set; }
        public int answer { get; set; }
        public int id { get; set; }
    }

    public class QuestionDataRoot
    {
        public string name { get; set; }
        public List<QuestionData> args { get; set; }
    }

}
