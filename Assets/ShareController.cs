
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShareController : MonoBehaviour
{

    public string AndroidStoreLink;
    public string IOSStoreLink;
    public Texture2D whatsAppShareImg;
    public string shareText;

    // Use this for initialization
    void Start()
    {

        //GetSocial.RegisterInviteChannelPlugin(InviteChannelIds.Facebook, new FacebookSharePlugin());

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WhatsAppShareUICallback()
    {

        /*
        if (NPBinding.Sharing.IsWhatsAppServiceAvailable())
        {
            VoxelBusters.NativePlugins.WhatsAppShareComposer wsc = new VoxelBusters.NativePlugins.WhatsAppShareComposer();

#if UNITY_ANDROID
            wsc.AttachImage(whatsAppShareImg);
#endif

            wsc.Text = shareText;
            if (AndroidStoreLink != "") wsc.Text += " \nDescarga para Android: " +
                    AndroidStoreLink;
            if(IOSStoreLink != "") wsc.Text += "\nDescarga para iOS: " + IOSStoreLink;
            NPBinding.Sharing.ShowView(wsc, finishWhatsappShare);
            //VoxelBusters.NativePlugins.SharingAndroid
        }
        else
        {

            bool onIOS = false;
#if UNITY_IOS
            onIOS = true;
#endif

            if (!onIOS) Application.OpenURL("https://play.google.com/store/apps/details?id=com.whatsapp");
            else Application.OpenURL("https://itunes.apple.com/es/app/whatsapp-messenger/id310633997?mt=8");

        }
    }
    public void finishWhatsappShare(VoxelBusters.NativePlugins.eShareResult _result)
    {
        // do shit
        Debug.Log("result: " + _result);
    }


*/

    }
}
