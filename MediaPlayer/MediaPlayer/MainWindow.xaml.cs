using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // playing Video
        public Video _currentPlaying = new Video();

        // most recent video played
        public Video _recentPlaying = new Video();

        // if true -> playing video
        // if false -> no video playing
        private bool _isPlaying = false;


        // playing playlist
        public Playlist _currentPlaylist = new Playlist();

        // save no shuffe playlist
        public Playlist _noShufflePlaylist = new Playlist();

        // last playlist played
        Playlist recentPlaylist = new Playlist();

        // index of current playing playlist: -1
        // -> no playlist playing
        public int _indexCurrentPlayingPlaylist = -1;

        // time
        DispatcherTimer _timer;

        // hook keyboard
        private IKeyboardMouseEvents _hook;

        // list of current playlist
        ObservableCollection<Playlist> _playlists = new ObservableCollection<Playlist>();

        // save path of current playlist
        string playlistPath = AppDomain.CurrentDomain.BaseDirectory + "Playlists";

        // save path of data when close window
        string DataPath = AppDomain.CurrentDomain.BaseDirectory + "Data";

        // filter openfile
        const string videoExtension = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;" +
            "*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;" +
            "*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;" +
            "*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";

        bool _isShuffle = false;
        int _preIndex = -1;

        // Window Loaded: Load saved playlist + Recent Playlist + Recent played files
                        // + create hook event + create folder Playlist/Data
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(playlistPath);
            Directory.CreateDirectory(DataPath);

            _currentPlaying.VideoURL = "";

            LoadPlaylist();
            LoadRecentPlays();

            homeButton_Click(this, new RoutedEventArgs());

            // Hooks
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += _hook_KeyUp;
        }

        // handle hook keyboard
        private void _hook_KeyUp(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(e.Control && e.Alt && (e.KeyCode == Keys.N))
            {
                // Ctrl + Alt + N -> Next Video
                nextButton_Click(this, new RoutedEventArgs());
            }

            if(e.Control && e.Alt && (e.KeyCode == Keys.P))
            {
                // Ctrl + Alt + P -> Previous Video
                prevButton_Click(this, new RoutedEventArgs());
            }

            if(e.Control && e.Alt && (e.KeyCode == Keys.B))
            {
                // Ctrl + Alt + B -> Pause/Play Video
                playVideoButton_Click(this, new RoutedEventArgs());
            }
        }

        // Delete hook event + save recent play files
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _hook.KeyUp -= _hook_KeyUp;
            _hook.Dispose();

            recentPlaylist = _currentPlaylist;
            SaveRecentPlays();

            Environment.Exit(0);
        }

        // delegate/event change current playling
        private void change_curPlaying(Video newVideo)
        {
            _currentPlaying = (Video)newVideo.Clone();
        }

        // delegate/event change current playlist
        private void change_curPlaylist(Playlist newPlaylist)
        {
            _currentPlaylist = (Playlist)newPlaylist.Clone();
        }

        // display home page: playing video + recent playlist
        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            Home.DataContext = _currentPlaying;
            if(recentPlaylist.VideoList.Count > 0)
            {
                recentPlayListName.ItemsSource = recentPlaylist.VideoList;
            }

            Home.Visibility = Visibility.Visible;
            PlaylistPage.Visibility = Visibility.Collapsed;
            PlaylistInforPage.Visibility = Visibility.Collapsed;
            PlayQueuePage.Visibility = Visibility.Collapsed;

            playlistButtonText.FontWeight = FontWeights.Normal;
            homeButtonText.FontWeight = FontWeights.Bold;
            playQueueButtonText.FontWeight = FontWeights.Normal;
        }

        // display play queue page: list of videos in current playlist
        private void playQueueButton_Click(object sender, RoutedEventArgs e)
        {
            Home.Visibility = Visibility.Collapsed;
            PlaylistPage.Visibility = Visibility.Collapsed;
            PlaylistInforPage.Visibility = Visibility.Collapsed;
            PlayQueuePage.Visibility = Visibility.Visible;

            playlistButtonText.FontWeight = FontWeights.Normal;
            homeButtonText.FontWeight = FontWeights.Normal;
            playQueueButtonText.FontWeight = FontWeights.Bold;

            if(_isPlaying == true && _indexCurrentPlayingPlaylist > -1)
            {
                listPlayQueue.ItemsSource = _currentPlaylist.VideoList;
                PlayQueuePage.DataContext = _currentPlaylist;
            }
            else
            {
                listPlayQueue.ItemsSource = null;
                PlayQueuePage.DataContext = null;
            }

            _noShufflePlaylist = _currentPlaylist;
        }

        // display playlist page: list of playlists
        private void playlistButton_Click(object sender, RoutedEventArgs e)
        {
            listPlaylist.ItemsSource = _playlists;

            Home.Visibility = Visibility.Collapsed;
            PlaylistPage.Visibility = Visibility.Visible;
            PlaylistInforPage.Visibility = Visibility.Collapsed;
            PlayQueuePage.Visibility = Visibility.Collapsed;

            playlistButtonText.FontWeight = FontWeights.Bold;
            homeButtonText.FontWeight = FontWeights.Normal;
            playQueueButtonText.FontWeight = FontWeights.Normal;
        }

        // Continue to play when click Now Playing
        private void playingButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                _currentPlaying.hour = videoPlayer.Position.Hours;
                _currentPlaying.minute = videoPlayer.Position.Minutes;
                _currentPlaying.second = videoPlayer.Position.Seconds + 0.5;

                _isPlaying = true;
                _currentPlaying.isPlaying = true;
                _hook.KeyUp -= _hook_KeyUp;
                _hook.Dispose();
                _hook = Hook.GlobalEvents();
                _hook.KeyUp += _hook_KeyUp;
                player_MediaOpened(this, new RoutedEventArgs());

                playQueueButton_Click(this, new RoutedEventArgs());
            }
        }

        // Play the most recent video played
        private void recentPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(_isPlaying == false && recentPlaylist.VideoList.Count > 0)
            {
                Home.Visibility = Visibility.Collapsed;
                PlaylistPage.Visibility = Visibility.Collapsed;
                PlaylistInforPage.Visibility = Visibility.Collapsed;
                PlayQueuePage.Visibility = Visibility.Visible;
                videoPlayer.Visibility = Visibility.Visible;
                mp3bg.Visibility = Visibility.Collapsed;


                _currentPlaylist = new Playlist();
                _currentPlaylist = (Playlist)recentPlaylist.Clone();
                _currentPlaying = (Video)_recentPlaying.Clone();


                Home.Visibility = Visibility.Collapsed;
                PlaylistPage.Visibility = Visibility.Collapsed;
                PlaylistInforPage.Visibility = Visibility.Collapsed;
                PlayQueuePage.Visibility = Visibility.Visible;


                listPlayQueue.ItemsSource = _currentPlaylist.VideoList;
                PlayQueuePage.DataContext = _currentPlaylist;
                _noShufflePlaylist = _currentPlaylist;

                _isPlaying = true;
                _currentPlaying.isPlaying = true;
                _hook.KeyUp -= _hook_KeyUp;
                _hook.Dispose();

                _hook = Hook.GlobalEvents();
                _hook.KeyUp += _hook_KeyUp;
                player_MediaOpened(this, new RoutedEventArgs());
            }
        }

        // add playlist
        private void addPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new AddPlaylist();
            bool check = true;
            if(screen.ShowDialog() == true)
            {
                var playlist = screen.NewPlaylist;
                if(playlist.PlaylistName != "")
                {
                    foreach (var p in _playlists)
                    {
                        if(p.PlaylistName == playlist.PlaylistName)
                        {
                            check = false;
                        }
                    }
                    if(check)
                    {
                        playlist.isShuffle = false;
                        _playlists.Add(playlist);
                    }
                }
            }
        }
        
        // view information of playlist
        private void viewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var index = listPlaylist.SelectedIndex;

            if(index >= 0 && index < _playlists.Count) 
            {
                Home.Visibility = Visibility.Collapsed;
                PlaylistPage.Visibility = Visibility.Collapsed;
                PlaylistInforPage.Visibility = Visibility.Visible;
                PlayQueuePage.Visibility = Visibility.Collapsed;

                playlistButtonText.FontWeight = FontWeights.Normal;
                homeButtonText.FontWeight = FontWeights.Bold;
                playQueueButtonText.FontWeight = FontWeights.Normal;

                _currentPlaylist = _playlists[index];
                _indexCurrentPlayingPlaylist = index;
                PlaylistInforPage.DataContext = _currentPlaylist;
                listVideo.ItemsSource = _playlists[index].VideoList;
            }
        }

        // delete playlist when right click on playlist -> choose delete
        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(_playlists.Count != 0)
            {
                int i = listPlaylist.SelectedIndex;
                if(i == _indexCurrentPlayingPlaylist)
                {
                    _indexCurrentPlayingPlaylist = -1;
                    _currentPlaying.VideoURL = "";
                }

                string filename = playlistPath + "//" + _playlists[i].PlaylistName + ".playlist";

                _playlists.RemoveAt(i);

                try
                {
                    // check if file exist with its full path
                    if(File.Exists(filename))
                    {
                        // if file found, delete it
                        File.Delete(filename);
                        Console.WriteLine("File deleted");
                        System.Windows.MessageBox.Show("File deleted", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Console.WriteLine("File not found");
                        System.Windows.MessageBox.Show("File not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } catch(IOException ioEx)
                {
                    Console.WriteLine(ioEx.Message);
                    System.Windows.MessageBox.Show(ioEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void playPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            int i = listPlaylist.SelectedIndex;
            _isPlaying = false;
            if (_playlists[i].VideoList.Count > 0 && i >= 0)
            {
                Home.Visibility = Visibility.Collapsed;
                PlaylistPage.Visibility = Visibility.Collapsed;
                PlaylistInforPage.Visibility = Visibility.Collapsed;
                PlayQueuePage.Visibility = Visibility.Visible;
                videoPlayer.Visibility = Visibility.Visible;
                mp3bg.Visibility = Visibility.Collapsed;

                _currentPlaylist = new Playlist();
                _currentPlaylist = (Playlist)_playlists[i].Clone();
                _indexCurrentPlayingPlaylist = i;
                _currentPlaying = (Video)_currentPlaylist.VideoList[0].Clone();
                _isPlaying = true;
                _currentPlaying.isPlaying = true;
                _hook.KeyUp -= _hook_KeyUp;
                _hook.Dispose();
                _hook = Hook.GlobalEvents();
                _hook.KeyUp += _hook_KeyUp;
                player_MediaOpened(this, new RoutedEventArgs());

                playQueueButton_Click(this, new RoutedEventArgs());
            }
        }

        // Add video to playlist
        private void addSongButton_Click(object sender, RoutedEventArgs e)
        {
            int i = listPlaylist.SelectedIndex;
            var screen = new Microsoft.Win32.OpenFileDialog();
            screen.Filter = videoExtension;
            screen.Multiselect = true;
            if(screen.ShowDialog() == true)
            {
                foreach(var filename in screen.FileNames)
                {
                    Video video = new Video();
                    video.VideoURL = filename;
                    _playlists[i].VideoList.Add(video);
                }
            }
        }

        // Click Delete: Delete watching playlist
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            int i = listPlaylist.SelectedIndex;
            if(i == _indexCurrentPlayingPlaylist)
            {
                _indexCurrentPlayingPlaylist = -1;
                _currentPlaying.VideoURL = "";
            }

            string filename = playlistPath + "//" + _playlists[i].PlaylistName + ".playlist";

            _playlists.RemoveAt(i);

            try
            {
                // check if file exist with its full path
                if(File.Exists(filename))
                {
                    // if file found, delete it
                    File.Delete(filename);
                    Console.WriteLine("File deleted.");
                    System.Windows.MessageBox.Show("File deleted", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Console.WriteLine("File not found");
                    System.Windows.MessageBox.Show("File not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } catch(IOException ioEx)
            {
                Console.WriteLine(ioEx.Message);
                System.Windows.MessageBox.Show(ioEx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            playlistButton_Click(this, new RoutedEventArgs());
        }

        // Click Save: Save playlist + show notification
        private void savePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            int i = listPlaylist.SelectedIndex;
            SavePlaylist(_playlists[i]);
            System.Windows.MessageBox.Show("Saved playlist", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        // delete video when right click on video -> choose remove
        private void deleteVideo_Click(object sender, RoutedEventArgs e)
        {
            int i = listPlaylist.SelectedIndex;
            int index = listVideo.SelectedIndex;
            _playlists[i].VideoList.RemoveAt(index);
            if(i == _indexCurrentPlayingPlaylist)
            {
                _currentPlaylist = _playlists[i];
                _currentPlaying.playlistIndex = -1;
            }
        }

        private void player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                videoPlayer.Source = new Uri(_currentPlaying.VideoURL, UriKind.Absolute);
                _isPlaying = true;
                videoPlayer.Volume = 100;

                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
                _timer.Tick += _timer_Tick;

                // Slider
                progressSlider.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(progressSlider_MouseLeftButtonUp), true);
                progressSlider.ValueChanged += progressSlider_ValueChanged;

                if(videoPlayer.NaturalDuration.HasTimeSpan)
                {
                    int hours = videoPlayer.NaturalDuration.TimeSpan.Hours;
                    int minutes = videoPlayer.NaturalDuration.TimeSpan.Minutes;
                    int seconds = videoPlayer.NaturalDuration.TimeSpan.Seconds;
                    totalPosition.Text = $"{hours}:{minutes}:{seconds}";
                    progressSlider.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                }

                currentPosition.Text = $"{_currentPlaying.hour}:{_currentPlaying.minute}:{_currentPlaying.second}";
                progressSlider.Value = _currentPlaying.hour * 3600 + _currentPlaying.minute * 60 + _currentPlaying.second;

                videoPlayer.Position = TimeSpan.FromSeconds(_currentPlaying.hour * 3600 + _currentPlaying.minute * 60 + _currentPlaying.second);
                videoPlayer.Play();
                _timer.Start();

                // can't recognize current positon
                if(_currentPlaying.isPlaying == false)
                {
                    // Not auto play after finish song  => comment
                    // playVideoButton_Click(this, new RoutedEventArgs());
                }
            }
            else
            {
                progressSlider.Value = 0;
                progressSlider.Maximum = 0;
                currentPosition.Text = "0:0:0";
                totalPosition.Text = "0:0:0";
            }
            videoName.Text = _currentPlaying.VideoName;
            if(isPlayingAudio())
            {
                videoPlayer.Visibility = Visibility.Collapsed;
                mp3bg.Visibility = Visibility.Visible;
            }
            else
            {
                videoPlayer.Visibility = Visibility.Visible;
                mp3bg.Visibility = Visibility.Collapsed;
            }
            panel_nowPlaying.Visibility = Visibility.Visible;

            if(_preIndex >= 0)
            {
                _currentPlaylist.VideoList[_preIndex].displayMode = "Hidden";
            }
            _currentPlaylist.VideoList[_currentPlaying.playlistIndex].displayMode = "Visible";
        }

        private void progressSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(videoPlayer.Source != null)
            {
                videoPlayer.Position = TimeSpan.FromSeconds(progressSlider.Value);
            }
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            if(videoPlayer.Source != null)
            {
                int hours = videoPlayer.Position.Hours;
                int minutes = videoPlayer.Position.Minutes;
                int seconds = videoPlayer.Position.Seconds;
                var currentPost = videoPlayer.Position;

                currentPosition.Text = $"{hours}:{minutes}:{seconds}";
                progressSlider.Value = currentPost.TotalSeconds;
            }
        }

        private void player_MediaEnded(object sender, RoutedEventArgs e)
        {
            _currentPlaying.isPlaying = false;
            if(_currentPlaying.playlistIndex >= _currentPlaylist.VideoList.Count -1)
            {
                videoPlayer.Stop();
            }
            else
            {
                videoPlayer.Stop();
                _currentPlaying.VideoURL = _currentPlaylist.VideoList[_currentPlaying.playlistIndex + 1].VideoURL;
                _preIndex = _currentPlaying.playlistIndex;
                ++_currentPlaying.playlistIndex;
                videoPlayer.Source = new Uri(_currentPlaying.VideoURL, UriKind.Absolute);
                _isPlaying = true;
                videoPlayer.Volume = 100;
                videoName.Text = _currentPlaying.VideoName;
            }
        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                if(_currentPlaying.playlistIndex > 0)
                {
                    videoPlayer.Stop();
                    _currentPlaying.VideoURL = _currentPlaylist.VideoList[_currentPlaying.playlistIndex - 1].VideoURL;
                    _preIndex = _currentPlaying.playlistIndex;
                    --_currentPlaying.playlistIndex;

                    videoPlayer.Source = new Uri(_currentPlaying.VideoURL, UriKind.Absolute);
                    _isPlaying = true;
                    videoPlayer.Volume = 100;
                }
            }
        }

        // shuffle playlist
        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if(_isShuffle)
            {
                _isShuffle = false;
                _currentPlaylist = _noShufflePlaylist;
                shuffleButtonText.Text = "Shuffle: Off";
                change_curPlaylist(_noShufflePlaylist);
            }
            else
            {
                shuffleButtonText.Text = "Shuffle: On";
                Random random = new Random();
                int n = _currentPlaylist.VideoList.Count;

                while(n > 0)
                {
                    n--;
                    int k = random.Next(n + 1);
                    if(k == _currentPlaying.playlistIndex)
                    {
                        _currentPlaying.playlistIndex = n;
                    }
                    Video value = _currentPlaylist.VideoList[k];
                    _currentPlaylist.VideoList[k] = _currentPlaylist.VideoList[n];
                    _currentPlaylist.VideoList[n] = value;
                }

                _isShuffle = true;
                _currentPlaylist.isShuffle = true;
                change_curPlaylist(_currentPlaylist);
            }
        }

        // play/pause video
        private void playVideoButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                if(_isPlaying)
                {
                    _isPlaying = false;
                    videoPlayer.Pause();
                    _timer.Stop();

                    playVideoButtonText.Text = "Play";
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(@"/img/play.png", UriKind.Relative);
                    bitmap.EndInit();
                    playVideoButtonIcon.Source = bitmap;
                }
                else
                {
                    _isPlaying = true;
                    videoPlayer.Play();
                    _timer.Start();

                    playVideoButtonText.Text = "Pause";
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(@"/img/pause.png", UriKind.Relative);
                    bitmap.EndInit();
                    playVideoButtonIcon.Source = bitmap;
                }
            }
        }

        // Stop
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                videoPlayer.Stop();
                _isPlaying = false;

                playVideoButtonText.Text = "Play";
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(@"/img/play.png", UriKind.Relative);
                bitmap.EndInit();
                playVideoButtonIcon.Source = bitmap;
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if(_currentPlaying.VideoURL != "")
            {
                if(_currentPlaying.playlistIndex < _currentPlaylist.VideoList.Count - 1)
                {
                    videoPlayer.Stop();
                    _currentPlaying.VideoURL = _currentPlaylist.VideoList[_currentPlaying.playlistIndex + 1].VideoURL;
                    _preIndex = _currentPlaying.playlistIndex;
                    ++_currentPlaying.playlistIndex;

                    videoPlayer.Source = new Uri(_currentPlaying.VideoURL, UriKind.Absolute);
                    _isPlaying = true;
                    videoPlayer.Volume = 100;
                }
            }
        }

        private void progressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(videoPlayer.Source != null)
            {
                double value = progressSlider.Value;
                TimeSpan newPosition = TimeSpan.FromSeconds(value);
                videoPlayer.Position = newPosition;
            }
        }

        // load recent played files
        private void LoadRecentPlays()
        {
            var filename = DataPath + "//" + "data.dat";
            try
            {
                string[] lines = File.ReadAllLines(filename);
                if(lines.Length >= 4)
                {
                    recentPlaylist.PlaylistName = "Recent Playlist";
                    recentPlaylist.VideoList.Clear();
                    _indexCurrentPlayingPlaylist = Int32.Parse(lines[0]);
                    if (lines[1] == "0")
                    {
                        for(int i = 2; i < lines.Length - 2; i++)
                        {
                            if (lines[i] != "" && lines[i] != null)
                            {
                                Video video = new Video(lines[i]);
                                recentPlaylist.VideoList.Add(video);
                            }
                        }
                        _recentPlaying = recentPlaylist.VideoList[0];
                    }
                    if (lines[1] == "1")
                    {
                        for(int i = 5; i < lines.Length - 2; i++)
                        {
                            if (lines[i] != "" && lines[i] != null)
                            {
                                Video video = new Video(lines[i]);
                                recentPlaylist.VideoList.Add(video);
                            }
                        }
                        _recentPlaying = new Video(lines[lines.Length - 2], Int32.Parse(lines[lines.Length - 1]));
                        int hour = Int32.Parse(lines[2]);
                        int minute = Int32.Parse(lines[3]);
                        int second = Int32.Parse(lines[4]);
                        _recentPlaying.hour = hour;
                        _recentPlaying.minute = minute;
                        _recentPlaying.second = second;
                    }
                }
            } catch
            {
                File.Create(filename);
            }
        }

        private bool isPlayingAudio()
        {
            string fileExt = (_currentPlaying.VideoName.Substring(_currentPlaying.VideoName.Length - 3)).ToUpper();
            return Regex.IsMatch(fileExt, @"((WAV)|(AIF)|(MP3)|(MID))$");
        }

        // save current playlist -> recent
        private void SaveRecentPlays()
        {
            string filename = DataPath + "//" + "data.dat";
            var writer = new StreamWriter(filename);
            writer.WriteLine(_indexCurrentPlayingPlaylist);
            if(_currentPlaying.VideoURL == "")
            {
                writer.WriteLine(0);
            }
            else
            {
                writer.WriteLine(1);
                writer.WriteLine(videoPlayer.Position.Hours.ToString());
                writer.WriteLine(videoPlayer.Position.Minutes.ToString());
                writer.WriteLine(videoPlayer.Position.Seconds.ToString());
            }
            foreach(var video in recentPlaylist.VideoList)
            {
                writer.WriteLine(video.VideoURL);
            }
            writer.WriteLine(_currentPlaying.VideoURL);
            writer.WriteLine(_currentPlaying.playlistIndex);
            writer.Close();
        }

        private void LoadPlaylist()
        {
            string[] lists = Directory.GetFiles(playlistPath, "*.playlist");
            foreach (var item in lists)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(item);
                Playlist playlist = new Playlist(name);

                string[] videos = File.ReadAllLines(item);
                foreach (var video in videos)
                {
                    Video v = new Video(video);
                    playlist.VideoList.Add(v);
                }
                _playlists.Add(playlist);
            }
        }

        private void SavePlaylist(Playlist p)
        {
            string filename = playlistPath + "//" + p.PlaylistName + ".playlist";
            var writer = new StreamWriter(filename);
            foreach (var video in p.VideoList)
            {
                writer.WriteLine(video.VideoURL);
            }
            writer.Close();
        }
    }
}