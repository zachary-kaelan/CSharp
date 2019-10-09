using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using NeuralNetworksLib;
using NeuralNetworksLib.NetworkModels;
using TinderAPI;
using TinderAPI.Models;
using TinderAPI.Models.Images;
using TinderAPI.Models.Recommendations;
using TinderPlus.Properties;
using ZachLib;
using ZachLib.Logging;
using ZachLib.Statistics;

namespace TinderPlus
{
    public partial class Form1 : Form
    {
        public static readonly string PATH_TEMP = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Temp\Tinder\";
        public static readonly string PATH_TEMP_CURR_PHOTOS = PATH_TEMP + @"Current Photos\";
        public const string PATH_SAVED_HOTTIES = @"G:\CCLeaner\resources\assets\Stalking\Random People\Tinder\";
        public const string PATH_SAVED_SOULMATES = @"E:\TEMP\Tinder Soulmates\";
        private static readonly DateTime _now = DateTime.Now;
        private static readonly double _meYearsOld = (new DateTime(1999, 8, 5) - _now).TotalDays / 365;

        private static readonly BaseRecFilter[] _filters = new BaseRecFilter[]
        {
            new StringRecFilter("strSugar", "sugar daddy"),
            new ArrayRecFilter(
                "arrAstrology", 
                new string[] {
                    "Aries",
                    "Taurus",
                    "Gemini",
                    "Cancer",
                    "Leo",
                    "Virgo",
                    "Libra",
                    "Scorpio",
                    "Sagittarius",
                    "Capricorn",
                    "Aquarius",
                    "Pisces"
                }
            ),
            new StringRecFilter("strJustAsk", "just ask"),
            new StringRecFilter("strVibes", "vibes"),
            new StringRecFilter("strSell", "sell"), // someone looking for a dealer
            new ArrayRecFilter(
                "arrHobbies", 
                new string[] {
                    "travel",
                    "food", // love food; AKA fat
                    "vacation",
                    "hik", // hiking
                    "fun", // love to have fun
                    "laugh" // love to laugh; key to making me laugh is to be handsome
                }
            ),
            new StringRecFilter("strAdventure", "adventure"),
            new RegexRecFilter(
                "rgxPhoneFilter", // obvious scam
                new Regex(
                    @"\(?(\d{3})[\) -]*(\d{3})-?(\d{4})",
                    RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled
                )
            )
        };

        //public Image[] CurrentImages { get; private set; }
        //private NeuralNetwork _nn = new NeuralNetwork(1000, 0.25, 0.7, false, new int[] { 7, 5, 3, 1 });
        private bool _loaded = false;
        private List<Vector> _nnInputs = new List<Vector>();
        private RecommendationsEnumerator _recsEnum;
        private string _curTempPath;
        private string _curSavedPath;
        private string[] _curPhotos;
        private int _curPhotoIndex = 0;
        private PictureBox[] InstagramPhotos;
        private FileStream _historyWriter = null;
        private SortedSet<string> UserHistory = new SortedSet<string>();
        private SortedSet<uint> TopPicks = new SortedSet<uint>();

        private IProgress<LoadingProgress> _progress;
        //private FileStream _inputsFile = new FileStream("NNInputs.dat", FileMode.Append, FileAccess.ReadWrite);

        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(PATH_TEMP))
                Directory.CreateDirectory(PATH_TEMP);
            if (!Directory.Exists(PATH_TEMP_CURR_PHOTOS))
                Directory.CreateDirectory(PATH_TEMP_CURR_PHOTOS);

            InstagramPhotos = new PictureBox[]
            {
                pbxInsta1,
                pbxInsta2,
                pbxInsta3,
                pbxInsta4,
                pbxInsta5,
                pbxInsta6
            };
        }

        private void lblBio_Click(object sender, EventArgs e)
        {

        }

        public async void DisplayRec(Recommendation rec)
        {
            if (UserHistory.Contains(rec.User.ID))
            {
                LogManager.Enqueue(
                    API.LOG_NAME,
                    EntryType.DEBUG,
                    "Auto-passed duplicate " + rec.User.Name,
                    rec.User.ID
                );
                ++numAutoSwiped.Value;
                NextRec(RecInteraction.Pass, false, true);
                return;
            }
            else if (TopPicks.Contains(rec.s_number))
            {
                LogManager.Enqueue(
                    API.LOG_NAME,
                    EntryType.DEBUG,
                    "Auto-liked " + rec.User.Name,
                    rec.User.ID
                );
                ++numAutoSwiped.Value;
                NextRec(RecInteraction.Like, false, true);
                return;
            }
            else if (_filters.Any(f => f.Match(rec.User.Biography)))
            {
                ++numAutoSwiped.Value;
                NextRec(RecInteraction.Pass, true, true);
                return;
            }

            radDull.Checked = true;
            _curPhotoIndex = 0;

            var profile = rec.User;
            var birthday = Convert.ToDateTime(profile.Birthday);

            _curTempPath = PATH_TEMP_CURR_PHOTOS + profile.ID + "\\";
            _curSavedPath = PATH_SAVED_HOTTIES +
                    profile.Name + "_" + birthday.ToString("yyMMdd") + "\\";
            Directory.CreateDirectory(_curTempPath);

            _progress.Report(null);

            lblName.Text = profile.Name;
            lblBio.Text = (
                rec.Distance > 0 ?
                    rec.Distance + " miles away\r\n\r\n" :
                    ""
            ) + profile.Biography;
            double yearsOld = _meYearsOld;
            if (birthday.Year > 1960)
            {
                yearsOld = (_now - birthday).TotalDays / 365;
                lblName.Text += " - " + yearsOld.ToString("#.0");
            }

            _progress.Report(null);

            //double instaPhotos = -1;
            if (rec.Instagram != null)
            {
                lblInstaInfo.Text =
                    (rec.Instagram.Username != null ?
                        rec.Instagram.Username + "\r\n" : "") +
                        "Last Fetch: " + rec.Instagram.LastFetch.ToString() +
                        "\r\nNumber of Photos: " + rec.Instagram.AccountNumPhotos;
                string instaPath = _curTempPath + "instagram\\";
                //instaPhotos = rec.Instagram.Photos.Length / 10.0;
                if (rec.Instagram.Photos != null && rec.Instagram.Photos.Any())
                    await Task.Factory.StartNew(
                        () => GetInstaPhotos(instaPath, rec.Instagram)
                    );
            }
            else
                lblInstaInfo.ResetText();

            _progress.Report(null);

            numPhotoCount.Value = profile.Photos.Length;
            _curPhotos = new string[profile.Photos.Length];
            //Directory.Delete(PATH_TEMP_CURR_PHOTOS);
            await Task.Factory.StartNew(
                () => GetPhotos(profile.Photos),
                TaskCreationOptions.PreferFairness
            );

            numCurPhoto.Maximum = _curPhotos.Length - 1;
            if (numCurPhoto.Maximum > 0)
                numCurPhoto.Value = 1;
            numCurPhoto.Value = 0;
            numPhotoCount.Value = _curPhotos.Length - 1;
            //pbxPhotos.ImageLocation = _curTempPath + _curPhotos[0];

            _progress.Report(new LoadingProgress(true, -1));

            /*Vector inputs = new Vector(
                new double[] {
                    profile.Biography.Length <= 3 ? -1 : (double)profile.Biography.Length / 519,
                    (yearsOld / _meYearsOld) - 1,
                    profile.Photos.Length,
                    instaPhotos,
                    profile.Jobs != null && profile.Jobs.Any() ? 1 : -1,
                    profile.Schools != null && profile.Schools.Any() ? 1 : -1,
                    rec.Distance / 25.0
                }
            );
            _nnInputs.Add(inputs);
            byte[] temp = new byte[8];
            foreach (var dbl in inputs)
            {
                temp = BitConverter.GetBytes(dbl);
                _inputsFile.Write(temp, 0, 8);
            }*/
        }

        private async void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_loaded)
                return;

            var curRec = _recsEnum.Current;
            if (e.Control)
            {
                var curUser = curRec.User;
                /*var path = PATH_SAVED_HOTTIES +
                    curUser.Name + "_" + curUser.Birthday.ToString("YYMMDD") + "\\";*/

                if (e.Shift)
                {
                    switch (e.KeyData)
                    {
                        case Keys.S:
                            await Task.Factory.StartNew(
                                () => SaveAllPhotos(_curSavedPath),
                                TaskCreationOptions.PreferFairness
                            );
                            await Task.Factory.StartNew(
                                 () => SaveInsta(curRec.Instagram, _curSavedPath + "instagram\\"),
                                 TaskCreationOptions.PreferFairness
                             );
                            break;

                        case Keys.G:
                            await Task.Factory.StartNew(
                                 () => API.SaveAllRelevantInfo(curRec, _curSavedPath, 0),
                                 TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
                             );
                            break;

                    }
                }
                else
                {
                    switch (e.KeyData)
                    {
                        case Keys.S:
                            SaveCurrentPhoto();
                            break;

                        case Keys.G:
                            await Task.Factory.StartNew(
                                () => SaveInsta(curRec.Instagram, _curSavedPath + "instagram\\"),
                                TaskCreationOptions.PreferFairness
                            );
                            break;
                    }
                }
            }
            else
            {
                switch (e.KeyData)
                {
                    case Keys.E:
                        ++_curPhotoIndex;
                        if (_curPhotoIndex < _curPhotos.Length)
                            ++numCurPhoto.Value;
                        else
                            --_curPhotoIndex;
                        break;

                    case Keys.Q:
                        --_curPhotoIndex;
                        if (_curPhotoIndex >= 0)
                            --numCurPhoto.Value;
                        else
                            ++_curPhotoIndex;
                        break;

                    case Keys.N:
                        radBigNo.Checked = true;
                        break;

                    case Keys.U:
                        radUgly.Checked = true;
                        break;

                    case Keys.H:
                        radHot.Checked = true;
                        break;

                    case Keys.I:
                        radFun.Checked = true;
                        break;

                    case Keys.B:
                        radMate.Checked = true;
                        break;

                    case Keys.W:
                        lblLoading2.Text = "Displaying recommendation...";
                        _progress.Report(new LoadingProgress(8));
                        await Task.Factory.StartNew(
                            () => NextRec(RecInteraction.Super),
                            TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
                        );
                        DisplayRec(_recsEnum.Current);
                        break;

                    case Keys.A:
                        lblLoading2.Text = "Displaying recommendation...";
                        _progress.Report(new LoadingProgress(8));
                        await Task.Factory.StartNew(
                            () => NextRec(RecInteraction.Pass),
                            TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
                        );
                        DisplayRec(_recsEnum.Current);
                        break;

                    case Keys.D:
                        lblLoading2.Text = "Displaying recommendation...";
                        _progress.Report(new LoadingProgress(8));
                        await Task.Factory.StartNew(
                            () => NextRec(RecInteraction.Like),
                            TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
                        );
                        DisplayRec(_recsEnum.Current);
                        break;
                }
            }
        }

        private void NextRec(RecInteraction swipe, bool saveInfo = true, bool isAutoSwipe = false)
        {
            _progress.Report(new LoadingProgress("Displaying next recommendation..."));

            ClearPhotos();

            _progress.Report(new LoadingProgress("Swiping..."));
            bool isMatch = false;

            if (_recsEnum.Current.Swipe(swipe, out isMatch) && swipe == RecInteraction.Like)
                MessageBox.Show("Out of likes!", "TinderPlus");
            if (isMatch)
                MessageBox.Show("It's a match!", "TinderPlus");
            _progress.Report(null);

            _progress.Report(new LoadingProgress("Saving info..."));
            string id = _recsEnum.Current.User.ID;
            if (UserHistory.Add(id))
            {
                for (int i = 0; i < 24; i += 2)
                {
                    _historyWriter.WriteByte(Convert.ToByte(id.Substring(i, 2), 16));
                }
            }

            if (saveInfo)
            {
                float score = 0;
                if (isAutoSwipe)
                {
                    if (swipe == RecInteraction.Pass)
                        score = -0.99f;
                    else
                        score = 0.1f;
                }
                else
                {
                    switch (swipe)
                    {
                        case RecInteraction.Like:
                            score = -0.15f;
                            break;

                        case RecInteraction.Pass:
                            score = -0.49f;
                            break;

                        case RecInteraction.Super:
                            score = 0.34f;
                            break;
                    }

                    if (radUgly.Checked)
                        score -= 0.25f;
                    else if (radHot.Checked || radFun.Checked)
                        score += 0.25f;
                    else if (radMate.Checked)
                        score += 0.5f;
                    else if (radBigNo.Checked)
                        score -= 0.5f;

                    _progress.Report(null);
                }

                var infoPath = _curSavedPath + _recsEnum.Current.User.ID + ".json";
                if (Directory.Exists(_curSavedPath) && File.Exists(infoPath))
                    File.Copy(
                        infoPath, @"E:\Programming\Tinder\" +
                            _recsEnum.Current.User.Name + "_" +
                            _recsEnum.Current.User.Birthday.ToString("yyMMdd")
                    );
                else
                    API.SaveAllRelevantInfo(_recsEnum.Current, @"E:\Programming\Tinder\", score);

                _progress.Report(null);

                if (Directory.Exists(_curTempPath))
                    Directory.Delete(_curTempPath, true);
                if (Directory.Exists(_curSavedPath))
                {
                    var files = Directory.GetFiles(_curSavedPath);
                    if (files == null || !files.Any())
                        Directory.Delete(_curSavedPath);
                }
            }

            _recsEnum.MoveNext();
            while (!_recsEnum.Current.User.Photos.Any())
                _recsEnum.MoveNext();

            _progress.Report(null);
            //DisplayRec(_recsEnum.Current);
        }

        private void GetPhotos(Photo[] photos)
        {
            int photoIndex = 0;
            foreach (var photo in photos)
            {
                photo.DownloadToFolder(_curTempPath);
                _curPhotos[photoIndex] = photo.Filename;
                ++photoIndex;
            }
        }

        private void GetInstaPhotos(string instaPath, InstagramPhotoCollection instagram)
        {
            Directory.CreateDirectory(instaPath);
            int max = Math.Min(6, instagram.Photos.Length);
            for (int i = 0; i < max; ++i)
            {
                InstagramPhotos[i].ImageLocation = instagram.Photos[i].DownloadToFolder(instaPath);
            }
        }

        private void ClearPhotos()
        {
            pbxPhotos.ImageLocation = "";
            for (int i = 0; i < 6; ++i)
            {
                InstagramPhotos[i].ImageLocation = "";
            }
        }

        #region Save
        private void SaveCurrentPhoto()
        {
            if (!Directory.Exists(_curSavedPath))
                Directory.CreateDirectory(_curSavedPath);
            File.Copy(pbxPhotos.ImageLocation, PATH_SAVED_HOTTIES + _curPhotos[(int)numCurPhoto.Value]);
        }

        private void SaveAllPhotos(string path)
        {
            if (!Directory.Exists(_curSavedPath))
                Directory.CreateDirectory(_curSavedPath);
            foreach (var photo in _curPhotos)
            {
                File.Copy(_curTempPath + photo, path + photo);
            }
        }

        private void SaveInsta(InstagramPhotoCollection instagram, string path, bool otherStalkerInfo = false)
        {
            if (!Directory.Exists(_curSavedPath))
                Directory.CreateDirectory(_curSavedPath);
            //var instagram = profile.InstagramPhotos;

            if (instagram != null && !Directory.Exists(path) && Directory.Exists(_curTempPath + "instagram"))
            {
                Directory.CreateDirectory(path);

                foreach (var photo in Directory.GetFiles(_curTempPath + "instagram"))
                {
                    File.Copy(photo, path + Path.GetFileName(photo));
                }
                if (instagram.Photos.Length > 6)
                {
                    foreach (var instaPhoto in instagram.Photos.Skip(6))
                    {
                        instaPhoto.DownloadToFolder(path);
                    }
                }
                instagram.DownloadProfilePicToFolder(path);
                
                /*
                if (otherStalkerInfo)
                    new Dictionary<string, string>()
                    {
                        { "username", instagram.Username },
                        { "media_count", instagram.AccountNumPhotos.ToString() },
                        { "last_fetch_time", instagram.LastFetch.ToString() },
                        {
                            "birth_date",
                            !String.IsNullOrWhiteSpace(profile.BirthdayInfo) ?
                                ((int)((DateTime.Now - profile.Birthday).TotalDays / 365.25)).ToString() :
                                profile.Birthday.ToShortDateString()
                        },
                        { "city", profile.Location.City },
                        {
                            "occupations",
                            profile.Schools != null && profile.Schools.Any() ? (
                                    profile.Jobs != null && profile.Jobs.Any() ?
                                        profile.Schools.Select(s => s.Name).Concat(profile.Jobs.Select(j => j.Title + " - " + j.Company)).ToArrayString() :
                                        profile.Schools.Select(s => s.Name).ToArrayString()
                                ) : (
                                    profile.Jobs != null && profile.Jobs.Any() ?
                                        profile.Jobs.Select(j => j.Title + " - " + j.Company).ToArrayString() :
                                        ""
                                )
                        },
                        { "name", profile.Name }
                    }.SaveDictAs(path + "insta_stalker_info.txt");
                else
                    new Dictionary<string, string>()
                    {
                        { "username", instagram.Username },
                        { "media_count", instagram.AccountNumPhotos.ToString() },
                        { "last_fetch_time", instagram.LastFetch.ToString() }
                    }.SaveDictAs(path + "insta_stalker_info.txt");
                */
            }
        }

        private void SaveHotInfo()
        {
            if (!Directory.Exists(_curSavedPath))
                Directory.CreateDirectory(_curSavedPath);
            var curRec = _recsEnum.Current;
            var bioScanResults = API.BioScan(curRec.User.Biography);
            if (curRec.Instagram != null && (bioScanResults == null || !bioScanResults.ContainsKey("instagram")))
            {
                var instagram = curRec.Instagram;
                new Dictionary<string, string>()
                    {
                        { "username", instagram.Username == null ? "" : instagram.Username },
                        { "media_count", instagram.AccountNumPhotos.ToString() },
                        { "last_fetch_time", instagram.LastFetch.ToString() }
                    }.SaveDictAs(_curSavedPath + "insta_stalker_info.txt");

                if (!Directory.Exists(_curSavedPath + "instagram"))
                    SaveInsta(instagram, _curSavedPath + "instagram\\");
            }

            if (bioScanResults != null)
                bioScanResults.SaveDictAs(_curSavedPath + "bio_usernames.txt");
        }

        private void SaveMateInfo()
        {
            var curRec = _recsEnum.Current;
            var path = PATH_SAVED_SOULMATES + curRec.User.Name + "_" + curRec.User.Birthday.ToString("yyMMdd") + "\\";
            Directory.CreateDirectory(path);
            API.SaveAllRelevantInfo(curRec, path, 0);
            SaveAllPhotos(path);
            SaveInsta(curRec.Instagram, path + "instagram\\");
        }
        #endregion

        #region ControlEvents
        private void btnSaveCurrent_Click(object sender, EventArgs e)
        {
            SaveCurrentPhoto();
        }

        private async void btnSaveAll_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(
                () => SaveAllPhotos(_curSavedPath),
                TaskCreationOptions.PreferFairness
            );
        }

        private async void btnSaveInsta_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(
                () => SaveInsta(_recsEnum.Current.Instagram, _curSavedPath + "instagram\\"),
                TaskCreationOptions.PreferFairness
            );
        }

        private async void btnSaveAllInfo_Click(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(
                () => API.SaveAllRelevantInfo(_recsEnum.Current, _curSavedPath, 0),
                TaskCreationOptions.PreferFairness
            );
        }

        private void numCurPhoto_ValueChanged(object sender, EventArgs e)
        {
            _curPhotoIndex = (int)numCurPhoto.Value;
            pbxPhotos.ImageLocation = _curTempPath + _curPhotos[_curPhotoIndex];
        }

        private void radBigNo_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private async void radHot_CheckedChanged(object sender, EventArgs e)
        {
            lblLoading3.Text = "Saving hottie info...";
            if (radHot.Checked && !File.Exists(_curSavedPath + _recsEnum.Current.User.ID + ".json"))
            {
                await Task.Factory.StartNew(
                    () => SaveHotInfo(),
                    TaskCreationOptions.PreferFairness
                );
                lblLoading3.Text = "";
            }
        }

        private void radMate_CheckedChanged(object sender, EventArgs e)
        {
            lblLoading3.Text = "Saving soulmate info...";
            if (radMate.Checked)
                SaveMateInfo();
            lblLoading3.Text = "";
        }

        private void radDull_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radUgly_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radFun_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private async void Form1_Load(object sender, EventArgs e)
        {
            _progress = new Progress<LoadingProgress>(LoadingProgressHandler);

            lblLoading1.Text = "Initializing...";
            lblLoading2.Text = "Checking user history...";
            lblLoading3.Text = "";

            await Task.Factory.StartNew(
                GetHistory, 
                TaskCreationOptions.PreferFairness
            );

            await Task.Factory.StartNew(
                () => API.RefreshAuth(), 
                TaskCreationOptions.PreferFairness
            );

            lblLoading2.Text = "Checking Top Picks...";

            /*await Task.Factory.StartNew(
                () => CheckTopPicks(_progress),
                TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning
            );*/

            lblLoading1.Text = "Running...";
            lblLoading2.Text = "Fetching recommendations...";

            _recsEnum = API.GetRecsEnumerator();

            await Task.Factory.StartNew(
                () => _recsEnum.MoveNext(),
                TaskCreationOptions.PreferFairness
            );

            lblLoading2.Text = "Displaying recommendations...";
            DisplayRec(_recsEnum.Current);
            _loaded = true;
        }

        private void LoadingProgressHandler(LoadingProgress update)
        {
            if (update == null)
            {
                if (prgLoading.Value < prgLoading.Maximum)
                    ++prgLoading.Value;
                return;
            }

            if (update.type.HasFlag(ProgressType.LoadingInitialize))
            {
                prgLoading.Value = 0;
                if (update.loadingMax == -1)
                {
                    prgLoading.Hide();
                    if (!update.type.HasFlag(ProgressType.Text))
                        lblLoading3.Text = "";
                }
                else
                {
                    prgLoading.Maximum = update.loadingMax;
                    prgLoading.Show();
                }
            }

            if (update.type.HasFlag(ProgressType.Increment) && prgLoading.Value < prgLoading.Maximum)
                ++prgLoading.Value;
            if (update.type.HasFlag(ProgressType.Text))
            {
                lblLoading3.Text = update.txt;
                LogManager.Enqueue(
                    API.LOG_NAME,
                    EntryType.STATUS,
                    update.txt
                );
            }
        }

        private void GetHistory()
        {
            var fileExists = File.Exists("user_history.txt");
            _historyWriter = new FileStream("user_history.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            if (fileExists)
            {
                var length = (int)(_historyWriter.Length / 6);
                _progress.Report(new LoadingProgress(length, "Reading user history..."));

                byte[] bytes = new byte[6];
                while (_historyWriter.Read(bytes, 0, 6) == 6)
                {
                    string str = "";
                    for (int i = 0; i < 6; ++i)
                    {
                        str += Convert.ToString(bytes[i], 16);
                    }
                    UserHistory.Add(str);
                    _progress.Report(null);
                }
            }

            _progress.Report(new LoadingProgress(-1));
        }

        private void CheckTopPicks(IProgress<LoadingProgress> progress)
        {
            var fileExists = File.Exists("top_picks.txt");
            if (!fileExists)
            {
                progress.Report(new LoadingProgress(-1, "Creating values file..."));
            }

            using (FileStream file = new FileStream("top_picks.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
            {
                byte[] bytes = new byte[8];
                if (fileExists)
                {
                    var length = (int)(file.Length / 4);
                    progress.Report(new LoadingProgress(length, "Reading values file..."));
                    
                    while (file.Read(bytes, 0, 4) == 4)
                    {
                        TopPicks.Add(BitConverter.ToUInt32(bytes, 0));
                        progress.Report(null);
                    }
                }

                if (Settings.Default.top_picks_refresh == null || Settings.Default.top_picks_refresh < _now)
                {
                    progress.Report(new LoadingProgress(-1, "Fetching..."));
                    var newTopPicks = API.GetTopPicks();

                    progress.Report(new LoadingProgress(newTopPicks.Length, "Downloading Photos..."));
                    foreach(var topPick in newTopPicks)
                    {
                        if (!TopPicks.Add(topPick.s_number))
                            continue;
                        bytes = BitConverter.GetBytes(topPick.s_number);
                        file.Write(bytes, 0, 4);

                        var id = topPick.User.Name + "_" + topPick.User.Birthday.ToString("yyMMdd");
                        var path = PATH_SAVED_HOTTIES + @"_TopPicks\" + id + "\\";
                        Directory.CreateDirectory(path);
                        foreach(var photo in topPick.User.Photos)
                        {
                            photo.DownloadToFolder(path);
                        }

                        if (topPick.Instagram != null)
                        {
                            var instaPath = path + "instagram\\";
                            Directory.CreateDirectory(instaPath);
                            foreach (var photo in topPick.Instagram.Photos)
                            {
                                photo.DownloadToFolder(instaPath);
                            }
                            
                            topPick.Instagram.DownloadProfilePicToFolder(path);
                        }

                        float score = 0.1f;

                        if (topPick.User.Biography.Length >= 32)
                        {
                            if (_filters.Any(f => f.Match(topPick.User.Biography)))
                                score = -0.64f;
                            else
                                score = 0.24f;
                        }

                        var bioScanResults = API.BioScan(topPick.User.Biography);
                        if (bioScanResults != null)
                            bioScanResults.SaveDictAs(path + "bio_usernames.txt");

                        API.SaveAllRelevantInfo(
                            topPick, 
                            @"E:\Programming\Tinder\", 
                            score
                        );
                        progress.Report(null);
                    }

                    Settings.Default.top_picks_refresh = new DateTime(_now.Year, _now.Month, _now.Day, 23, 59, 59);
                    Settings.Default.Save();
                }
                
                progress.Report(new LoadingProgress(-1, "Checking Superlikable..."));
                if (API.GetSuperLikable(out V2BaseModel<SuperlikableResponseData> superlikable))
                {
                    progress.Report(new LoadingProgress(superlikable.data.Results.Length, "Saving Superlikable candidates..."));
                    foreach(var topPick in superlikable.data.Results)
                    {
                        if (!TopPicks.Add(topPick.s_number))
                            continue;
                        bytes = BitConverter.GetBytes(topPick.s_number);
                        file.Write(bytes, 0, 4);

                        var id = topPick.User.Name + "_" + topPick.User.Birthday.ToString("yyMMdd");
                        var path = PATH_SAVED_SOULMATES + @"_SuperLikable\" + id + "\\";
                        Directory.CreateDirectory(path);
                        foreach (var photo in topPick.User.Photos)
                        {
                            photo.DownloadToFolder(path);
                        }

                        if (topPick.Instagram != null)
                        {
                            var instaPath = path + "instagram\\";
                            Directory.CreateDirectory(instaPath);
                            foreach (var photo in topPick.Instagram.Photos)
                            {
                                photo.DownloadToFolder(instaPath);
                            }

                            topPick.Instagram.DownloadProfilePicToFolder(path);
                        }

                        var bioScanResults = API.BioScan(topPick.User.Biography);
                        if (bioScanResults != null)
                            bioScanResults.SaveDictAs(path + "bio_usernames.txt");

                        API.SaveAllRelevantInfo(
                            topPick,
                            @"E:\Programming\Tinder\",
                            0.49f + (topPick.User.Biography.Length >= 32 ? 0.25f : 0)
                        );
                        File.Copy(@"E:\Programming\Tinder\" + id + ".json", path + topPick.User.ID + ".json");
                        progress.Report(null);
                    }
                }
            }

            progress.Report(new LoadingProgress());
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _historyWriter.Close();
            Application.Exit();
        }
    }
}
