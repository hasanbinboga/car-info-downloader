// See https://aka.ms/new-console-template for more information
using Cars2Json.Models;
using CefSharp;
using CefSharp.OffScreen;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Diagnostics;

var carComUrl = "https://www.cars.com/";
var testUrl = "https://www.cars.com/";

Console.WriteLine("This example application will load {0}, take datas and a screenshot, and save image and json file to your desktop.", testUrl);
Console.WriteLine("You may see Chromium fatal error output, please wait...");
Console.WriteLine();
var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var settings = new CefSettings()
{
    //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
    CachePath = Path.Combine(basePath, "CefSharp\\Cache"),
    JavascriptFlags = "--trace-opt",
    IgnoreCertificateErrors = true,
    CookieableSchemesExcludeDefaults = true,
    Locale = "en-US",
    MultiThreadedMessageLoop = true,
    PersistUserPreferences = true,
    PersistSessionCookies = true,
    WindowlessRenderingEnabled = true,
    ChromeRuntime = false,
    LogSeverity = LogSeverity.Error,
    
    PackLoadingDisabled= false,
    

    // BackgroundColor = uint.MaxValue


};

//Perform dependency check to make sure all relevant resources are in our output directory.
Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

// Create the offscreen Chromium browser.
var browser = new ChromiumWebBrowser(testUrl);


EventHandler<LoadingStateChangedEventArgs> handler = null;

var waitBrowser = true;

handler = async (s, e) =>
{
    // Check to see if loading is complete - this event is called twice, one when loading starts
    // second time when it's finished
    if (!e.IsLoading)
    {
        // Remove the load event handler, because we only want one snapshot of the page.
        //browser.LoadingStateChanged -= handler;
        waitBrowser = false;

    }
};

// An event that is fired when the first page is finished loading.
// This returns to us from another thread.
browser.LoadingStateChanged += handler;


while (waitBrowser && !browser.CanExecuteJavascriptInMainFrame)
{
    Thread.Sleep(200);
}

try
{
    var menuClick = browser.EvaluateScriptAsync("document.querySelector('[class=nav-user-menu-button]').click()");
    var signInClick = browser.EvaluateScriptAsync("document.querySelector('[data-component=sign-in-start]').click()");
    var setUserName = browser.EvaluateScriptAsync("document.querySelector('[id=auth-modal-email]').value = 'johngerson808@gmail.com'");
    var setPassword = browser.EvaluateScriptAsync("document.querySelector('[id=auth-modal-current-password]').value = 'test8008'");
    var clickLogin = browser.EvaluateScriptAsync("document.querySelector('cars-auth-modal').form[0].form.submit()");

    await menuClick;
    Console.WriteLine("Menu clicked.");

    await signInClick;
    
    Console.WriteLine("Sign in clicked.");

    await setUserName;

    await setPassword;

    Console.WriteLine("User name and password set.");

    waitBrowser = true;

    await clickLogin;

    Console.WriteLine("Login clicked.");



    while (waitBrowser && !browser.CanExecuteJavascriptInMainFrame)
    {
        Thread.Sleep(1000);
    }

    #region Tesla Model S

    var criteriaList = new List<SearchCriterias>();
    Console.WriteLine("Search Tesla Model S. Get page 1");

    var newCriteria = await SearchWithNewCriterias(page: 1, makes: "tesla", model: "tesla-model_s", stockType: "used", distance: "all", maxPrice: 100000, zipCode: "94596");
    criteriaList.Add(newCriteria);

    Console.WriteLine("Search Tesla Model S. Get page 2");
    newCriteria = await SearchWithNewCriterias(page: 2, makes: "tesla", model: "tesla-model_s", stockType: "used", distance: "all", maxPrice: 100000, zipCode: "94596");
    if (newCriteria.Cars.Count > 0)
    {
        Console.WriteLine("Get first car details ({0})", newCriteria.Cars[0].Title);

        var updatedCar = await GetCarDetailsAsync(newCriteria.Cars[0]);
        newCriteria.Cars[0] = updatedCar;
    }

    criteriaList.Add(newCriteria);

    #endregion

    #region Tesla Model X

    criteriaList = new List<SearchCriterias>();
    Console.WriteLine("Search Tesla Model X. Get page 1");

    newCriteria = await SearchWithNewCriterias(page: 1, makes: "tesla", model: "tesla-model_x", stockType: "used", distance: "all", maxPrice: 100000, zipCode: "94596");
    criteriaList.Add(newCriteria);
    Console.WriteLine("Search Tesla Model X. Get page 2");
    newCriteria = await SearchWithNewCriterias(page: 2, makes: "tesla", model: "tesla-model_x", stockType: "used", distance: "all", maxPrice: 100000, zipCode: "94596");
    if (newCriteria.Cars.Count > 0)
    {
        Console.WriteLine("Get first car details ({0})", newCriteria.Cars[0].Title);

        var updatedCar = await GetCarDetailsAsync(newCriteria.Cars[0]);
        newCriteria.Cars[0] = updatedCar;
    }
    criteriaList.Add(newCriteria);

    #endregion

    Console.WriteLine("Save json file");
    string json = JsonConvert.SerializeObject(criteriaList);
    File.WriteAllText("tesla-cars.json", json);


    // Wait for the screenshot to be taken.
    var image = await browser.CaptureScreenshotAsync();
    // File path to save our screenshot e.g. C:\Users\{username}\Desktop\CefSharp screenshot.png
    var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot.png");

    Console.WriteLine();
    Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

    var bitmapAsByteArray = image;

    // Save the Bitmap to the path.
    File.WriteAllBytes(screenshotPath, bitmapAsByteArray);

    Console.WriteLine("Screenshot saved.  Launching your default image viewer...");

    // Tell Windows to launch the saved image.
    Process.Start(new ProcessStartInfo(screenshotPath)
    {
        // UseShellExecute is false by default on .NET Core.
        UseShellExecute = true
    });

    Console.WriteLine("Image viewer launched.  Press any key to exit.");


    // We have to wait for something, otherwise the process will exit too soon.
    Console.ReadKey();

    // Clean up Chromium objects. You need to call this in your application otherwise
    // you will get a crash when closing.
    //The ChromiumWebBrowser instance will be disposed
    //Cef.Shutdown();

}
catch (Exception)
{

    throw;
}





string GetUrl(SearchCriterias criterias)
{
    NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

    queryString.Add("page", criterias.Page.ToString());
    queryString.Add("page_size", criterias.PageSize.ToString());
    queryString.Add("list_price_max", criterias.MaxPrice.ToString());
    queryString.Add("makes%5B%5D", criterias.Makes);
    queryString.Add("models%5B%5D", criterias.Models);
    queryString.Add("maximum_distance", criterias.Distance);
    queryString.Add("stock_type", criterias.StockType);
    queryString.Add("zip", criterias.ZipCode);
    if (criterias.HomeDelivery.HasValue)
    {
        queryString.Add("home_delivery", criterias.HomeDelivery.Value.ToString());

    }
    var url = "https://www.cars.com/shopping/results/?" + queryString.ToString();
    return  url;
}


async Task<SearchCriterias> SearchWithNewCriterias(int page = 1, int pageSize = 20,
                                                   double? maxPrice = null,
                                                   string makes = "", string model = "",
                                                   string stockType = "",
                                                   string zipCode = "", string distance = "",
                                                   bool? homeDelivery = null)
{
    var newCriteria = new SearchCriterias
    {
        Distance = distance,
        StockType = stockType,
        Makes = makes,
        Models = model,
        MaxPrice = maxPrice,
        ZipCode = zipCode,
        HomeDelivery = homeDelivery,
        Page = page,
        PageSize = pageSize,
        
    };

    waitBrowser = true;
    Thread.Sleep(1000);
    await browser.LoadUrlAsync(GetUrl(newCriteria));

    //Give the browser a little time to render 
    while (waitBrowser && !browser.CanExecuteJavascriptInMainFrame)
    {
        Thread.Sleep(1000);
    }
    var rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('div[class=\"sds-page-section listings-page\"]').getAttribute(\"data-site-activity\")");
    newCriteria.RawData = JsonConvert.DeserializeObject(rawDataReq.Result.ToString());
    var dataCountReq = await browser.EvaluateScriptAsync("document.querySelectorAll('div[data-tracking-type=\"srp-vehicle-card\"]').length");
    var dataCount = 0;

    int.TryParse(dataCountReq.Result.ToString(), out dataCount);

    if (newCriteria.RawData != null && newCriteria?.RawData?.vehicleArray != null)
    {
        for (int i = 0; i < dataCount; i++)
        {
            var newCar = await GetCarInfo(newCriteria?.RawData?.vehicleArray[i], i);
            newCriteria.Cars.Add(newCar);
        }
    }

    return newCriteria;

}

async Task<Car> GetCarInfo(dynamic carInfo, int i)
{
    var newCar = new Car();

    newCar.Id = carInfo.listing_id;
    newCar.Make = carInfo.make;
    newCar.Model = carInfo.model;
    newCar.Trim = carInfo.trim;
    newCar.Year = carInfo.year;
    newCar.Title = carInfo.year + " " + carInfo.make + " " + carInfo.model + " " + carInfo.trim;
    newCar.Url = carComUrl + $"vehicledetail/{carInfo.listing_id}";
    newCar.Vin = carInfo.vin;
    newCar.BodyStyle = carInfo.bodystyle;
    newCar.Category = carInfo.cat;
    newCar.CustomerId = carInfo.customer_id;
    newCar.ExteriorColor = carInfo.exterior_color;
    newCar.FuelType = carInfo.fuel_type;
    newCar.Mileage = carInfo.mileage;
    newCar.Price = carInfo.price;
    newCar.SellerType = carInfo.seller_type;
    newCar.StockType = carInfo.stock_type;
    newCar.IsGreatDeal = carInfo.badges != null && carInfo.badges.Count > 0 && carInfo.badges.Contains("great_deal");
    newCar.IsHomeDelivery = carInfo.badges != null && carInfo.badges.Count > 0 && carInfo.badges.Contains("home_delivery");
    newCar.IsVirtualAppointments = carInfo.badges != null && carInfo.badges.Count > 0 && carInfo.badges.Contains("virtual_appt");

    #region Get images

    var imageCountReq = await browser.EvaluateScriptAsync($"document.querySelectorAll('div[data-tracking-type=\"srp-vehicle-card\"]')[{i}].querySelectorAll('div[class=\"image-wrap\"').length");
    var imageCount = 0;
    if (imageCountReq.Result != null)
    {
        int.TryParse(imageCountReq.Result.ToString(), out imageCount);
    }

    for (int j = 0; j < imageCount; j++)
    {
        var imageReq = await browser.EvaluateScriptAsync($"document.querySelectorAll('div[data-tracking-type=\"srp-vehicle-card\"]')[{i}].querySelectorAll('div[class=\"image-wrap\"')[{j}].querySelector('img').getAttribute('data-src')");
        if (imageReq != null && imageReq.Result != null && !string.IsNullOrEmpty(imageReq.Result.ToString()))
        {
            newCar.GalleryUrls.Add(imageReq.Result.ToString());
        }
    }

    #endregion

    return newCar;
}

async Task<Car> GetCarDetailsAsync(Car car)
{
    waitBrowser = true;
    var currCarUrl = car.Url;
    Thread.Sleep(1000);
    await browser.LoadUrlAsync(currCarUrl);

    Thread.Sleep(200);
    //Give the browser a little time to render 
    while (waitBrowser && !browser.CanExecuteJavascriptInMainFrame)
    {
        Thread.Sleep(1000);
    }


    var rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('dl[class=\"fancy-description-list\"]').children[3].textContent");
    car.InteriorColor = rawDataReq?.Result?.ToString();
    
    rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('dl[class=\"fancy-description-list\"]').children[5].textContent");
    car.Drivetrain = "";

    rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('dl[class=\"fancy-description-list\"]').children[9].textContent");
    car.Transmission = rawDataReq?.Result?.ToString();

    rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('dl[class=\"fancy-description-list\"]').children[11].textContent");
    car.Engine = rawDataReq?.Result?.ToString();

    rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('div[class=\"sellers-notes\"]').textContent");
    car.SellersNotes = rawDataReq?.Result?.ToString();

    rawDataReq = await browser.EvaluateScriptAsync("document.querySelector('cars-price-history').getAttribute(\"price-history-data\")");
    var priceHistoryRawData = JsonConvert.DeserializeObject<dynamic>(rawDataReq.Result.ToString());

    foreach (var item in (List<dynamic>) priceHistoryRawData.price_history)
    {
        car.PriceHistories.Add(new PriceHistory { Date = item.inserted_at, Price = item.list_price });
    }
    car.PriceGoodThreshold = priceHistoryRawData.good_threshold;
    car.GreatThreshold = priceHistoryRawData.great_threshold;
    car.PredictedPrice = priceHistoryRawData.predicted_price;
    car.ListedDays = priceHistoryRawData.listed_days;


    return car;
}