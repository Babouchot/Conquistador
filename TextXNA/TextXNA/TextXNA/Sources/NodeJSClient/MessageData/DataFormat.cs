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

}
