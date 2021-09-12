using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMob : SingletonMonoBehaviour<AdMob>
{
    private BannerView bannerView;
    private InterstitialAd interstitial;

    public void Start()
    {
        MobileAds.Initialize(initStatus => {
            RequestBanner();

            RequestInterstitial();
            interstitial.OnAdClosed += delegate (object sender, EventArgs args)    // 閉じたら再ロードするようにする
            {
                interstitial.Destroy();
                RequestInterstitial();
            };
        });
    }

    private void RequestBanner()
    {

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // TEST
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716"; // TEST
#else
        string adUnitId = "unexpected_platform";
#endif

        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712"; // TEST
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910"; // TEST
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(adUnitId);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void DisplayInterstitial()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
        else
        {
            Debug.Log("Interstitial is not ready yet.");
        }
    }
}