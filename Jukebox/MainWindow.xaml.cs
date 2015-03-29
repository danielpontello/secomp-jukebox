using SpotifyAPI.SpotifyLocalAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using SpotifyEventHandler = SpotifyAPI.SpotifyLocalAPI.SpotifyEventHandler;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Windows.Threading;

namespace Jukebox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int lastT;

        Song[] musicasVotacao;

        List<Song> musicas;
        List<Song> JaFoi;
        Song musicaAtual;

        Random r;

        SpotifyLocalAPIClass spotify;
        SpotifyMusicHandler mh;

        DispatcherTimer timer;

        bool firstTime = true;

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            StartSpotify();
            lastT = 0;
            musicasVotacao = new Song[5];
            musicas = new List<Song>();
            r = new Random();

            PopulateSongList();
            Thread.Sleep(5000);

            timer = new DispatcherTimer();
            timer.Tick += (snd, evt) =>
            {
                if (!firstTime)
                {
                    int maior = 0;
                    int maiorIndice = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        if (musicasVotacao[i].votos > maior)
                        {
                            maior = musicasVotacao[i].votos;
                            maiorIndice = i;
                        }
                    }
                    /*
                    Bar1.Width = 512 * ((float)musicasVotacao[0].votos / (float)(maior));
                    Bar2.Width = 512 * ((float)musicasVotacao[1].votos / (float)(maior));
                    Bar3.Width = 512 * ((float)musicasVotacao[2].votos / (float)(maior));
                    Bar4.Width = 512 * ((float)musicasVotacao[3].votos / (float)(maior));
                    Bar5.Width = 512 * ((float)musicasVotacao[4].votos / (float)(maior));
                    */

                    musicaAtual = musicasVotacao[maiorIndice];
                }
                else
                {
                    musicaAtual = musicas[r.Next(musicas.Count)];
                    firstTime = false;
                }

                // Sorteia novas musicas
                for (int i = 0; i < 5; i++)
                {
                    Song s = musicas[r.Next(musicas.Count)];

                    if (musicasVotacao.Contains(s) || musicasVotacao.Contains(musicaAtual))
                    {
                        s = musicas[r.Next(musicas.Count)];
                    }
                    musicasVotacao[i] = s;
                }

                foreach (Song s in musicasVotacao)
                {
                    s.votos = 0;
                }

                NowPlayingTitle.Text = musicaAtual.Name;
                NowPlayingArtist.Text = musicaAtual.Artist;
                NowPlayingAlbum.Text = "";

                PlayMusic();

                Bar1.Width = 0;
                Bar2.Width = 0;
                Bar3.Width = 0;
                Bar4.Width = 0;
                Bar5.Width = 0;

                UpdateNames();
                UpdateCovers();
            };
            timer.Interval = TimeSpan.FromSeconds(60.0f);

            for (int i = 0; i < 5; i++)
            {
                Song s = musicas[r.Next(musicas.Count)];
                if (musicasVotacao.Contains(s))
                {
                    s = musicas[r.Next(musicas.Count)];
                }
                musicasVotacao[i] = s;
            }
            musicaAtual = musicas[r.Next(musicasVotacao.Length)];

            NowPlayingTitle.Text = musicaAtual.Name;
            NowPlayingArtist.Text = musicaAtual.Artist;


            UpdateCovers();
            UpdateNames();
            timer.Start();
        }

        public void UpdateNames()
        {
            Votes1.Text = "" + musicasVotacao[0].votos;
            Votes2.Text = "" + musicasVotacao[1].votos;
            Votes3.Text = "" + musicasVotacao[2].votos;
            Votes4.Text = "" + musicasVotacao[3].votos;
            Votes5.Text = "" + musicasVotacao[4].votos;

            Artist1.Text = musicasVotacao[0].Artist;
            Artist2.Text = musicasVotacao[1].Artist;
            Artist3.Text = musicasVotacao[2].Artist;
            Artist4.Text = musicasVotacao[3].Artist;
            Artist5.Text = musicasVotacao[4].Artist;

            Title1.Text = musicasVotacao[0].Name;
            Title2.Text = musicasVotacao[1].Name;
            Title3.Text = musicasVotacao[2].Name;
            Title4.Text = musicasVotacao[3].Name;
            Title5.Text = musicasVotacao[4].Name;
        }

        public void PopulateSongList()
        {
            string[] lines = File.ReadAllLines(@"Resources/TrackList.txt");

            foreach (string s in lines)
            {
                string[] split = s.Split(new Char[] { '|' });
                string musica = split[0];
                string artista = split[1];
                string link = split[2];

                musicas.Add(new Song(musica, artista, link));
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            lastT--;

            if (lastT < 1920)
                lastT = 1920;

            foreach (TextBlock t in TickerCanvas.Children)
            {
                Canvas.SetLeft(t, Canvas.GetLeft(t) - 3);
            }

            double y = Canvas.GetTop(ImgBackground);

            if (y < -1080)
            {
                Canvas.SetTop(ImgBackground, y - 1.0f);
            }

        }

        public void UpdateCovers()
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    timer.Interval = TimeSpan.FromMilliseconds(t.duration_ms + 5000);
                    NowPlayingAlbum.Text = t.album.name;
                    Console.WriteLine(t.duration_ms);
                    NowPlayingImage.Source = b;
                    ImgBackground.Source = b;
                }
                catch (Exception exc)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(7200000);
                }
            };
            string id = musicaAtual.URI.Substring(30);

            Console.WriteLine("ID: " + id);
            client.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id, UriKind.Absolute));

            WebClient client1 = new WebClient();
            WebClient client2 = new WebClient();
            WebClient client3 = new WebClient();
            WebClient client4 = new WebClient();
            WebClient client5 = new WebClient();

            client1.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    Cover1.Source = b;
                }
                catch (Exception exc)
                {

                }
            };

            client2.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    Cover2.Source = b;
                }
                catch (Exception exc)
                {

                }
            };

            client3.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    Cover3.Source = b;
                }
                catch (Exception exc)
                {

                }
            };

            client4.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    Cover4.Source = b;
                }
                catch (Exception exc)
                {

                }
            };

            client5.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    Track t = JsonConvert.DeserializeObject<Track>(e.Result);
                    BitmapImage b = new BitmapImage();
                    b.BeginInit();
                    b.UriSource = new Uri(t.album.images[0].url, UriKind.Absolute);
                    b.EndInit();
                    Cover5.Source = b;
                }
                catch (Exception exc)
                {

                }
            };

            string id1 = musicasVotacao[0].URI.Substring(30);
            string id2 = musicasVotacao[1].URI.Substring(30);
            string id3 = musicasVotacao[2].URI.Substring(30);
            string id4 = musicasVotacao[3].URI.Substring(30);
            string id5 = musicasVotacao[4].URI.Substring(30);

            client1.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id1, UriKind.Absolute));
            client2.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id2, UriKind.Absolute));
            client3.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id3, UriKind.Absolute));
            client4.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id4, UriKind.Absolute));
            client5.DownloadStringAsync(new Uri("https://api.spotify.com/v1/tracks/" + id5, UriKind.Absolute));
        }

        void PlayMusic()
        {

        }

        private void BtnGetTweets_Click(object sender, RoutedEventArgs e)
        {
            TwitterCredentials.SetCredentials("Seu Access Token", "Seu Access Secret", "Sua Consumer Key", "Seu Consumer Secret");
            var filteredStream = Tweetinvi.Stream.CreateFilteredStream();
            filteredStream.AddTrack("#JukeboxInatel");
            filteredStream.MatchingTweetReceived += (s, a) =>
            {
                // Antibot
                if (a.Tweet.Text.Contains("jukebot") || a.Tweet.Text.Contains("@t411"))
                {

                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (a.Tweet.Text.Contains("1"))
                        {
                            musicasVotacao[0].votos++;
                        }
                        else

                            if (a.Tweet.Text.Contains("2"))
                            {
                                musicasVotacao[1].votos++;
                            }
                            else

                                if (a.Tweet.Text.Contains("3"))
                                {
                                    musicasVotacao[2].votos++;
                                }
                                else

                                    if (a.Tweet.Text.Contains("4"))
                                    {
                                        musicasVotacao[3].votos++;
                                    }
                                    else

                                        if (a.Tweet.Text.Contains("5"))
                                        {
                                            musicasVotacao[4].votos++;
                                        }

                        UpdateNames();

                        int maior = 0;
                        int maiorIndice = 0;

                        for (int i = 0; i < 5; i++)
                        {
                            if (musicasVotacao[i].votos > maior)
                            {
                                maior = musicasVotacao[i].votos;
                                maiorIndice = i;
                            }
                        }

                        if (maior > 0)
                        {
                            Bar1.Width = 700 * ((float)musicasVotacao[0].votos / (float)(maior));
                            Bar2.Width = 700 * ((float)musicasVotacao[1].votos / (float)(maior));
                            Bar3.Width = 700 * ((float)musicasVotacao[2].votos / (float)(maior));
                            Bar4.Width = 700 * ((float)musicasVotacao[3].votos / (float)(maior));
                            Bar5.Width = 700 * ((float)musicasVotacao[4].votos / (float)(maior));
                        }
                        TextBlock t = new TextBlock();
                        t.FontSize = 36;
                        t.FontWeight = FontWeights.Light;
                        t.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        t.Text = "@" + a.Tweet.Creator.ScreenName + ": " + a.Tweet.Text.Replace('\n', ' ') + " // ";

                        TickerCanvas.Children.Add(t);
                        Canvas.SetLeft(t, lastT);

                        t.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        lastT += (int)t.DesiredSize.Width;
                    }));
                }
            };
            var thread = new Thread(() => filteredStream.StartStreamMatchingAllConditions());
            thread.Start();
        }

        public void StartSpotify()
        {
            spotify = new SpotifyLocalAPIClass();

            if (!SpotifyLocalAPIClass.IsSpotifyRunning())
            {
                spotify.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyLocalAPIClass.IsSpotifyWebHelperRunning())
            {
                spotify.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            try
            {
                if (!spotify.Connect())
                {
                    MessageBox.Show("Couldn't connect to Spotify");
                    return;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("TRETA DONE");
            }

            mh = spotify.GetMusicHandler();

            if (mh == null)
            {
                MessageBox.Show("Handler is null");
            }
        }

        private void BtnResetStream_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
