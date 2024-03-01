using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer
{
    public class Video : INotifyPropertyChanged, ICloneable
    {
        public string VideoURL { get; set; }

        public string VideoName
        {
            get
            {
                var tokens = VideoURL.Split(new string[] { "\\" }, StringSplitOptions.None);
                if(tokens.Length > 1)
                {
                    return tokens[tokens.Length - 1];
                }
                else
                {
                    tokens = VideoURL.Split(new string[] { "/" }, StringSplitOptions.None);
                    return tokens[tokens.Length - 1];
                }
            }
        }

        public int playlistIndex { get; set; }

        public int hour { get; set; }

        public int minute { get; set; }

        public double second { get; set; }

        public bool isPlaying { get; set; }

        public string displayMode { get; set; }

        public Video(string url, int index = 0)
        {
            VideoURL = url;
            isPlaying = false;
            hour = 0;
            minute = 0;
            second = 0;
            displayMode = "Hidden";
            playlistIndex = index;
        }

        public Video()
        {
            VideoURL = "";
            isPlaying = false;
            hour = 0;
            minute = 0;
            second = 0;
            displayMode = "Hidden";
            playlistIndex = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
