using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace movie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class lovePage : Page
    {
        public lovePage()
        {
            this.InitializeComponent();
            this.SizeChanged += (s, e) =>
            {
                var state = "VisualState000";
                if (service.status == 1)
                    state = "VisualState001";
                if (e.NewSize.Width > 700)
                {
                    state = "VisualState700";
                }
                if (e.NewSize.Width > 700 && service.status == 1)
                {
                    state = "VisualState701";
                }
                VisualStateManager.GoToState(this, state, true);
                service.st = e.NewSize.Width;
                service.check();
            };
        }
        ObservableCollection<LikeModel> list = new ObservableCollection<LikeModel>();
        ObservableCollection<LoveModel> Like = new ObservableCollection<LoveModel>();
        private int error = 0;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            netWork();
        }

        private void continueStatus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            netWork();
        }
        private async void netWork()
        {
            loadingStatus.Visibility = Visibility.Visible;
            noneStatus.Visibility = Visibility.Collapsed;
            continueStatus.Visibility = Visibility.Collapsed;
            try
            {
                await Task.Delay(60);
                var query = await (service.db.Table<LoveModel>().Where(v => v._Id >= 1)).ToListAsync();
                Like = new ObservableCollection<LoveModel>(query);
                foreach(var item in Like)
                {
                    string[] tig = item.genres.Split(',');
                    string[] dName= item.DirectorsName.Split(',');
                    string[] dPic= item.DirectorsPic.Split(',');
                    string[] cName= item.CastsName.Split(',');
                    string[] cPic= item.CastsPic.Split(',');
                    List<People> Dir = new List<People>();
                    List<People> Cas = new List<People>();
                    for (int i = 0; i < dName.Count(); i++)
                    {
                        Dir.Add(new People { name = dName[i], Pic = dPic[i] });
                    }
                    for (int j = 0; j < cName.Count(); j++)
                    {
                        Cas.Add(new People { name = cName[j], Pic = cPic[j] });
                    }
                    list.Add(new LikeModel { _Id = item._Id, MidImage = item.MidImage, title = item.title, original_title = item.original_title, year = item.year, BigImage = item.BigImage, rating = item.rating, summary = item.summary, genres = tig, Directors = Dir, Casts = Cas, id = item.id });
                }
                ListView.ItemsSource = list;
            }
            catch
            {
                error = 1;
            }
            if (error == 0)
            {
                loadingStatus.Visibility = Visibility.Collapsed;
                noneStatus.Visibility = Visibility.Visible;
                continueStatus.Visibility = Visibility.Collapsed;
            }
            if (error == 1)
            {
                loadingStatus.Visibility = Visibility.Collapsed;
                noneStatus.Visibility = Visibility.Collapsed;
                continueStatus.Visibility = Visibility.Visible;
                error = 0;
            }
        }
        private async Task<string> GetHttpClient(string uri)
        {
            string content = "";
            return await Task.Run(() =>
            {
                HttpClient httpClient = new HttpClient();
                System.Net.Http.HttpResponseMessage response;
                try
                {
                    response = httpClient.GetAsync(new Uri(uri)).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                        content = response.Content.ReadAsStringAsync().Result;
                }
                catch
                {
                    error = 1;
                }
                return content;
            });
        }

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            service.status = 1;
            var state = "VisualState000";
            if (service.st < 700)
                state = "VisualState001";
            if (service.st > 700)
                state = "VisualState701";
            VisualStateManager.GoToState(this, state, true);
            loading.Visibility = Visibility.Visible;
            loadedError.Visibility = Visibility.Collapsed;
            detail.Visibility = Visibility.Collapsed;
            try
            {
                if (chooseItem != (LikeModel)e.ClickedItem)
                {
                    chooseItem = (LikeModel)e.ClickedItem;
                    titleBlock.Text = chooseItem.title;
                    yearBlock.Text = chooseItem.year;
                    rateBlock.Text = Convert.ToString(chooseItem.rating);
                    summary.Text = chooseItem.summary;
                    img.Children.Clear();
                    Image bigimg = new Image();
                    HttpClient client = new HttpClient();
                    byte[] bytes = await client.GetByteArrayAsync(chooseItem.BigImage);
                    bigimg.Source = await trans.SaveToImageSource(bytes);
                    img.Children.Add(bigimg);
                    genListView.Items.Clear();
                    for (int i = 0; i < chooseItem.genres.Count(); i++)
                    {
                        genListView.Items.Add(new Gen { text = chooseItem.genres[i] });
                    }
                    dirListView.ItemsSource = chooseItem.Directors;
                    casListView.ItemsSource = chooseItem.Casts;
                }
                loading.Visibility = Visibility.Collapsed;
                detail.Visibility = Visibility.Visible;
            }
            catch
            {
                loading.Visibility = Visibility.Collapsed;
                loadedError.Visibility = Visibility.Visible;
            }
        }
        private async Task<string> getSummry(string x)
        {
            try
            {
                string content = await GetHttpClient("https://api.douban.com/v2/movie/subject/" + x);
                JObject jsonobj = JObject.Parse(content);
                string json = jsonobj["summary"].ToString();
                return json;
            }
            catch
            {
                return "暂无简介";
            }
        }
        private void back_Click(object sender, RoutedEventArgs e)
        {
            service.status = 0;
            var state = "VisualState000";
            if (service.st > 700)
                state = "VisualState700";
            VisualStateManager.GoToState(this, state, true);
        }
        public LikeModel chooseItem = new LikeModel();
        private class Gen
        {
            public string text { get; set; }

        }

        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            loading.Visibility = Visibility.Visible;
            loadedError.Visibility = Visibility.Collapsed;
            detail.Visibility = Visibility.Collapsed;
            try
            {
                titleBlock.Text = chooseItem.title;
                yearBlock.Text = chooseItem.year;
                rateBlock.Text = Convert.ToString(chooseItem.rating);
                summary.Text = await getSummry(chooseItem.id);
                img.Children.Clear();
                Image bigimg = new Image();
                HttpClient client = new HttpClient();
                byte[] bytes = await client.GetByteArrayAsync(chooseItem.BigImage);
                bigimg.Source = await trans.SaveToImageSource(bytes);
                img.Children.Add(bigimg);
                genListView.Items.Clear();
                for (int i = 0; i < chooseItem.genres.Count(); i++)
                {
                    genListView.Items.Add(new Gen { text = chooseItem.genres[i] });
                }
                dirListView.ItemsSource = chooseItem.Directors;
                casListView.ItemsSource = chooseItem.Casts;
                loading.Visibility = Visibility.Collapsed;
                detail.Visibility = Visibility.Visible;
            }
            catch
            {
                loading.Visibility = Visibility.Collapsed;
                loadedError.Visibility = Visibility.Visible;
            }
        }

        private async void delete_Click(object sender, RoutedEventArgs e)
        {
            var dig = new MessageDialog("确定删除该条记录？", "提示");
            var btnOk = new UICommand("是");
            dig.Commands.Add(btnOk);
            var btnCancel = new UICommand("否");
            dig.Commands.Add(btnCancel);
            var result = await dig.ShowAsync();
            if (null != result && result.Label == "是")
            {
                var query = await (service.db.Table<LoveModel>().Where(v => v.id == chooseItem.id).ToListAsync());
                await service.db.DeleteAsync(query[0]);
                list.Remove(list.Where(v => v.id == chooseItem.id).FirstOrDefault());
                ListView.ItemsSource = list;
                service.status = 0;
                var state = "VisualState000";
                if (service.st > 700)
                    state = "VisualState700";
                VisualStateManager.GoToState(this, state, true);
            }
            else if (null != result && result.Label == "否")
            {
            }
        }
    }
}
