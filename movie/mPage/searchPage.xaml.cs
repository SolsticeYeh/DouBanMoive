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
    public sealed partial class searchPage : Page
    {
        public searchPage()
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
            };
            service.check();
        }
        private async void add_Click(object sender, RoutedEventArgs e)
        {
            if (service.loadStatus == 1)
            {
                List<string> directorsName = new List<string>();
                List<string> directorsPic = new List<string>();
                List<string> castsName = new List<string>();
                List<string> castsPic = new List<string>();
                foreach (var item in chooseItem.Directors)
                {
                    directorsName.Add(item.name);
                    directorsPic.Add(item.Avatars.medium);
                }
                foreach (var item in chooseItem.casts)
                {
                    castsName.Add(item.name);
                    castsPic.Add(item.Avatars.medium);
                }
                LoveModel love = new LoveModel
                {
                    MidImage = chooseItem.images.small,
                    title = chooseItem.title,
                    original_title = chooseItem.original_title,
                    year = chooseItem.year,
                    BigImage = chooseItem.images.large,
                    rating = chooseItem.rating.average,
                    summary = summary.Text,
                    genres = string.Join(",", chooseItem.genres.ToArray()),
                    DirectorsName = string.Join(",", directorsName.ToArray()),
                    DirectorsPic = string.Join(",", directorsPic.ToArray()),
                    CastsName = string.Join(",", castsName.ToArray()),
                    CastsPic = string.Join(",", castsPic.ToArray()),
                    id = chooseItem.id
                };
                var query = await (service.db.Table<LoveModel>().Where(v => v.id == chooseItem.id)).ToListAsync();
                if (query.Count() == 0)
                    await service.db.InsertAsync(love);
                else
                {
                    XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                    XmlNodeList elements = toastXml.GetElementsByTagName("text");
                    elements[0].AppendChild(toastXml.CreateTextNode("《" + chooseItem.title + "》已经添加过了！"));
                    ToastNotification toast = new ToastNotification(toastXml);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
            else
            {
                var dig = new MessageDialog("请等待加载完毕", "提示");
                var btnOk = new UICommand("确定");
                dig.Commands.Add(btnOk);
                var result = await dig.ShowAsync();
                if (null != result && result.Label == "确定")
                {
                }
            }
        }
        private void search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ListView.Visibility = Visibility.Collapsed;
            netWork(search.Text);
        }
        ObservableCollection<nowModel> list = new ObservableCollection<nowModel>();
        private int error = 0;

        private void continueStatus_Tapped(object sender, TappedRoutedEventArgs e)
        {
            netWork(search.Text);
        }
        private async void netWork(string context)
        {
            noStatus.Visibility = Visibility.Collapsed;
            loadingStatus.Visibility = Visibility.Visible;
            noneStatus.Visibility = Visibility.Collapsed;
            continueStatus.Visibility = Visibility.Collapsed;
            try
            {
                string content = await GetHttpClient("https://api.douban.com/v2/movie/search?q={"+ context + "}");
                JObject jsonobj = JObject.Parse(content);
                string json = jsonobj["subjects"].ToString();
                list = JsonConvert.DeserializeObject<ObservableCollection<nowModel>>(json);
                ListView.ItemsSource = list;
                ListView.Visibility = Visibility.Visible;
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
            try
            {
                loading.Visibility = Visibility.Visible;
                loadedError.Visibility = Visibility.Collapsed;
                detail.Visibility = Visibility.Collapsed;
                if (chooseItem != (nowModel)e.ClickedItem)
                {
                    service.loadStatus = 0;
                    service.error = 0;
                    chooseItem = (nowModel)e.ClickedItem;
                    titleBlock.Text = chooseItem.title;
                    yearBlock.Text = chooseItem.year;
                    rateBlock.Text = Convert.ToString(chooseItem.rating.average);
                    summary.Text = await getSummry(chooseItem.id);
                    img.Children.Clear();
                    Image bigimg = new Image();
                    HttpClient client = new HttpClient();
                    byte[] bytes = await client.GetByteArrayAsync(chooseItem.images.large);
                    bigimg.Source = await trans.SaveToImageSource(bytes);
                    img.Children.Add(bigimg);
                    genListView.Items.Clear();
                    for (int i = 0; i < chooseItem.genres.Count; i++)
                    {
                        genListView.Items.Add(new Gen { text = chooseItem.genres[i] });
                    }
                    dirListView.ItemsSource = chooseItem.Directors;
                    casListView.ItemsSource = chooseItem.casts;
                }
                loading.Visibility = Visibility.Collapsed;
                if (service.error == 0)
                {
                    detail.Visibility = Visibility.Visible;
                    service.loadStatus = 1;
                }
                else
                    loadedError.Visibility = Visibility.Visible;
            }
            catch
            {
                loading.Visibility = Visibility.Collapsed;
                loadedError.Visibility = Visibility.Visible;
                service.error = 1;
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
        public nowModel chooseItem = new nowModel();
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
                rateBlock.Text = Convert.ToString(chooseItem.rating.average);
                summary.Text = await getSummry(chooseItem.id);
                img.Children.Clear();
                Image bigimg = new Image();
                HttpClient client = new HttpClient();
                byte[] bytes = await client.GetByteArrayAsync(chooseItem.images.large);
                bigimg.Source = await trans.SaveToImageSource(bytes);
                img.Children.Add(bigimg);
                genListView.Items.Clear();
                for (int i = 0; i < chooseItem.genres.Count; i++)
                {
                    genListView.Items.Add(new Gen { text = chooseItem.genres[i] });
                }
                dirListView.ItemsSource = chooseItem.Directors;
                casListView.ItemsSource = chooseItem.casts;
                loading.Visibility = Visibility.Collapsed;
                detail.Visibility = Visibility.Visible;
            }
            catch
            {
                loading.Visibility = Visibility.Collapsed;
                loadedError.Visibility = Visibility.Visible;
            }
        }
    }
}
