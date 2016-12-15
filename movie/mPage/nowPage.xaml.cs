using SQLite;
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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace movie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class nowPage : Page
    {
        public nowPage()
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
        ObservableCollection<nowModel> list = new ObservableCollection<nowModel>();
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
                string content = await GetHttpClient("https://api.douban.com/v2/movie/in_theaters");
                JObject jsonobj = JObject.Parse(content);
                string json = jsonobj["subjects"].ToString();
                list = JsonConvert.DeserializeObject<ObservableCollection<nowModel>>(json);
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
                if (chooseItem != (nowModel)e.ClickedItem)
                {
                    service.loadStatus = 0;
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
                detail.Visibility = Visibility.Visible;
                service.loadStatus = 1;
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
    }
}
public class nowModel
{
    [PrimaryKey, AutoIncrement]
    public int _Id { get; set; }
    public Rating rating { get; set; }
    public List<string> genres { get; set; }
    public string title { get; set; }
    public Casts[] casts { get; set; }
    public string original_title { get; set; }
    public directors[] Directors { get; set; }
    public Images images { get; set; }
    public string year { get; set; }
    public string id { get; set; }
    
}
public class Rating
{
    public int max { get; set; }
    public double average { get; set; }
    public string stars { get; set; }
    public int min { get; set; }
}
public class Casts
{
    public string name { get; set; }
    public avatars Avatars { get; set; }
}
public class Images
{
    public string small { get; set; }
    public string large { get; set; }
    public string medium { get; set; }
}
public class avatars
{
    public string small { get; set; }
    public string large { get; set; }
    public string medium { get; set; }
}

public class directors
{
    public string name { get; set; }
    public avatars Avatars { get; set; }
}
public static class trans
{
    public static async Task<ImageSource> SaveToImageSource(this byte[] imageBuffer)
    {
        ImageSource imageSource = null;
        using (MemoryStream stream = new MemoryStream(imageBuffer))
        {
            var ras = stream.AsRandomAccessStream();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.JpegDecoderId, ras);
            var provider = await decoder.GetPixelDataAsync();
            byte[] buffer = provider.DetachPixelData();
            WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
            await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
            imageSource = bitmap;
        }
        return imageSource;
    }

}